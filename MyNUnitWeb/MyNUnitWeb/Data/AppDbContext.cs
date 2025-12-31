// <copyright file="AppDbContext.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Data;

using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UploadedAssembly> UploadedAssemblies => this.Set<UploadedAssembly>();

    public DbSet<TestInfo> TestInfos => Set<TestInfo>();

    public DbSet<TestRun> TestRuns => Set<TestRun>();
}