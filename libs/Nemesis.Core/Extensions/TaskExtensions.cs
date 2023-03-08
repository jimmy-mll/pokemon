using System.Diagnostics;

namespace Nemesis.Core.Extensions;

/// <summary>
///     Extension methods for <see cref="Task" /> and <see cref="ValueTask" />.
/// </summary>
public static class TaskExtensions
{
	/// <summary>
	///     Safely execute the <paramref name="task" /> without awaiting it.
	/// </summary>
	/// <param name="task">The task to execute.</param>
	public static async void FireAndForget(this Task task)
	{
		try
		{
			await task.ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Debug.WriteLine(e, "An exception occurred in a FireAndForget task.");
		}
	}

	/// <summary>
	///     Safely execute the <paramref name="task" /> without awaiting it.
	/// </summary>
	/// <param name="task">The task to execute.</param>
	/// <typeparam name="T">The type of the task's result.</typeparam>
	public static async void FireAndForget<T>(this Task<T> task)
	{
		try
		{
			await task.ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Debug.WriteLine(e, "An exception occurred in a FireAndForget task.");
		}
	}

	/// <summary>
	///     Safely execute the <paramref name="valueTask" /> without awaiting it.
	/// </summary>
	/// <param name="valueTask">The valueTask to execute.</param>
	public static async void FireAndForget(this ValueTask valueTask)
	{
		try
		{
			await valueTask.ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Debug.WriteLine(e, "An exception occurred in a FireAndForget valueTask.");
		}
	}

	/// <summary>
	///     Safely execute the <paramref name="valueTask" /> without awaiting it.
	/// </summary>
	/// <param name="valueTask">The task to execute.</param>
	/// <typeparam name="T">The type of the valueTask's result.</typeparam>
	public static async void FireAndForget<T>(this ValueTask<T> valueTask)
	{
		try
		{
			await valueTask.ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Debug.WriteLine(e, "An exception occurred in a FireAndForget valueTask.");
		}
	}
}