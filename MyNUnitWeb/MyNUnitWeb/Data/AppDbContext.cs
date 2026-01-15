// <copyright file="AppDbContext.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Data;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// The database context.
/// </summary>
/// <param name="options">Options.</param>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets uploaded assemblies.
    /// </summary>
    public DbSet<UploadedAssembly> UploadedAssemblies => this.Set<UploadedAssembly>();

    /// <summary>
    /// Gets info about tests.
    /// </summary>
    public DbSet<TestInfo> TestInfos => Set<TestInfo>();

    /// <summary>
    /// Gets test runs.
    /// </summary>
    public DbSet<TestRun> TestRuns => Set<TestRun>();
}