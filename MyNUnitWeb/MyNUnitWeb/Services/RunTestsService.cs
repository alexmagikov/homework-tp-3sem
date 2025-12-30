// <copyright file="RunTests.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;

namespace MyNUnitWeb.Services;

using MyNUnitWeb.Data;
using MyNUnit;

public class RunTestsService(UploadedAssemblyDbContext dbContext)
{
    public async Task RunAsync()
    {
        if (!(await dbContext.UploadedAssemblies.AnyAsync()))
        {
            throw new Exception("No uploaded assemblies found");
        }

        var pathUploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        var resultsStrings = await MyNUnit.RunAsync(pathUploads);


    }
}