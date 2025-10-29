// <copyright file="Client.cs" company="AlexanderKuchin">
// Copyright (c) AlexanderKuchin. All rights reserved.
// </copyright>

using System.Text;

namespace SimpleFTP;

using System.Net.Sockets;

/// <summary>
/// Part of client with List and Get requests.
/// </summary>
public class Client : IDisposable
{
    private readonly TcpClient client;
    private readonly NetworkStream stream;
    private readonly StreamWriter writer;
    private readonly StreamReader reader;

    public Client(int port, string host)
    {
        this.client = new TcpClient(host, port);
        Console.WriteLine($"{DateTime.Now:HH:mm:ss}>>>Client>>>Connected to: {host}:{port}");
        this.stream = this.client.GetStream();
        this.writer = new StreamWriter(this.stream);
        this.reader = new StreamReader(this.stream);
    }

    /// <summary>
    /// Take request to get list of files and directories by relative path.
    /// </summary>
    /// <param name="path">PathName.</param>
    /// <returns>Error code, size of directory, data.</returns>
    public async Task<(string? ErrorCode, int Size, string? Data)> SendListRequestAsync(string path)
    {
        try
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}>>>Client>>>Sending request");
            await this.writer.WriteLineAsync($"1 {path}");
            await this.writer.FlushAsync();

            var response = await this.reader.ReadLineAsync() ?? throw new ArgumentNullException();

            Console.WriteLine($"{DateTime.Now:HH:mm:ss)}>>>Client>>>Response: {response}");

            var responseArray = response.Split(' ');
            var size = int.Parse(responseArray[0]);

            if (size == -1)
            {
                return ("Invalid path", -1, string.Empty);
            }

            var data = string.Join(" ", responseArray.Skip(1));

            return (string.Empty, size, data);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}>>>Client>>>Client error: {exception.Message}");
            return (exception.Message, -1, string.Empty);
        }
    }

    /// <summary>
    /// Take request to get FileData by relative path.
    /// </summary>
    /// <param name="path">PathName.</param>
    /// <returns>Error-code, fileBytes.</returns>
    public async Task<(string? ErrorCode, byte[]? FileBytes)> SendGetRequestAsync(string path)
    {
        try
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}>>>Client>>>Sending request");
            await this.writer.WriteLineAsync($"2 {path}");
            await this.writer.FlushAsync();

            var sizeBytes = new byte[8];
            await this.stream.ReadExactlyAsync(sizeBytes);
            var size = BitConverter.ToInt64(sizeBytes);
            if (size != -1)
            {
                var fileBytes = new byte[size];
                const int bufferSize = 8192;
                var readSize = 0;

                while (readSize < size)
                {
                    var remaining = (int)(size - readSize);
                    var chunkSize = Math.Min(bufferSize, remaining);

                    await this.stream.ReadExactlyAsync(fileBytes, readSize, chunkSize);

                    readSize += chunkSize;
                }

                Console.WriteLine($"{DateTime.Now:HH:mm:ss}>>>Client>>>Response: {size} {Encoding.UTF8.GetString(fileBytes)}");
                return (string.Empty, fileBytes);
            }
            else
            {
                var response = await this.reader.ReadLineAsync();

                Console.WriteLine($"{DateTime.Now:HH:mm:ss}>>>Client>>>Response: {size} {response}");
                return (response, null);
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}>>>Client>>>Client error: {exception.Message}");
            return (exception.Message, null);
        }
    }

    public void Dispose()
    {
        this.writer.Dispose();
        this.stream.Dispose();
        this.client.Dispose();
        this.reader.Dispose();
    }
}