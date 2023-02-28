namespace Pokemon.Core.Threading;

public abstract class Singleton<T>
	where T : class, new()
{
	private static readonly Lazy<T> LazyInstance = new(() => new T(), LazyThreadSafetyMode.ExecutionAndPublication);

	public static T Instance => 
		LazyInstance.Value;
}