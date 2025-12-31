// <copyright file="IndexModel.cshtml.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyNUnit;
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
    RunTestsService runTestsService) : PageModel
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
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets current Tests.
    /// </summary>
    public List<TestResult>? CurrentTests { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether edger
    /// </summary>
    public bool IsTestsRun { get; set; } = false;

    /// <summary>
    /// Upload dlls.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostUploadAsync()
    {
        if (this.Files.Count == 0)
        {
            this.Error = "Нужно выбрать файлы";
            return this.RedirectToPage();
        }

        await uploadService.UploadAsync(this.Files);

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
        try
        {
            this.CurrentTests = await runTestsService.RunAsync();
            IsTestsRun = true;
        }
        catch (Exception ex)
        {
            this.Error = ex.Message;
        }

        return this.Page();
    }
}