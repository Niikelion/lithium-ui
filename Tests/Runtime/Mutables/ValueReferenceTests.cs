using NUnit.Framework;
using UI.Li;

public class ValueReferenceTests
{
    [Test]
    public void CheckInitAndUpdates()
    {
        var valueReference = new ValueReference<int>(2);
        var observer = new MutationObserver(valueReference);
        
        Assert.IsTrue(valueReference.Value == 2);
        Assert.IsTrue(!observer.Updated);
        
        valueReference.Value = 1;
        
        Assert.IsTrue(valueReference.Value == 1);
        Assert.IsTrue(!observer.Updated);
        
        valueReference.NotifyChanged();
        
        Assert.IsTrue(valueReference.Value == 1);
        Assert.IsTrue(observer.Updated);
    }

    [Test]
    public void CheckDispose()
    {
        var valueReference = new ValueReference<int>(4);
        var observer = new MutationObserver(valueReference);
        
        Assert.IsTrue(!observer.Updated);
        
        valueReference.Dispose();
        
        Assert.IsTrue(!observer.Updated);
        
        valueReference.NotifyChanged();
        
        Assert.IsTrue(!observer.Updated);
    }
}