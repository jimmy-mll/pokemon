using System.Diagnostics;

namespace Pokemon.Core.Extensions;

public static class TaskExtensions
{
	public static async void FireAndForget(this Task task)
	{
		try
		{
			await task.ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Debug.WriteLine(e, "TaskExtensions");
		}
	}

	public static async void FireAndForget<T>(this Task<T> task)
	{
		try
		{
			await task.ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Debug.WriteLine(e, "TaskExtensions");
		}
	}

	public static async void FireAndForget(this ValueTask task)
	{
		try
		{
			await task.ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Debug.WriteLine(e, "TaskExtensions");
		}
	}

	public static async void FireAndForget<T>(this ValueTask<T> task)
	{
		try
		{
			await task.ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Debug.WriteLine(e, "TaskExtensions");
		}
	}
}