#define DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Extensions.FileProviders.Internal;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.FileProviders.Physical.Internal;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.FileProviders;

public class PhysicalFileProvider : IFileProvider, IDisposable
{
	private const string PollingEnvironmentKey = "DOTNET_USE_POLLING_FILE_WATCHER";

	private static readonly char[] _pathSeparators = new char[2]
	{
		Path.DirectorySeparatorChar,
		Path.AltDirectorySeparatorChar
	};

	private readonly ExclusionFilters _filters;

	private readonly Func<PhysicalFilesWatcher> _fileWatcherFactory;

	private PhysicalFilesWatcher _fileWatcher;

	private bool _fileWatcherInitialized;

	private object _fileWatcherLock = new object();

	private bool? _usePollingFileWatcher;

	private bool? _useActivePolling;

	public bool UsePollingFileWatcher
	{
		get
		{
			if (_fileWatcher != null)
			{
				throw new InvalidOperationException("Cannot modify UsePollingFileWatcher once file watcher has been initialized.");
			}
			if (!_usePollingFileWatcher.HasValue)
			{
				ReadPollingEnvironmentVariables();
			}
			return _usePollingFileWatcher.Value;
		}
		set
		{
			_usePollingFileWatcher = value;
		}
	}

	public bool UseActivePolling
	{
		get
		{
			if (!_useActivePolling.HasValue)
			{
				ReadPollingEnvironmentVariables();
			}
			return _useActivePolling.Value;
		}
		set
		{
			_useActivePolling = value;
		}
	}

	internal PhysicalFilesWatcher FileWatcher
	{
		get
		{
			return LazyInitializer.EnsureInitialized(ref _fileWatcher, ref _fileWatcherInitialized, ref _fileWatcherLock, _fileWatcherFactory);
		}
		set
		{
			Debug.Assert(!_fileWatcherInitialized);
			_fileWatcherInitialized = true;
			_fileWatcher = value;
		}
	}

	public string Root { get; }

	public PhysicalFileProvider(string root)
		: this(root, ExclusionFilters.Sensitive)
	{
	}

	public PhysicalFileProvider(string root, ExclusionFilters filters)
	{
		if (!Path.IsPathRooted(root))
		{
			throw new ArgumentException("The path must be absolute.", "root");
		}
		string fullRoot = Path.GetFullPath(root);
		Root = PathUtils.EnsureTrailingSlash(fullRoot);
		if (!Directory.Exists(Root))
		{
			throw new DirectoryNotFoundException(Root);
		}
		_filters = filters;
		_fileWatcherFactory = () => CreateFileWatcher();
	}

	internal PhysicalFilesWatcher CreateFileWatcher()
	{
		string root = PathUtils.EnsureTrailingSlash(Path.GetFullPath(Root));
		return new PhysicalFilesWatcher(root, new FileSystemWatcher(root), UsePollingFileWatcher, _filters)
		{
			UseActivePolling = UseActivePolling
		};
	}

	private void ReadPollingEnvironmentVariables()
	{
		string environmentValue = Environment.GetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER");
		bool pollForChanges = string.Equals(environmentValue, "1", StringComparison.Ordinal) || string.Equals(environmentValue, "true", StringComparison.OrdinalIgnoreCase);
		_usePollingFileWatcher = pollForChanges;
		_useActivePolling = pollForChanges;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		_fileWatcher?.Dispose();
	}

	~PhysicalFileProvider()
	{
		Dispose(disposing: false);
	}

	private string GetFullPath(string path)
	{
		if (PathUtils.PathNavigatesAboveRoot(path))
		{
			return null;
		}
		string fullPath;
		try
		{
			fullPath = Path.GetFullPath(Path.Combine(Root, path));
		}
		catch
		{
			return null;
		}
		if (!IsUnderneathRoot(fullPath))
		{
			return null;
		}
		return fullPath;
	}

	private bool IsUnderneathRoot(string fullPath)
	{
		return fullPath.StartsWith(Root, StringComparison.OrdinalIgnoreCase);
	}

	public IFileInfo GetFileInfo(string subpath)
	{
		if (string.IsNullOrEmpty(subpath) || PathUtils.HasInvalidPathChars(subpath))
		{
			return new NotFoundFileInfo(subpath);
		}
		subpath = subpath.TrimStart(_pathSeparators);
		if (Path.IsPathRooted(subpath))
		{
			return new NotFoundFileInfo(subpath);
		}
		string fullPath = GetFullPath(subpath);
		if (fullPath == null)
		{
			return new NotFoundFileInfo(subpath);
		}
		FileInfo fileInfo = new FileInfo(fullPath);
		if (FileSystemInfoHelper.IsExcluded(fileInfo, _filters))
		{
			return new NotFoundFileInfo(subpath);
		}
		return new PhysicalFileInfo(fileInfo);
	}

	public IDirectoryContents GetDirectoryContents(string subpath)
	{
		try
		{
			if (subpath == null || PathUtils.HasInvalidPathChars(subpath))
			{
				return NotFoundDirectoryContents.Singleton;
			}
			subpath = subpath.TrimStart(_pathSeparators);
			if (Path.IsPathRooted(subpath))
			{
				return NotFoundDirectoryContents.Singleton;
			}
			string fullPath = GetFullPath(subpath);
			if (fullPath == null || !Directory.Exists(fullPath))
			{
				return NotFoundDirectoryContents.Singleton;
			}
			return new PhysicalDirectoryContents(fullPath, _filters);
		}
		catch (DirectoryNotFoundException)
		{
		}
		catch (IOException)
		{
		}
		return NotFoundDirectoryContents.Singleton;
	}

	public IChangeToken Watch(string filter)
	{
		if (filter == null || PathUtils.HasInvalidFilterChars(filter))
		{
			return NullChangeToken.Singleton;
		}
		filter = filter.TrimStart(_pathSeparators);
		return FileWatcher.CreateFileChangeToken(filter);
	}
}
