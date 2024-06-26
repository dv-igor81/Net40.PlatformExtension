using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class ILEmitResolverBuilderContext
{
	public ILGenerator Generator { get; set; }

	public List<object> Constants { get; set; }

	public List<Func<IServiceProvider, object>> Factories { get; set; }
}
