using RunTestsUtils;

namespace TestProject1;

public class TestClass
{
    [BeforeClass]
    public static int BeforeClass()
    {
        return 32;
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
        Console.WriteLine("AfterClass");
    }
}