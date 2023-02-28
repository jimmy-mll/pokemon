using Pokemon.Core.Threading;

namespace Pokemon.Core.Network.Infrastructure;

public sealed class NullServiceProvider : Singleton<NullServiceProvider>, IServiceProvider
{
	public object? GetService(Type serviceType) =>
		null;
}