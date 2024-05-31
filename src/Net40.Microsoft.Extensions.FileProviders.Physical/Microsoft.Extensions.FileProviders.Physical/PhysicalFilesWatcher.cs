using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders.Physical.Internal;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Primitives;
using Theraot.Collections.Specialized;

namespace Microsoft.Extensions.FileProviders.Physical;

public class PhysicalFilesWatcher : IDisposable
{
	private readonly struct ChangeTokenInfo
	{
		public CancellationTokenSource TokenSource { get; }

		public CancellationChangeToken ChangeToken { get; }

		public Matcher Matcher { get; }

		public ChangeTokenInfo(CancellationTokenSource tokenSource, CancellationChangeToken changeToken)
			: this(tokenSource, changeToken, null)
		{
		}

		public ChangeTokenInfo(CancellationTokenSource tokenSource, CancellationChangeToken changeToken, Matcher matcher)
		{
			TokenSource = tokenSource;
			ChangeToken = changeToken;
			Matcher = matcher;
		}
	}

	private static readonly Action<object> _cancelTokenSource = delegate(object state)
	{
		((CancellationTokenSource)state).Cancel();
	};

	internal static TimeSpan DefaultPollingInterval = TimeSpan.FromSeconds(4.0);

	private readonly ConcurrentDictionary<string, ChangeTokenInfo> _filePathTokenLookup = new ConcurrentDictionary<string, ChangeTokenInfo>(StringComparer.OrdinalIgnoreCase);

	private readonly ConcurrentDictionary<string, ChangeTokenInfo> _wildcardTokenLookup = new ConcurrentDictionary<string, ChangeTokenInfo>(StringComparer.OrdinalIgnoreCase);

	private readonly FileSystemWatcher _fileWatcher;

	private readonly object _fileWatcherLock = new object();

	private readonly string _root;

	private readonly ExclusionFilters _filters;

	private Timer _timer;

	private bool _timerInitialzed;

	private object _timerLock = new object();

	private Func<Timer> _timerFactory;

	internal bool PollForChanges { get; }

	internal bool UseActivePolling { get; set; }

	internal ConcurrentDictionary<IPollingChangeToken, IPollingChangeToken> PollingChangeTokens { get; }

	public PhysicalFilesWatcher(string root, FileSystemWatcher fileSystemWatcher, bool pollForChanges)
		: this(root, fileSystemWatcher, pollForChanges, ExclusionFilters.Sensitive)
	{
	}

	public PhysicalFilesWatcher(string root, FileSystemWatcher fileSystemWatcher, bool pollForChanges, ExclusionFilters filters)
	{
		_root = root;
		_fileWatcher = fileSystemWatcher;
		_fileWatcher.IncludeSubdirectories = true;
		_fileWatcher.Created += OnChanged;
		_fileWatcher.Changed += OnChanged;
		_fileWatcher.Renamed += OnRenamed;
		_fileWatcher.Deleted += OnChanged;
		_fileWatcher.Error += OnError;
		PollForChanges = pollForChanges;
		_filters = filters;
		PollingChangeTokens = new ConcurrentDictionary<IPollingChangeToken, IPollingChangeToken>();
		_timerFactory = () => NonCapturingTimer.Create(RaiseChangeEvents, PollingChangeTokens, TimeSpan.Zero, DefaultPollingInterval);
	}

	public IChangeToken CreateFileChangeToken(string filter)
	{
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		filter = NormalizePath(filter);
		if (Path.IsPathRooted(filter) || PathUtils.PathNavigatesAboveRoot(filter))
		{
			return NullChangeToken.Singleton;
		}
		IChangeToken changeToken = GetOrAddChangeToken(filter);
		TryEnableFileSystemWatcher();
		return changeToken;
	}

	private IChangeToken GetOrAddChangeToken(string pattern)
	{
		if (UseActivePolling)
		{
			LazyInitializer.EnsureInitialized(ref _timer, ref _timerInitialzed, ref _timerLock, _timerFactory);
		}
		if (pattern.IndexOf('*') != -1 || IsDirectoryPath(pattern))
		{
			return GetOrAddWildcardChangeToken(pattern);
		}
		return GetOrAddFilePathChangeToken(pattern);
	}

