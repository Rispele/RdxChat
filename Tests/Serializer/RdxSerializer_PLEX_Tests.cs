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

    [TestCaseSource(nameof(SerializeThenDeserializeRdxSetTestCaseSource))]
    public void RdxSet_Serialize_Then_Deserialize_ShouldEquals(RdxSet<int> set)
    {
        var serialized = serializer.Serialize(set);
        var deserialized = serializer.Deserialize<RdxSet<int>>(serialized);

        deserialized.Should().BeEquivalentTo(set);
        deserialized.Version.Should().Be(set.Version);
        deserialized.ReplicaId.Should().Be(set.ReplicaId);
    }

    [TestCaseSource(nameof(SerializeThenDeserializeRdxDictionaryTestCaseSource))]
    public void RdxDictionary_Serialize_Then_Deserialize_ShouldEquals(RdxDictionary<int, int> dictionary)
    {
        var serialized = serializer.Serialize(dictionary);
        var deserialized = serializer.Deserialize<RdxDictionary<int, int>>(serialized);

        deserialized.Should().BeEquivalentTo(dictionary);
        deserialized.Version.Should().Be(dictionary.Version);
        deserialized.ReplicaId.Should().Be(dictionary.ReplicaId);
    }

    [TestCaseSource(nameof(SerializeThenDeserializeRdxTupleTestCaseSource))]
    public void RdxTuple_Serialize_Then_Deserialize_ShouldEquals(RdxTuple<double, string> tuple)
    {
        var serialized = serializer.Serialize(tuple);
        var deserialized = serializer.Deserialize<RdxTuple<double, string>>(serialized);

        deserialized.First.Should().Be(tuple.First);
        deserialized.Second.Should().Be(tuple.Second);
        deserialized.Version.Should().Be(tuple.Version);
        deserialized.ReplicaId.Should().Be(tuple.ReplicaId);
    }

    [TestCaseSource(nameof(SerializeThenDeserializeRdxXPleTestCaseSource))]
    public void RdxXPle_Serialize_Then_Deserialize_ShouldEquals(RdxXPle<string> xple)
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
        for (var i = 0; i < 10; i++) yield return factory.NewTuple(rand.NextDouble(), rand.NextInt64().ToString());
    }

    public static IEnumerable<RdxXPle<string>> SerializeThenDeserializeRdxXPleTestCaseSource()
    {
        var rand = new Random();
        for (var i = 0; i < 10; i++)
            yield return new RdxXPle<string>(
                Enumerable.Range(0, rand.Next(10, 20)).Select(_ => rand.Next().ToString()).ToList(),
                rand.Next(),
                rand.Next(),
                rand.Next());
    }

    public static IEnumerable<RdxDictionary<int, int>> SerializeThenDeserializeRdxDictionaryTestCaseSource()
    {
        var rand = new Random();
        for (var i = 0; i < 10; i++)
            yield return new RdxDictionary<int, int>(
                Enumerable.Range(0, rand.Next(10, 20)).ToDictionary(_ => rand.Next(), _ => rand.Next()),
                rand.Next(),
                rand.Next(),
                rand.Next());
    }

    public static IEnumerable<RdxSet<int>> SerializeThenDeserializeRdxSetTestCaseSource()
    {
        var rand = new Random();
        for (var i = 0; i < 10; i++)
            yield return new RdxSet<int>(
                Enumerable.Range(0, rand.Next(10, 20)).Select(_ => rand.Next()).ToHashSet(),
                rand.Next(),
                rand.Next(),
                rand.Next());
    }
}