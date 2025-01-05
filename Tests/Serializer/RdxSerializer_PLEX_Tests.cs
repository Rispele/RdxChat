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

    [TestCaseSource(nameof(RdxValueTestCaseSource))]
    public string Serialize_RdxValues_Should_ReturnCorrectValue(object value)
    {
        return serializer.Serialize(value);
    }
    
    public static IEnumerable<TestCaseData> RdxValueTestCaseSource()
    {
        var factory = new RdxObjectFactory(new ConstIdProvider(0));
        yield return new TestCaseData(factory.NewTuple(1.1, true)).Returns("<@0-0 1.1:True>");
        yield return new TestCaseData(factory.NewTuple(1.1, "string")).Returns("<@0-0 1.1:\"string\">");
        yield return new TestCaseData(factory.NewTuple("string", false)).Returns("<@0-0 \"string\":False>");
        
        yield return new TestCaseData(factory.NewTuple(factory.NewTuple(1, true), factory.NewValue("string")))
            .Returns("<@0-0 <@0-0 1:True>:\"string\"@0-0>");
    }
}