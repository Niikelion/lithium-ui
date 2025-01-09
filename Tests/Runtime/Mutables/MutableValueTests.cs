using NUnit.Framework;
using UI.Li;

public class MutableValueTests
{
    [Test]
    public void CheckInitAndUpdates()
    {
        var mutableValue = new MutableValue<int>(1);
        var observer = new MutationObserver(mutableValue);
        
        Assert.IsTrue(mutableValue.Value == 1);
        Assert.IsTrue(!observer.Updated);
        
        mutableValue.Value = 5;
        
        Assert.IsTrue(mutableValue.Value == 5);
        Assert.IsTrue(observer.Updated);

        observer.Reset();
        
        mutableValue.Value = 4;
        
        Assert.IsTrue(observer.Updated);
    }

    [Test]
    public void CheckDispose()
    {
        var mutableValue = new MutableValue<int>(1);
        var observer = new MutationObserver(mutableValue);
        
        Assert.IsTrue(!observer.Updated);
        
        mutableValue.Dispose();
        
        Assert.IsTrue(!observer.Updated);
        
        mutableValue.Value = 5;
        
        Assert.IsTrue(!observer.Updated);
    }
}