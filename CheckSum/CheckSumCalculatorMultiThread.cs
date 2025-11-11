// <copyright file="CheckSumCalculatorMultiThread.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Security.Cryptography;
using System.Text;

namespace CheckSum;

/// <summary>
/// Calculate check sum of directory in one thread.
/// </summary>
public class CheckSumCalculatorMultiThreadAsync
{
    public async Task<byte[]> CalculateCheckSum(string path)
    {
        var attributes = File.GetAttributes(path);
        if (!attributes.HasFlag(FileAttributes.Directory))
        {
            throw new ArgumentException("Not directory");
        }

        var dirInfo = new DirectoryInfo(path);
        return await this.CalculateCheckSumDirectory(dirInfo, CancellationToken.None);
    }

    public async Task<byte[]?> CalculateCheckSumFile(FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        var nameBytes = Encoding.UTF8.GetBytes(fileInfo.Name);

        using var md5 = MD5.Create();
        const int bufferSize = 8192;
        var buffer = new byte[bufferSize];
        await using var combinedStream = new MemoryStream();

        await combinedStream.WriteAsync(nameBytes, cancellationToken);

        await using var fileStream = File.OpenRead(fileInfo.FullName);

        md5.ComputeHashAsync(combinedStream, cancellationToken);

        int bytesRead;
        while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            await combinedStream.WriteAsync(buffer, nameBytes.Length + bytesRead, bytesRead, cancellationToken);
            await md5.ComputeHashAsync(combinedStream, cancellationToken);
        }

        return md5.Hash;
    }

    public async Task<byte[]> CalculateCheckSumDirectory(DirectoryInfo dirInfo, CancellationToken cancellationToken = default)
    {
        var nameBytes = Encoding.UTF8.GetBytes(dirInfo.Name);

        var children = dirInfo.GetFileSystemInfos()
            .OrderBy(f => f.Name)
            .ToList();

        using var md5 = MD5.Create();
        using var stream = new MemoryStream();
        stream.Write(nameBytes, 0, nameBytes.Length);
        md5.ComputeHashAsync(stream, cancellationToken);
        System.ParallelFor(
            );
            byte[]? childHash;
            switch (child)
            {
                case DirectoryInfo subDir:
                    childHash = await this.CalculateCheckSumDirectory(subDir, cancellationToken);
                    break;
                case FileInfo file:
                    childHash = await this.CalculateCheckSumFile(file, cancellationToken);
                    break;
                default:
                    continue;
            }

            stream.Write(childHash, 0, childHash.Length);
        }

        stream.Position = 0;
        return await md5.ComputeHashAsync(stream, cancellationToken);
    }
}