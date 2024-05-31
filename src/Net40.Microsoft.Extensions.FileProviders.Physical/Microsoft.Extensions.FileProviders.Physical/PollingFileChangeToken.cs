#define DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.FileProviders.Physical;

public class PollingFileChangeToken : IPollingChangeToken, IChangeToken
{
	private readonly FileInfo _fileInfo;

	private DateTime _previousWriteTimeUtc;

	private DateTime _lastCheckedTimeUtc;

	private bool _hasChanged;

	private CancellationTokenSource _tokenSource;

	private CancellationChangeToken _changeToken;

	internal static TimeSpan PollingInterval { get; set; } = PhysicalFilesWatcher.DefaultPollingInterval;


	public bool ActiveChangeCallbacks { get; internal set; }

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

	public bool HasChanged
	{
		get
		{
			if (_hasChanged)
			{
				return _hasChanged;
			}
			DateTime currentTime = DateTime.UtcNow;
			if (currentTime - _lastCheckedTimeUtc < PollingInterval)
			{
				return _hasChanged;
			}
			DateTime lastWriteTimeUtc = GetLastWriteTimeUtc();
			if (_previousWriteTimeUtc != lastWriteTimeUtc)
			{
				_previousWriteTimeUtc = lastWriteTimeUtc;
				_hasChanged = true;
			}
			_lastCheckedTimeUtc = currentTime;
			return _hasChanged;
		}
	}

	public PollingFileChangeToken(FileInfo fileInfo)
	{
		_fileInfo = fileInfo;
		_previousWriteTimeUtc = GetLastWriteTimeUtc();
	}

	private DateTime GetLastWriteTimeUtc()
	{
		_fileInfo.Refresh();
		return _fileInfo.Exists ? _fileInfo.LastWriteTimeUtc : DateTime.MinValue;
	}

	public IDisposable RegisterChangeCallback(Action<object> callback, object state)
	{
		if (!ActiveChangeCallbacks)
		{
			return EmptyDisposable.Instance;
		}
		return _changeToken.RegisterChangeCallback(callback, state);
	}
}
