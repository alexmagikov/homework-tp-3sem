// <copyright file="MultiThreadedLazy.cs" company="AlexanderKuchin">
// Copyright (c) AlexanderKuchin. All rights reserved.
// </copyright>

namespace Lazy;

/// <summary>
/// Lazy with multithreading support.
/// </summary>
/// <param name="supplier">Supplier.</param>
/// <typeparam name="T">Type of value.</typeparam>
public class MultiThreadedLazy<T>(Func<T> supplier) : ILazy<T>
{
    private readonly object lockObject = new();

    private volatile bool isInitialized;

    private Func<T>? supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));

    private T? value;

    /// <summary>
    /// Get method.
    /// </summary>
    /// <returns>Type value.</returns>
    public T? Get()
    {
        if (!this.isInitialized)
        {
            lock (this.lockObject)
            {
                if (!this.isInitialized)
                {
                    this.value = this.supplier();
                    this.isInitialized = true;
                    this.supplier = null;
                }
            }
        }

        return this.value;
    }
}