	internal IChangeToken GetOrAddFilePathChangeToken(string filePath)
	{
		if (!_filePathTokenLookup.TryGetValue(filePath, out var tokenInfo))
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			CancellationChangeToken cancellationChangeToken = new CancellationChangeToken(cancellationTokenSource.Token);
			tokenInfo = new ChangeTokenInfo(cancellationTokenSource, cancellationChangeToken);
			tokenInfo = _filePathTokenLookup.GetOrAdd(filePath, tokenInfo);
		}
		IChangeToken changeToken = tokenInfo.ChangeToken;
		if (PollForChanges)
		{
			PollingFileChangeToken pollingChangeToken = new PollingFileChangeToken(new FileInfo(Path.Combine(_root, filePath)));
			if (UseActivePolling)
			{
				pollingChangeToken.ActiveChangeCallbacks = true;
				pollingChangeToken.CancellationTokenSource = new CancellationTokenSource();
				PollingChangeTokens.TryAdd(pollingChangeToken, pollingChangeToken);
			}
			changeToken = new CompositeChangeToken(EnumerationList<IChangeToken>.Create(new IChangeToken[2] { changeToken, pollingChangeToken }));
		}
		return changeToken;
	}

	internal IChangeToken GetOrAddWildcardChangeToken(string pattern)
	{
		if (!_wildcardTokenLookup.TryGetValue(pattern, out var tokenInfo))
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			CancellationChangeToken cancellationChangeToken = new CancellationChangeToken(cancellationTokenSource.Token);
			Matcher matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
			matcher.AddInclude(pattern);
			tokenInfo = new ChangeTokenInfo(cancellationTokenSource, cancellationChangeToken, matcher);
			tokenInfo = _wildcardTokenLookup.GetOrAdd(pattern, tokenInfo);
		}
		IChangeToken changeToken = tokenInfo.ChangeToken;
		if (PollForChanges)
		{
			PollingWildCardChangeToken pollingChangeToken = new PollingWildCardChangeToken(_root, pattern);
			if (UseActivePolling)
			{
				pollingChangeToken.ActiveChangeCallbacks = true;
				pollingChangeToken.CancellationTokenSource = new CancellationTokenSource();
				PollingChangeTokens.TryAdd(pollingChangeToken, pollingChangeToken);
			}
			changeToken = new CompositeChangeToken(EnumerationList<IChangeToken>.Create(new IChangeToken[2] { changeToken, pollingChangeToken }));
		}
		return changeToken;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		_fileWatcher.Dispose();
		_timer?.Dispose();
	}

	~PhysicalFilesWatcher()
	{
		Dispose(disposing: false);
	}

	private void OnRenamed(object sender, RenamedEventArgs e)
	{
		OnFileSystemEntryChange(e.OldFullPath);
		OnFileSystemEntryChange(e.FullPath);
		if (!Directory.Exists(e.FullPath))
		{
			return;
		}
		try
		{
			foreach (string newLocation in Directory.EnumerateFileSystemEntries(e.FullPath, "*", SearchOption.AllDirectories))
			{
				string oldLocation = Path.Combine(e.OldFullPath, newLocation.Substring(e.FullPath.Length + 1));
				OnFileSystemEntryChange(oldLocation);
				OnFileSystemEntryChange(newLocation);
			}
		}
		catch (Exception ex) when (ex is IOException || ex is SecurityException || ex is DirectoryNotFoundException || ex is UnauthorizedAccessException)
		{
		}
	}

	private void OnChanged(object sender, FileSystemEventArgs e)
	{
		OnFileSystemEntryChange(e.FullPath);
	}

	private void OnError(object sender, ErrorEventArgs e)
	{
		foreach (string path in _filePathTokenLookup.Keys)
		{
			ReportChangeForMatchedEntries(path);
		}
	}

	private void OnFileSystemEntryChange(string fullPath)
	{
		try
		{
			FileInfo fileSystemInfo = new FileInfo(fullPath);
			if (!FileSystemInfoHelper.IsExcluded(fileSystemInfo, _filters))
			{
				string relativePath = fullPath.Substring(_root.Length);
				ReportChangeForMatchedEntries(relativePath);
			}
		}
		catch (Exception ex) when (ex is IOException || ex is SecurityException || ex is UnauthorizedAccessException)
		{
		}
	}

	private void ReportChangeForMatchedEntries(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		path = NormalizePath(path);
		bool matched = false;
		if (_filePathTokenLookup.TryRemove(path, out var matchInfo))
		{
			CancelToken(matchInfo);
			matched = true;
		}
		foreach (KeyValuePair<string, ChangeTokenInfo> wildCardEntry in _wildcardTokenLookup)
		{
			PatternMatchingResult matchResult = wildCardEntry.Value.Matcher.Match(path);
			if (matchResult.HasMatches && _wildcardTokenLookup.TryRemove(wildCardEntry.Key, out matchInfo))
			{
				CancelToken(matchInfo);
				matched = true;
			}
		}
		if (matched)
		{
			TryDisableFileSystemWatcher();
		}
	}

	private void TryDisableFileSystemWatcher()
	{
		lock (_fileWatcherLock)
		{
			if (_filePathTokenLookup.IsEmpty && _wildcardTokenLookup.IsEmpty && _fileWatcher.EnableRaisingEvents)
			{
				_fileWatcher.EnableRaisingEvents = false;
			}
		}
	}

	private void TryEnableFileSystemWatcher()
	{
		lock (_fileWatcherLock)
		{
			if ((!_filePathTokenLookup.IsEmpty || !_wildcardTokenLookup.IsEmpty) && !_fileWatcher.EnableRaisingEvents)
			{
				_fileWatcher.EnableRaisingEvents = true;
			}
		}
	}

	private static string NormalizePath(string filter)
	{
		return filter = filter.Replace('\\', '/');
	}

	private static bool IsDirectoryPath(string path)
	{
		return path.Length > 0 && (path[path.Length - 1] == Path.DirectorySeparatorChar || path[path.Length - 1] == Path.AltDirectorySeparatorChar);
	}

	private static void CancelToken(ChangeTokenInfo matchInfo)
	{
		if (!matchInfo.TokenSource.IsCancellationRequested)
		{
			Task.Factory.StartNew(_cancelTokenSource, matchInfo.TokenSource, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
		}
	}

	internal static void RaiseChangeEvents(object state)
	{
		ConcurrentDictionary<IPollingChangeToken, IPollingChangeToken> changeTokens = (ConcurrentDictionary<IPollingChangeToken, IPollingChangeToken>)state;
		foreach (KeyValuePair<IPollingChangeToken, IPollingChangeToken> item in changeTokens)
		{
			IPollingChangeToken token = item.Key;
			if (token.HasChanged && changeTokens.TryRemove(token, out var _))
			{
				try
				{
					token.CancellationTokenSource.Cancel();
				}
				catch
				{
				}
			}
		}
	}
}
