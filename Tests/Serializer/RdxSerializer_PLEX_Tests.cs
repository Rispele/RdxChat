using FluentAssertions;
using NUnit.Framework;
using Rdx.Objects;
using Rdx.Objects.PlexValues;
using Rdx.Serialization;

namespace Tests.Serializer;

[TestFixture]
[Parallelizable]
public class RdxSerializer_PLEX_Tests
{
    private readonly RdxSerializer serializer = new(new ConstIdProvider(123));

    [TestCaseSource(nameof(SerializeThenDeserializeRdxTupleTestCaseSource))]
    public void Serialize_Then_Deserialize_ShouldEquals(RdxTuple<double, string> tuple)
    {
        var serialized = serializer.Serialize(tuple);
        var deserialized = serializer.Deserialize<RdxTuple<double, string>>(serialized);
        
        deserialized.First.Should().Be(tuple.First);
        deserialized.Second.Should().Be(tuple.Second);
        deserialized.Version.Should().Be(tuple.Version);
        deserialized.ReplicaId.Should().Be(tuple.ReplicaId);
    }
    
    [TestCaseSource(nameof(SerializeThenDeserializeRdxXPleTestCaseSource))]
    public void Serialize_Then_Deserialize_ShouldEquals(RdxXPle<string> xple)
    {
        var serialized = serializer.Serialize(xple);
        var deserialized = serializer.Deserialize<RdxXPle<string>>(serialized);
        
        deserialized.Should().BeEquivalentTo(xple);
        deserialized.Version.Should().Be(xple.Version);
        deserialized.ReplicaId.Should().Be(xple.ReplicaId);
    }
    
    [TestCaseSource(nameof(SerializeTestCaseSource))]
    public string Serialize_RdxValues_Should_ReturnCorrectValue(object value)
    {
        return serializer.Serialize(value);
    }
    
    public static IEnumerable<TestCaseData> SerializeTestCaseSource()
    {
        var factory = new RdxObjectFactory(new ConstIdProvider(0));
        yield return new TestCaseData(factory.NewTuple(1.1, true)).Returns("<@0-0 1.1:True>");
        yield return new TestCaseData(factory.NewTuple(1.1, "string")).Returns("<@0-0 1.1:\"string\">");
        yield return new TestCaseData(factory.NewTuple("string", false)).Returns("<@0-0 \"string\":False>");
        
        yield return new TestCaseData(factory.NewTuple(factory.NewTuple(1, true), factory.NewValue("string")))
            .Returns("<@0-0 <@0-0 1:True>:\"string\"@0-0>");
    }

    public static IEnumerable<RdxTuple<double, string>> SerializeThenDeserializeRdxTupleTestCaseSource()
    {
        var factory = new RdxObjectFactory(new ConstIdProvider(0));
        var rand = new Random();
        for (var i = 0; i < 10; i++)
        {
            yield return factory.NewTuple(rand.NextDouble(), rand.NextInt64().ToString());
        }
    }
    
    public static IEnumerable<RdxXPle<string>> SerializeThenDeserializeRdxXPleTestCaseSource()
    {
        var rand = new Random();
        for (var i = 0; i < 10; i++)
        {
            yield return new RdxXPle<string>(
                Enumerable.Range(0, rand.Next(10, 20)).Select(_ => (object)rand.Next().ToString()).ToList(),
                rand.Next(),
                rand.Next(),
                rand.Next());
        }
    }
}