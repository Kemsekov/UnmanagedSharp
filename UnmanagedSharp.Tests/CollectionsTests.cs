using Xunit;
using System.Linq;
namespace UnmanagedSharp.Tests;

public class CollectionsTests
{

    [Fact]
    public void LinkedListTest()
    {
        var expected = new System.Collections.Generic.LinkedList<int>();
        using var actual = new UnmanagedSharp.LinkedList<int>();

        Assert.Equal(expected.Count, actual.Count);
        Common.FillCollection(expected, 10, i => i);
        Common.FillCollection(actual, 10, i => i);
        Assert.Equal(expected, actual);

        expected.Remove(2);
        actual.Remove(2);
        Assert.Equal(expected, actual);
        Common.FillCollection(expected, 10, i => i*2);
        Common.FillCollection(actual, 10, i => i*2);
        Assert.Equal(expected, actual);
        var buffer1 = new int[expected.Count];
        var buffer2 = new int[expected.Count];
        expected.CopyTo(buffer1, 0);
        actual.CopyTo(buffer2, 0);
        Assert.Equal(buffer1, buffer2);
        foreach(var n in expected)
            Assert.True(actual.Contains(n));
        
        actual.AddRange(new[]{9,8,7});
        expected.AddLast(9);
        expected.AddLast(8);
        expected.AddLast(7);
        Assert.Equal(expected, actual);
    }
}