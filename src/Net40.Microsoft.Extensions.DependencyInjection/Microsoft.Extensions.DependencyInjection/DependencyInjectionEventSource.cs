using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection.ServiceLookup;

namespace Microsoft.Extensions.DependencyInjection;

internal sealed class DependencyInjectionEventSource
{
	private class NodeCountingVisitor : ExpressionVisitor
	{
		public int NodeCount { get; private set; }

		public override Expression Visit(Expression e)
		{
			base.Visit(e);
			NodeCount++;
			return e;
		}
	}

	public static readonly DependencyInjectionEventSource Log = new DependencyInjectionEventSource();

	private int MaxChunkSize = 10240;

	private DependencyInjectionEventSource()
	{
	}

	private void CallSiteBuilt(string serviceType, string callSite, int chunkIndex, int chunkCount)
	{
	}

	public void ServiceResolved(string serviceType)
	{
	}

	public void ExpressionTreeGenerated(string serviceType, int nodeCount)
	{
	}

	public void DynamicMethodBuilt(string serviceType, int methodSize)
	{
	}

	public void ServiceResolved(Type serviceType)
	{
		ServiceResolved(serviceType.ToString());
	}

	public void CallSiteBuilt(Type serviceType, ServiceCallSite callSite)
	{
		string format = CallSiteJsonFormatter.Instance.Format(callSite);
		int chunkCount = format.Length / MaxChunkSize + ((format.Length % MaxChunkSize > 0) ? 1 : 0);
		for (int i = 0; i < chunkCount; i++)
		{
			CallSiteBuilt(serviceType.ToString(), format.Substring(i * MaxChunkSize, Math.Min(MaxChunkSize, format.Length - i * MaxChunkSize)), i, chunkCount);
		}
	}

	public void ExpressionTreeGenerated(Type serviceType, Expression expression)
	{
		NodeCountingVisitor visitor = new NodeCountingVisitor();
		visitor.Visit(expression);
		ExpressionTreeGenerated(serviceType.ToString(), visitor.NodeCount);
	}

	public void DynamicMethodBuilt(Type serviceType, int methodSize)
	{
		DynamicMethodBuilt(serviceType.ToString(), methodSize);
	}
}
