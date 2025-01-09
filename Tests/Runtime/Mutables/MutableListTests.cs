using System.Linq;
using NUnit.Framework;
using UI.Li;

public class MutableListTests
{
    [Test]
    public void CheckInitAndIteration()
    {
        int[] source = { 0, 1, 6, 4 };
        
        var mutableList = new MutableList<int>(source);
        var observer = new MutationObserver(mutableList);
        
        Assert.IsTrue(mutableList.Count == source.Length);
        Assert.IsTrue(mutableList.Zip(source, (a, b) => a == b).All(b => b));
        
        for (int i=0; i<source.Length; i++)
            Assert.IsTrue(mutableList[i] == source[i]);
        
        Assert.IsTrue(!observer.Updated);
    }

    [Test]
    public void CheckDispose()
    {
        int[] source = { 2, 3 };
        
        var mutableList = new MutableList<int>(source);
        var observer = new MutationObserver(mutableList);
        
        Assert.IsTrue(!observer.Updated);
    }
}