// <copyright file="ClearService.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Services;

using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Data;

public class ClearService(AppDbContext dbContext)
{
    public async Task ClearAsync()
    {
        var pathUploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        foreach (var file in Directory.GetFiles(pathUploads))
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        var rows = await dbContext.UploadedAssemblies.ToListAsync();
        dbContext.UploadedAssemblies.RemoveRange(rows);
        await dbContext.SaveChangesAsync();
    }
}