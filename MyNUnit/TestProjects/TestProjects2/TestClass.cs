using RunTestsUtils;

namespace TestProjects2;

public class TestClass
{
    [BeforeClass]
    public static void BeforeClass()
    {
        Console.WriteLine("2");
    }

    [Test]
    public void Test1()
    {
        Assert.AreEqual(1, 2);
    }

    [AfterClass]
    public static void AfterClass()
    {
        Console.WriteLine("3");
    }
}