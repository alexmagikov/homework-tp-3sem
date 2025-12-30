// <copyright file="Index.cshtml.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyNUnitWeb.Data;
using MyNUnitWeb.Services;

public class IndexModel(
    UploadedAssemblyDbContext uploadedAssemblyDbContext,
    ClearService clearService,
    UploadService uploadService) : PageModel
{
    [BindProperty]
    public List<IFormFile> Files { get; set; } = [];

    public List<string> StagedFiles { get; set; } = [];

    [TempData]
    public string? Error { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostUploadAsync()
    {
        if (Files.Count == 0)
        {
            this.Error = "Нужно выбрать файлы";
            return this.RedirectToPage();
        }

        await uploadService.UploadAsync(Files);

        return this.RedirectToPage();
    }

    public async Task<IActionResult> OnPostClearAsync()
    {
        await clearService.ClearAsync();
        return this.RedirectToPage();
    }

    public async Task<IActionResult> OnPostRunTestsAsync()
    {
        try
        {
            // todo
        }
        catch (Exception ex)
        {
            this.Error = ex.Message;
        }

        return this.RedirectToPage();
    }
}