// <copyright file="Program.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Data;
using MyNUnitWeb.Services;

SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<UploadedAssemblyDbContext>(options =>
    options.UseSqlite("Data Source=uploaded-Assemblies.db"));

builder.Services.AddScoped<UploadService>();

builder.Services.AddScoped<ClearService>();

builder.Services.AddScoped<RunTestsService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();