// <copyright file="Server.cs" company="AlexanderKuchin">
// Copyright (c) AlexanderKuchin. All rights reserved.
// </copyright>

using System.Text;

namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;

public class Server(int port)
{
    public async Task StartAsync()
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Listening on port {port}");

        while (true)
        {
            var socket = await listener.AcceptSocketAsync();

            Task.Run(
                async () =>
            {
                await using var stream = new NetworkStream(socket);
                using var reader = new StreamReader(stream);
                var data = await reader.ReadLineAsync();
                await using var writer = new StreamWriter(stream);

                try
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), data.Split(' ')[1]);

                    switch (data[0])
                    {
                        case '1':
                            await writer.WriteLineAsync(this.ListRequestHandler(path));
                            await writer.FlushAsync();
                            break;
                        case '2':
                            var (size, fileBytes) = await GetRequestHandler(path);
                            Console.WriteLine(size);
                            var sizeBytes = BitConverter.GetBytes(size);

                            await stream.WriteAsync(sizeBytes);
                            await stream.WriteAsync(fileBytes);
                            await stream.FlushAsync();
                            break;
                    }
                }
                catch(Exception exception)
                {
                    switch (data[0])
                    {
                        case '1':
                            await writer.WriteLineAsync($"-1 {exception.Message}");
                            await writer.FlushAsync();
                            break;
                        case '2':
                            await stream.WriteAsync(BitConverter.GetBytes((long)-1));
                            await stream.WriteAsync(Encoding.UTF8.GetBytes(exception.Message));
                            await writer.FlushAsync();
                            break;
                    }
                }

                socket.Close();
            });
        }
    }

    private string ListRequestHandler(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new ArgumentException("Invalid path");
        }

        var directories = Directory.GetDirectories(path);

        var files = Directory.GetFiles(path);

        var result = new StringBuilder();

        foreach (var dir in directories)
            result.Append($"{Path.GetFileName(dir)} true ");

        foreach (var file in files)
            result.Append($"{Path.GetFileName(file)} false ");

        return $"{directories.Length + files.Length} {result}";
    }

    private async Task<(long, byte[])> GetRequestHandler(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException("Invalid path");
        }

        var fileBytes = await File.ReadAllBytesAsync(path);
        long size = fileBytes.Length;

        return (size, fileBytes);
    }
}