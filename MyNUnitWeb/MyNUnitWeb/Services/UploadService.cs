// <copyright file="UploadService.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Services;

using MyNUnitWeb.Data;

/// <summary>
/// Upload service.
/// </summary>
/// <param name="dbContext">Database context.</param>
public class UploadService(AppDbContext dbContext)
{
    /// <summary>
    /// Upload libraries.
    /// </summary>
    /// <param name="files">Files.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public async Task<int> UploadAsync(List<IFormFile> files)
    {
        var uploadId = Guid.NewGuid().ToString();
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads",  uploadId);
        Directory.CreateDirectory(uploadDir);

        foreach (var file in files)
        {
            var filePath = Path.GetFileName(file.FileName);
            if (!filePath.EndsWith(".dll"))
            {
                continue;
            }

            var storedPath = Path.Combine(uploadDir, filePath);

            await using var stream = new FileStream(storedPath, FileMode.Create);
            await file.CopyToAsync(stream);

            dbContext.Add(new UploadedAssembly
            {
                Name = file.FileName,
                Path = storedPath,
            });
        }

        var testRun = new TestRun
        {
            DirectoryName = uploadId,
        };

        dbContext.Add(testRun);

        await dbContext.SaveChangesAsync();

        return testRun.Id;
    }
}