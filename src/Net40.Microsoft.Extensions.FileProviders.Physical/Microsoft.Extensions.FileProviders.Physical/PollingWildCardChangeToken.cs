#define DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.FileProviders.Physical;

public class PollingWildCardChangeToken : IPollingChangeToken, IChangeToken
{
	private static readonly byte[] Separator = Encoding.Unicode.GetBytes("|");

	private readonly object _enumerationLock = new object();

	private readonly DirectoryInfoBase _directoryInfo;

	private readonly Matcher _matcher;

	private bool _changed;

	private DateTime? _lastScanTimeUtc;

	private byte[] _byteBuffer;

	private byte[] _previousHash;

	private CancellationTokenSource _tokenSource;

	private CancellationChangeToken _changeToken;

	public bool ActiveChangeCallbacks { get; internal set; }

	internal TimeSpan PollingInterval { get; set; } = PhysicalFilesWatcher.DefaultPollingInterval;


	internal CancellationTokenSource CancellationTokenSource
	{
		get
		{
			return _tokenSource;
		}
		set
		{
			Debug.Assert(_tokenSource == null, "We expect CancellationTokenSource to be initialized exactly once.");
			_tokenSource = value;
			_changeToken = new CancellationChangeToken(_tokenSource.Token);
		}
	}

	CancellationTokenSource IPollingChangeToken.CancellationTokenSource => CancellationTokenSource;

	private IClock Clock { get; }

	public bool HasChanged
	{
		get
		{
			if (_changed)
			{
				return _changed;
			}
			DateTime utcNow = Clock.UtcNow;
			DateTime? lastScanTimeUtc = _lastScanTimeUtc;
			if (utcNow - lastScanTimeUtc >= PollingInterval)
			{
				lock (_enumerationLock)
				{
					_changed = CalculateChanges();
				}
			}
			return _changed;
		}
	}

	public PollingWildCardChangeToken(string root, string pattern)
		: this(new DirectoryInfoWrapper(new DirectoryInfo(root)), pattern, Microsoft.Extensions.FileProviders.Physical.Clock.Instance)
	{
	}

	internal PollingWildCardChangeToken(DirectoryInfoBase directoryInfo, string pattern, IClock clock)
	{
		_directoryInfo = directoryInfo;
		Clock = clock;
		_matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
		_matcher.AddInclude(pattern);
		CalculateChanges();
	}

	private bool CalculateChanges()
	{
		throw new NotImplementedException("Не реализованно");
	}

	protected virtual DateTime GetLastWriteUtc(string path)
	{
		return File.GetLastWriteTimeUtc(Path.Combine(_directoryInfo.FullName, path));
	}

	private static bool ArrayEquals(byte[] previousHash, byte[] currentHash)
	{
		if (previousHash == null)
		{
			return true;
		}
		Debug.Assert(previousHash.Length == currentHash.Length);
		for (int i = 0; i < previousHash.Length; i++)
		{
			if (previousHash[i] != currentHash[i])
			{
				return false;
			}
		}
		return true;
	}

	IDisposable IChangeToken.RegisterChangeCallback(Action<object> callback, object state)
	{
		if (!ActiveChangeCallbacks)
		{
			return EmptyDisposable.Instance;
		}
		return _changeToken.RegisterChangeCallback(callback, state);
	}
}
