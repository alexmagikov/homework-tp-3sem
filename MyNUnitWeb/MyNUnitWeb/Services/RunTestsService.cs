// <copyright file="RunTestsService.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Services;

using Microsoft.EntityFrameworkCore;
using MyNUnit;
using MyNUnitWeb.Data;

/// <summary>
/// Run tests service.
/// </summary>
/// <param name="dbContext">Database context.</param>
public class RunTestsService(UploadedAssemblyDbContext dbContext)
{
    /// <summary>
    /// Run tests async.
    /// </summary>
    /// <exception cref="Exception">Exception to running or uploading.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<List<TestResult>> RunAsync()
    {
        if (!(await dbContext.UploadedAssemblies.AnyAsync()))
        {
            throw new Exception("No uploaded assemblies found");
        }

        var pathUploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        var testResults = await MyNUnit.RunAsync(pathUploads);

        foreach (var testResult in testResults)
        {
            var testInfo = new TestInfo
            {
                AssemblyName = testResult.AssemblyName,
                TestName = testResult.TestName,
                IsPassed = testResult.IsPassed,
                WorkTime = testResult.WorkTime,
                ErrorMessage = testResult.ErrorMessage,
                IgnoreReason = testResult.IgnoreReason,
            };

            dbContext.Add(testInfo);
        }

        await dbContext.SaveChangesAsync();

        return testResults.ToList();
    }
}