using System;

namespace Microsoft.Extensions.Options;

public class PostConfigureOptions<TOptions> : IPostConfigureOptions<TOptions> where TOptions : class
{
	public string Name { get; }

	public Action<TOptions> Action { get; }

	public PostConfigureOptions(string name, Action<TOptions> action)
	{
		Name = name;
		Action = action;
	}

	public virtual void PostConfigure(string name, TOptions options)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		if (Name == null || name == Name)
		{
			Action?.Invoke(options);
		}
	}
}
public class PostConfigureOptions<TOptions, TDep> : IPostConfigureOptions<TOptions> where TOptions : class where TDep : class
{
	public string Name { get; }

	public Action<TOptions, TDep> Action { get; }

	public TDep Dependency { get; }

	public PostConfigureOptions(string name, TDep dependency, Action<TOptions, TDep> action)
	{
		Name = name;
		Action = action;
		Dependency = dependency;
	}

	public virtual void PostConfigure(string name, TOptions options)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		if (Name == null || name == Name)
		{
			Action?.Invoke(options, Dependency);
		}
	}

	public void PostConfigure(TOptions options)
	{
		PostConfigure(Options.DefaultName, options);
	}
}
public class PostConfigureOptions<TOptions, TDep1, TDep2> : IPostConfigureOptions<TOptions> where TOptions : class where TDep1 : class where TDep2 : class
{
	public string Name { get; }

	public Action<TOptions, TDep1, TDep2> Action { get; }

	public TDep1 Dependency1 { get; }

	public TDep2 Dependency2 { get; }

	public PostConfigureOptions(string name, TDep1 dependency, TDep2 dependency2, Action<TOptions, TDep1, TDep2> action)
	{
		Name = name;
		Action = action;
		Dependency1 = dependency;
		Dependency2 = dependency2;
	}

	public virtual void PostConfigure(string name, TOptions options)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		if (Name == null || name == Name)
		{
			Action?.Invoke(options, Dependency1, Dependency2);
		}
	}

	public void PostConfigure(TOptions options)
	{
		PostConfigure(Options.DefaultName, options);
	}
}
public class PostConfigureOptions<TOptions, TDep1, TDep2, TDep3> : IPostConfigureOptions<TOptions> where TOptions : class where TDep1 : class where TDep2 : class where TDep3 : class
{
	public string Name { get; }

	public Action<TOptions, TDep1, TDep2, TDep3> Action { get; }

	public TDep1 Dependency1 { get; }

	public TDep2 Dependency2 { get; }

	public TDep3 Dependency3 { get; }

	public PostConfigureOptions(string name, TDep1 dependency, TDep2 dependency2, TDep3 dependency3, Action<TOptions, TDep1, TDep2, TDep3> action)
	{
		Name = name;
		Action = action;
		Dependency1 = dependency;
		Dependency2 = dependency2;
		Dependency3 = dependency3;
	}

	public virtual void PostConfigure(string name, TOptions options)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		if (Name == null || name == Name)
		{
			Action?.Invoke(options, Dependency1, Dependency2, Dependency3);
		}
	}

	public void PostConfigure(TOptions options)
	{
		PostConfigure(Options.DefaultName, options);
	}
}
public class PostConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4> : IPostConfigureOptions<TOptions> where TOptions : class where TDep1 : class where TDep2 : class where TDep3 : class where TDep4 : class
{
	public string Name { get; }

	public Action<TOptions, TDep1, TDep2, TDep3, TDep4> Action { get; }

	public TDep1 Dependency1 { get; }

	public TDep2 Dependency2 { get; }

	public TDep3 Dependency3 { get; }

	public TDep4 Dependency4 { get; }

	public PostConfigureOptions(string name, TDep1 dependency1, TDep2 dependency2, TDep3 dependency3, TDep4 dependency4, Action<TOptions, TDep1, TDep2, TDep3, TDep4> action)
	{
		Name = name;
		Action = action;
		Dependency1 = dependency1;
		Dependency2 = dependency2;
		Dependency3 = dependency3;
		Dependency4 = dependency4;
	}

	public virtual void PostConfigure(string name, TOptions options)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		if (Name == null || name == Name)
		{
			Action?.Invoke(options, Dependency1, Dependency2, Dependency3, Dependency4);
		}
	}

	public void PostConfigure(TOptions options)
	{
		PostConfigure(Options.DefaultName, options);
	}
}
public class PostConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> : IPostConfigureOptions<TOptions> where TOptions : class where TDep1 : class where TDep2 : class where TDep3 : class where TDep4 : class where TDep5 : class
{
	public string Name { get; }

	public Action<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> Action { get; }

	public TDep1 Dependency1 { get; }

	public TDep2 Dependency2 { get; }

	public TDep3 Dependency3 { get; }

	public TDep4 Dependency4 { get; }

	public TDep5 Dependency5 { get; }

	public PostConfigureOptions(string name, TDep1 dependency1, TDep2 dependency2, TDep3 dependency3, TDep4 dependency4, TDep5 dependency5, Action<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> action)
	{
		Name = name;
		Action = action;
		Dependency1 = dependency1;
		Dependency2 = dependency2;
		Dependency3 = dependency3;
		Dependency4 = dependency4;
		Dependency5 = dependency5;
	}

	public virtual void PostConfigure(string name, TOptions options)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		if (Name == null || name == Name)
		{
			Action?.Invoke(options, Dependency1, Dependency2, Dependency3, Dependency4, Dependency5);
		}
	}

	public void PostConfigure(TOptions options)
	{
		PostConfigure(Options.DefaultName, options);
	}
}
