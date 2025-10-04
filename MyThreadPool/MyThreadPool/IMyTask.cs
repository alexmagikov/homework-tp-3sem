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
    public bool IsCompleted { get; }

    TResult? Result { get; }

    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
}