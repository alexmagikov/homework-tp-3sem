// <copyright file="IMyTask.cs" company="AlexanderKuchin">
// Copyright (c) AlexanderKuchin. All rights reserved.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Interface of task like the System.Threading.Tasks.
/// </summary>
/// <typeparam name="TResult">Result type.</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether task completion property.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Gets result of task.
    /// </summary>
    TResult Result { get; }

    /// <summary>
    /// Add new task to execute if general task is completed.
    /// </summary>
    /// <param name="continuation">Continuation func.</param>
    /// <typeparam name="TNewResult">Result of task.</typeparam>
    /// <returns>Result task.</returns>
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
}