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
    
    [TestCaseSource(nameof(SerializeThenDeserializeHashSetTestCaseSource))]
    public void HashSet_Serialize_Then_Deserialize_ShouldEquals(HashSet<int> hashSet)
    {
        var serialized = serializer.Serialize(hashSet);
        var deserialized = serializer.Deserialize<HashSet<int>>(serialized);
        
        deserialized.Should().BeEquivalentTo(hashSet);
    }
    
    [TestCaseSource(nameof(SerializeThenDeserializeDictionaryTestCaseSource))]
    public void Dictionary_Serialize_Then_Deserialize_ShouldEquals(Dictionary<int, string> hashSet)
    {
        var serialized = serializer.Serialize(hashSet);
        var deserialized = serializer.Deserialize<Dictionary<int, string>>(serialized);
        
        deserialized.Should().BeEquivalentTo(hashSet);
    }

    public static IEnumerable<List<int>> SerializeThenDeserializeListTestCaseSource()
    {
        var rand = new Random();
        for (var i = 0; i < 10; i++)
        {
            yield return new List<int>(Enumerable.Range(0, rand.Next(10, 20)).Select(_ => rand.Next()));
        }
    }
    
    public static IEnumerable<HashSet<int>> SerializeThenDeserializeHashSetTestCaseSource()
    {
        var rand = new Random();
        for (var i = 0; i < 10; i++)
        {
            yield return new HashSet<int>(Enumerable.Range(0, rand.Next(10, 20)).Select(_ => rand.Next()));
        }
    }
    
    public static IEnumerable<Dictionary<int, string>> SerializeThenDeserializeDictionaryTestCaseSource()
    {
        var rand = new Random();
        for (var i = 0; i < 10; i++)
        {
            yield return new Dictionary<int, string>(
                Enumerable.Range(0, rand.Next(10, 20)).ToDictionary(_ => rand.Next(), _ => rand.Next().ToString()));
        }
    }
}