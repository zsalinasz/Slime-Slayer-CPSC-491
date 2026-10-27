using NUnit.Framework;

public class SOC2CITest
{
    [Test]
    public void IntentionalFailureForCIValidation()
    {
        Assert.AreEqual(1, 2);
    }
}