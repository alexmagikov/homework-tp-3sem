using RunTestsUtils;
using Assert = RunTestsUtils.Assert;

namespace TestProject3;

public class TestClass
{
    public void TestWithoutAttribute()
    {
        Console.WriteLine("4");
    }

    [Before]
    public void Before()
     => throw new Exception();

    [RunTestsUtils.Test]
    public void Test1()
    {
        Assert.AreEqual(1, 1);
    }

    [RunTestsUtils.Test]
    public void Test2()
    {
        Assert.AreEqual(1, 1);
    }

    [After]
    public void After()
        => throw new Exception();
}