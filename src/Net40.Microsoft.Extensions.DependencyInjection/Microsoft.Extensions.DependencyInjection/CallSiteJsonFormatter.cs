using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection.ServiceLookup;

namespace Microsoft.Extensions.DependencyInjection;

internal class CallSiteJsonFormatter : CallSiteVisitor<CallSiteJsonFormatter.CallSiteFormatterContext, object>
{
	internal struct CallSiteFormatterContext
	{
		private readonly HashSet<ServiceCallSite> _processedCallSites;

		private bool _firstItem;

		public int Offset { get; }

		public StringBuilder Builder { get; }

		public CallSiteFormatterContext(StringBuilder builder, int offset, HashSet<ServiceCallSite> processedCallSites)
		{
			Builder = builder;
			Offset = offset;
			_processedCallSites = processedCallSites;
			_firstItem = true;
		}

		public bool ShouldFormat(ServiceCallSite serviceCallSite)
		{
			return _processedCallSites.Add(serviceCallSite);
		}

		public CallSiteFormatterContext IncrementOffset()
		{
			CallSiteFormatterContext result = new CallSiteFormatterContext(Builder, Offset + 4, _processedCallSites);
			result._firstItem = true;
			return result;
		}

		public CallSiteFormatterContext StartObject()
		{
			Builder.Append("{");
			return IncrementOffset();
		}

		public void EndObject()
		{
			Builder.Append("}");
		}

		public void StartProperty(string name)
		{
			if (!_firstItem)
			{
				Builder.Append(",");
			}
			else
			{
				_firstItem = false;
			}
			Builder.AppendFormat("\"{0}\":", name);
		}

		public void StartArrayItem()
		{
			if (!_firstItem)
			{
				Builder.Append(",");
			}
			else
			{
				_firstItem = false;
			}
		}

		public void WriteProperty(string name, object value)
		{
			StartProperty(name);
			if (value != null)
			{
				Builder.AppendFormat(" \"{0}\"", value);
			}
			else
			{
				Builder.AppendFormat("null");
			}
		}

		public CallSiteFormatterContext StartArray()
		{
			Builder.Append("[");
			return IncrementOffset();
		}

		public void EndArray()
		{
			Builder.Append("]");
		}
	}

	internal static CallSiteJsonFormatter Instance = new CallSiteJsonFormatter();

	private CallSiteJsonFormatter()
	{
	}

	public string Format(ServiceCallSite callSite)
	{
		StringBuilder stringBuilder = new StringBuilder();
		CallSiteFormatterContext context = new CallSiteFormatterContext(stringBuilder, 0, new HashSet<ServiceCallSite>());
		VisitCallSite(callSite, context);
		return stringBuilder.ToString();
	}

	protected override object VisitConstructor(ConstructorCallSite constructorCallSite, CallSiteFormatterContext argument)
	{
		argument.WriteProperty("implementationType", constructorCallSite.ImplementationType);
		if (constructorCallSite.ParameterCallSites.Length != 0)
		{
			argument.StartProperty("arguments");
			CallSiteFormatterContext childContext = argument.StartArray();
			ServiceCallSite[] parameterCallSites = constructorCallSite.ParameterCallSites;
			foreach (ServiceCallSite parameter in parameterCallSites)
			{
				childContext.StartArrayItem();
				VisitCallSite(parameter, childContext);
			}
			argument.EndArray();
		}
		return null;
	}

	protected override object VisitCallSiteMain(ServiceCallSite callSite, CallSiteFormatterContext argument)
	{
		if (argument.ShouldFormat(callSite))
		{
			CallSiteFormatterContext childContext = argument.StartObject();
			childContext.WriteProperty("serviceType", callSite.ServiceType);
			childContext.WriteProperty("kind", callSite.Kind);
			childContext.WriteProperty("cache", callSite.Cache.Location);
			base.VisitCallSiteMain(callSite, childContext);
			argument.EndObject();
		}
		else
		{
			argument.StartObject().WriteProperty("ref", callSite.ServiceType);
			argument.EndObject();
		}
		return null;
	}

	protected override object VisitConstant(ConstantCallSite constantCallSite, CallSiteFormatterContext argument)
	{
		argument.WriteProperty("value", constantCallSite.DefaultValue ?? "");
		return null;
	}

	protected override object VisitServiceProvider(ServiceProviderCallSite serviceProviderCallSite, CallSiteFormatterContext argument)
	{
		return null;
	}

	protected override object VisitServiceScopeFactory(ServiceScopeFactoryCallSite serviceScopeFactoryCallSite, CallSiteFormatterContext argument)
	{
		return null;
	}

	protected override object VisitIEnumerable(IEnumerableCallSite enumerableCallSite, CallSiteFormatterContext argument)
	{
		argument.WriteProperty("itemType", enumerableCallSite.ItemType);
		argument.WriteProperty("size", enumerableCallSite.ServiceCallSites.Length);
		if (enumerableCallSite.ServiceCallSites.Length != 0)
		{
			argument.StartProperty("items");
			CallSiteFormatterContext childContext = argument.StartArray();
			ServiceCallSite[] serviceCallSites = enumerableCallSite.ServiceCallSites;
			foreach (ServiceCallSite item in serviceCallSites)
			{
				childContext.StartArrayItem();
				VisitCallSite(item, childContext);
			}
			argument.EndArray();
		}
		return null;
	}

	protected override object VisitFactory(FactoryCallSite factoryCallSite, CallSiteFormatterContext argument)
	{
		argument.WriteProperty("method", factoryCallSite.Factory.Method);
		return null;
	}
}
