// <copyright file="UploadService.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Services;

using MyNUnitWeb.Data;

public class UploadService(UploadedAssemblyDbContext dbContext)
{
    public async Task UploadAsync(List<IFormFile> files)
    {
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(uploadDir);

        foreach (var file in files)
        {
            var filePath = Path.GetFileName(file.FileName);
            if (!filePath.EndsWith(".dll"))
            {
                continue;
            }

            var storedPath = $"{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}_{filePath}";
            storedPath = Path.Combine(uploadDir, storedPath);

            await using var stream = new FileStream(storedPath, FileMode.Create);
            await file.CopyToAsync(stream);

            dbContext.Add(new UploadedAssembly
            {
                Name = file.FileName,
                Path = storedPath,
            });
        }

        await dbContext.SaveChangesAsync();
    }
}