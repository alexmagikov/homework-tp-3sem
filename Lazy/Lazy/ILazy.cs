// <copyright file="ILazy.cs" company="AlexanderKuchin">
// Copyright (c) AlexanderKuchin. All rights reserved.
// </copyright>

namespace Lazy;

/// <summary>
/// Lazy computing interface.
/// </summary>
/// <typeparam name="T">The type of the computed value.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Gets the lazily computed value.
    /// </summary>
    /// <returns>The computed value of type <typeparamref name="T"/>.</returns>
    T? Get();
}