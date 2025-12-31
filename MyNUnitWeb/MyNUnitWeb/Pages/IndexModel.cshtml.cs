// <copyright file="IndexModel.cshtml.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Data;
using MyNUnitWeb.Services;

/// <summary>
/// Index model code.
/// </summary>
/// <param name="clearService">ClearService.</param>
/// <param name="uploadService">UploadService.</param>
/// <param name="runTestsService">RunTestsService.</param>
public class IndexModel(
    ClearService clearService,
    UploadService uploadService,
    RunTestsService runTestsService,
    AppDbContext dbContext) : PageModel
{
    /// <summary>
    /// Gets or sets dlls.
    /// </summary>
    [BindProperty]
    public List<IFormFile> Files { get; set; } = [];

    /// <summary>
    /// Gets or sets current error.
    /// </summary>
    [TempData]
    public string? UploadError { get; set; }

    /// <summary>
    /// Gets or sets current error.
    /// </summary>
    [TempData]
    public string? RunTestsError { get; set; }

    /// <summary>
    /// Gets or sets current Tests.
    /// </summary>
    public List<TestInfo>? CurrentTests { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether edger.
    /// </summary>
    public bool IsTestsRun { get; set; } = false;

    [BindProperty]
    public int CurrentRunId { get; set; }

    public List<TestRun> History { get; set; } = [];

    public void OnGet()
    {
        this.History = dbContext.TestRuns
            .Include(x => x.TestInfos)
            .ToList();
    }

    /// <summary>
    /// Upload dlls.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostUploadAsync()
    {
        if (this.Files.Count == 0)
        {
            this.UploadError = "Нужно выбрать файлы";
            return this.RedirectToPage();
        }

        var runId = await uploadService.UploadAsync(this.Files);

        TempData["CurrentRunId"] = runId;

        return this.RedirectToPage();
    }

    /// <summary>
    /// Clear uploaded dlls.
    /// </summary>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostClearAsync()
    {
        await clearService.ClearAsync();
        return this.RedirectToPage();
    }

    /// <summary>
    /// Run tests async.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostRunTestsAsync()
    {
        if (TempData["CurrentRunId"] is int runId)
        {
            try
            {
                this.CurrentTests = await runTestsService.RunAsync(runId);
                TempData.Keep("CurrentRunId");
                IsTestsRun = true;
            }
            catch (Exception ex)
            {
                this.RunTestsError = ex.Message;
            }
        }
        else
        {
            this.RunTestsError = "Нужно загрузить тесты";
        }

        this.OnGet();

        return this.Page();
    }
}