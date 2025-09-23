// <copyright file="ILazy.cs" company="AlexanderKuchin">
// Copyright (c) AlexanderKuchin. All rights reserved.
// </copyright>

namespace Lazy;

/// <summary>
/// Lazy calculation with 1 thread.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingleThreadedLazy<T>(Func<T> supplier) : ILazy<T>
{
    private bool isInitialized;

    private Func<T>? supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
    
    private T? value;

    /// <summary>
    /// Get method.
    /// </summary>
    /// <returns>T type.</returns>
    /// <exception cref="NullReferenceException">Null exception.</exception>
    public T? Get()
    {
        if (!this.isInitialized)
        {
            if (this.supplier == null)
            {
                throw new NullReferenceException("Supplier is null");
            }
            this.value = this.supplier();
            this.isInitialized = true;
            this.supplier = null;
        }
        
        return this.value;
    }
}