using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Internal;
using Net40.Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class CallSiteChain
{
	private struct ChainItemInfo
	{
		public int Order { get; }

		public Type ImplementationType { get; }

		public ChainItemInfo(int order, Type implementationType)
		{
			Order = order;
			ImplementationType = implementationType;
		}
	}

	private readonly Dictionary<Type, ChainItemInfo> _callSiteChain;

	public CallSiteChain()
	{
		_callSiteChain = new Dictionary<Type, ChainItemInfo>();
	}

	public void CheckCircularDependency(Type serviceType)
	{
		if (_callSiteChain.ContainsKey(serviceType))
		{
			throw new InvalidOperationException(CreateCircularDependencyExceptionMessage(serviceType));
		}
	}

	public void Remove(Type serviceType)
	{
		_callSiteChain.Remove(serviceType);
	}

	public void Add(Type serviceType, Type implementationType = null)
	{
		_callSiteChain[serviceType] = new ChainItemInfo(_callSiteChain.Count, implementationType);
	}

	private string CreateCircularDependencyExceptionMessage(Type type)
	{
		StringBuilder messageBuilder = new StringBuilder();
		messageBuilder.AppendFormat(Resources.CircularDependencyException, TypeNameHelper.GetTypeDisplayName(type));
		messageBuilder.AppendLine();
		AppendResolutionPath(messageBuilder, type);
		return messageBuilder.ToString();
	}

	private void AppendResolutionPath(StringBuilder builder, Type currentlyResolving = null)
	{
		foreach (KeyValuePair<Type, ChainItemInfo> pair in _callSiteChain.OrderBy(p => p.Value.Order))
		{
			Type serviceType = pair.Key;
			Type implementationType = pair.Value.ImplementationType;
			if (implementationType == null || serviceType == implementationType)
			{
				builder.Append(TypeNameHelper.GetTypeDisplayName(serviceType));
			}
			else
			{
				builder.AppendFormat("{0}({1})", TypeNameHelper.GetTypeDisplayName(serviceType), TypeNameHelper.GetTypeDisplayName(implementationType));
			}
			builder.Append(" -> ");
		}
		builder.Append(TypeNameHelper.GetTypeDisplayName(currentlyResolving));
	}
}
