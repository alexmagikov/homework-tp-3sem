// <copyright file="ChatMessenger.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace NetworkChat;

using System.Net.Sockets;

public class ChatMessenger(TextReader input, TextWriter output)
{
    public async Task RunAsync(NetworkStream stream, CancellationToken cts)
    {
        using var streamReader = new StreamReader(stream, leaveOpen: true);
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);
        streamWriter.AutoFlush = true;

        var read = ReadFromNetworkAsync(streamReader, cts);
        var write = WriteToNetworkAsync(streamWriter, cts);
        await Task.WhenAny(read, write);
    }

    private async Task WriteToNetworkAsync(StreamWriter writer, CancellationToken cts)
    {
        await output.WriteAsync("Local: ");
        while (!cts.IsCancellationRequested)
        {
            var line = await Task.Run(input.ReadLine, cts);

            if (line is null or "exit")
            {
                break;
            }

            await writer.WriteLineAsync(line.AsMemory(), cts);
            await output.WriteAsync("Local: ");
        }
    }

    private async Task ReadFromNetworkAsync(StreamReader reader, CancellationToken cts)
    {
        try
        {
            while (!cts.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(cts);
                if (line == null)
                {
                    break;
                }

                await output.WriteLineAsync($"Remote: {line}");
                await output.WriteAsync("Local: ");
            }
        }
        catch (IOException)
        {
            // ignored
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
    }
}