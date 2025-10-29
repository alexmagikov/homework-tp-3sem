// <copyright file="Server.cs" company="AlexanderKuchin">
// Copyright (c) AlexanderKuchin. All rights reserved.
// </copyright>

namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Accept request to get list of files and directories by relative path.
/// </summary>
/// <param name="port">Port of connection.</param>
public class Server(int port) : IDisposable
{
    private readonly TcpListener listener = new(IPAddress.Any, port);
    private readonly CancellationTokenSource cts = new();

    /// <summary>
    /// Start listening of channel.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StartAsync()
    {
        try
        {
            this.listener.Start();
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}>>>Server>>>Listening on port {port}");

            while (!this.cts.IsCancellationRequested)
            {
                var socket = await this.listener.AcceptSocketAsync();

                _ = Task.Run(
                    async () =>
                    {
                        var stream = new NetworkStream(socket, true);
                        await using var safeStream = Stream.Synchronized(stream);

                        using var reader = new StreamReader(safeStream);
                        await using var writer = new StreamWriter(safeStream);
                        writer.AutoFlush = true;

                        Console.WriteLine($"{DateTime.Now:HH:mm:ss}>>>Server>>>Server is sending data");

                        while (!this.cts.IsCancellationRequested)
                        {
                            var data = await reader.ReadLineAsync();
                            if (data == null)
                            {
                                break;
                            }

                            try
                            {
                                var path = Path.Combine(Directory.GetCurrentDirectory(), data.Split(' ')[1]);
                                Console.WriteLine(path);
                                switch (data[0])
                                {
                                    case '1':
                                        await this.ListRequestHandler(path, writer);
                                        break;
                                    case '2':
                                        await this.GetRequestHandler(path, safeStream);
                                        break;
                                }
                            }
                            catch (Exception exception)
                            {
                                switch (data[0])
                                {
                                    case '1':
                                        await writer.WriteLineAsync($"-1 {exception.Message}");
                                        break;
                                    case '2':
                                        await safeStream.WriteAsync(BitConverter.GetBytes(-1L));
                                        await writer.WriteLineAsync(exception.Message);
                                        break;
                                }

                                await writer.FlushAsync();
                            }
                        }
                    });
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}>>>Server>>>Server error: {exception.Message}");
            this.Dispose();
        }
    }

    public void Dispose()
    {
        this.listener.Stop();
        this.listener.Dispose();
        this.cts.Cancel();
    }

    private async Task GetRequestHandler(string path, Stream stream)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException("Invalid path");
        }

        var fileInfo = new FileInfo(path);
        long size = fileInfo.Length;

        var sizeBytes = BitConverter.GetBytes(size);

        await stream.WriteAsync(sizeBytes);

        const int bufferSize = 8192;
        var buffer = new byte[bufferSize];

        await using var fileStream = File.OpenRead(path);
        int bytesRead;
        while ((bytesRead = await fileStream.ReadAsync(buffer)) > 0)
        {
            await stream.WriteAsync(buffer.AsMemory(0, bytesRead));
        }

        await stream.FlushAsync();

        Console.WriteLine($"{DateTime.Now:HH:mm:ss}>>>Server>>>File sent: {path}, size: {size} bytes");
    }

    private async Task ListRequestHandler(string path, StreamWriter writer)
    {
        if (!Directory.Exists(path))
        {
            throw new ArgumentException("Invalid path");
        }

        var directories = Directory.GetDirectories(path);

        var files = Directory.GetFiles(path);

        var result = new StringBuilder();

        foreach (var dir in directories)
        {
            result.Append($"{Path.GetFileName(dir)} true ");
        }

        foreach (var file in files)
        {
            result.Append($"{Path.GetFileName(file)} false ");
        }

        await writer.WriteLineAsync($"{directories.Length + files.Length} {result}");
        await writer.FlushAsync();

        Console.WriteLine($"{DateTime.Now:HH:mm:ss}>>>Server>>>List sent: {path}, items: {directories.Length + files.Length}");
    }
}