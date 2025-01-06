using FluentAssertions;
using NUnit.Framework;
using Rdx.Objects.PlexValues;
using Rdx.Serialization;

namespace Tests.Serializer;

[TestFixture]
[Parallelizable]
public class RdxSerializer_DefaultCollections
{
    private readonly RdxSerializer serializer = new(new ConstIdProvider(123));

    [TestCaseSource(nameof(SerializeThenDeserializeListTestCaseSource))]
    public void List_Serialize_Then_Deserialize_ShouldEquals(List<int> list)
    {
        var serialized = serializer.Serialize(list);
        var deserialized = serializer.Deserialize<List<int>>(serialized);
        
        deserialized.Should().BeEquivalentTo(list);
    }

    public static IEnumerable<List<int>> SerializeThenDeserializeListTestCaseSource()
    {
        var rand = new Random();
        for (var i = 0; i < 10; i++)
        {
            yield return new List<int>(Enumerable.Range(0, rand.Next(10, 20)).Select(_ => rand.Next()));
        }
    }
}