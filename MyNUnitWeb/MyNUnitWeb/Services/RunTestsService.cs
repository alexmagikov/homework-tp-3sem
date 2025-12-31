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
public class RunTestsService(AppDbContext dbContext)
{
    /// <summary>
    /// Run tests async.
    /// </summary>
    /// <exception cref="Exception">Exception to running or uploading.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<List<TestInfo>> RunAsync(int testRunId)
    {
        var testRun = await dbContext.TestRuns.FindAsync(testRunId);
        if (testRun == null)
        {
            throw new Exception("Test run not found");
        }

        var pathUploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", testRun.DirectoryName);
        var testResults = await MyNUnit.RunAsync(pathUploads);
        List<TestInfo> testInfos = new();

        foreach (var testResult in testResults)
        {
            var testInfo = new TestInfo
            {
                TestId = testRunId,
                AssemblyName = testResult.AssemblyName,
                TestName = testResult.TestName,
                IsPassed = testResult.IsPassed,
                WorkTime = testResult.WorkTime,
                ErrorMessage = testResult.ErrorMessage,
                IgnoreReason = testResult.IgnoreReason,
            };

            testInfos.Add(testInfo);
            dbContext.TestInfos.Add(testInfo);
        }

        testRun.TestInfos = testInfos;

        await dbContext.SaveChangesAsync();

        return testInfos;
    }
}