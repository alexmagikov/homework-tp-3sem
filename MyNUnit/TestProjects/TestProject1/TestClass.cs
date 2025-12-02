using RunTestsUtils;

namespace TestProject1;

public class TestClass
{
    [BeforeClass]
    public static void BeforeClass()
    {
        Console.WriteLine("2");
    }

    [BeforeClass]
    public void BeforeClass2()
    {
        Console.WriteLine("3");
    }


    [Test]
    public void Test1()
    {
        Assert.AreEqual(1, 2);
    }

    [Test(Expected = typeof(DivideByZeroException))]
    public void Test2()
        => throw new DivideByZeroException();

    [Test(Ignore = "Ignore")]
    public void Test3()
        => Assert.AreEqual(1, 2);

    [AfterClass]
    public static void AfterClass()
    {
        Console.WriteLine("3");
    }
}