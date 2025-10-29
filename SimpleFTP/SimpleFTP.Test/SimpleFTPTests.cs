// <copyright file="SimpleFTPTests.cs" company="AlexanderKuchin">
// Copyright (c) AlexanderKuchin. All rights reserved.
// </copyright>

namespace SimpleFTP.Test;

public class Tests
{
    private Server server;
    private Client client;
    private string pathFile;
    private string path;

    [OneTimeSetUp]
    public async Task SetupAsync()
    {
        Directory.CreateDirectory( Path.Combine(Directory.GetCurrentDirectory(), "test", "testDirectory"));
        this.path = Path.Combine("test");
        this.pathFile = Path.Combine("test", "test1.txt");

        await File.WriteAllTextAsync(this.pathFile, "textData");
        this.server = new Server(8080);
        _ = this.server.StartAsync();

        this.client = new Client(8080, "127.0.0.1");
    }

    [Test]
    public async Task ServerShouldReturnDataFromListRequest()
    {
        var (errorCode, size, data) = await this.client.SendListRequestAsync(this.path);
        Assert.Multiple(() =>
        {
            Assert.That(errorCode, Is.EqualTo(string.Empty));
            Assert.That(size, Is.EqualTo(2));
            Assert.That(data, Is.EqualTo("testDirectory true test1.txt false "));
        });
    }

    [Test]
    public async Task ServerShouldReturnDataFromGetRequest()
    {
        var (errorCode, data) = await this.client.SendGetRequestAsync(this.pathFile);
        Assert.Multiple(() =>
        {
            Assert.That(errorCode, Is.EqualTo(string.Empty));
            Assert.That(data, Is.EqualTo("textData"));
        });
    }

    [Test]
    public async Task ServerShouldReturnExceptionFromIncorrectPath()
    {
        var errorCode1 = (await this.client.SendGetRequestAsync(this.pathFile + "incorrect")).ErrorCode;
        var errorCode2 = (await this.client.SendListRequestAsync(this.pathFile + "incorrect")).ErrorCode;

        Assert.Multiple(() =>
        {
            Assert.That(errorCode1, Is.EqualTo("Invalid path"));
            Assert.That(errorCode2, Is.EqualTo("Invalid path"));
        });
    }

    [Test]
    public async Task ServerShouldReturnDataBySequentiallyRequests()
    {
        var tasks = new List<(string? ErrorCode, byte[]? FileBytes)>();

        for (var i = 0; i < 10; i++)
        {
            tasks.Add(await this.client.SendGetRequestAsync(this.pathFile));
        }

        foreach (var (errorCode, data) in tasks)
        {
            Assert.That(errorCode, Is.EqualTo(string.Empty));
            Assert.That(data, Is.EqualTo("textData"));
        }
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        this.client.Dispose();
        this.server.Dispose();
        Directory.Delete(this.path, true);
    }
}