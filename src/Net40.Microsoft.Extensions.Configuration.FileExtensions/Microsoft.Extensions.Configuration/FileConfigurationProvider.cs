using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration;

public abstract class FileConfigurationProvider : ConfigurationProvider, IDisposable
{
	private readonly IDisposable _changeTokenRegistration;

	public FileConfigurationSource Source { get; }

	public FileConfigurationProvider(FileConfigurationSource source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		Source = source;
		if (Source.ReloadOnChange && Source.FileProvider != null)
		{
			_changeTokenRegistration = ChangeToken.OnChange(() => Source.FileProvider.Watch(Source.Path), delegate
			{
				Thread.Sleep(Source.ReloadDelay);
				Load(reload: true);
			});
		}
	}

	public override string ToString()
	{
		return GetType().Name + " for '" + Source.Path + "' (" + (Source.Optional ? "Optional" : "Required") + ")";
	}

	private void Load(bool reload)
	{
		IFileInfo file = Source.FileProvider?.GetFileInfo(Source.Path);
		if (file == null || !file.Exists)
		{
			if (Source.Optional || reload)
			{
				base.Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			}
			else
			{
				StringBuilder error = new StringBuilder("The configuration file '" + Source.Path + "' was not found and is not optional.");
				if (!string.IsNullOrEmpty(file?.PhysicalPath))
				{
					error.Append(" The physical path is '" + file.PhysicalPath + "'.");
				}
				HandleException(ExceptionDispatchInfo.Capture(new FileNotFoundException(error.ToString())));
			}
		}
		else
		{
			if (reload)
			{
				base.Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			}
			using Stream stream = file.CreateReadStream();
			try
			{
				Load(stream);
			}
			catch (Exception e)
			{
				HandleException(ExceptionDispatchInfo.Capture(e));
			}
		}
		OnReload();
	}

	public override void Load()
	{
		Load(reload: false);
	}

	public abstract void Load(Stream stream);

	private void HandleException(ExceptionDispatchInfo info)
	{
		bool ignoreException = false;
		if (Source.OnLoadException != null)
		{
			FileLoadExceptionContext exceptionContext = new FileLoadExceptionContext
			{
				Provider = this,
				Exception = info.SourceException
			};
			Source.OnLoadException(exceptionContext);
			ignoreException = exceptionContext.Ignore;
		}
		if (!ignoreException)
		{
			info.Throw();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		_changeTokenRegistration?.Dispose();
	}
}
