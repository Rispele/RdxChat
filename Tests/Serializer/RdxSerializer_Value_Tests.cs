using FluentAssertions;
using NUnit.Framework;
using Rdx.Objects.ValueObjects;
using Rdx.Serialization;

namespace Tests.Serializer;

[TestFixture]
[Parallelizable]
public class RdxSerializer_Value_Tests
{
    private readonly RdxSerializer serializer = new(new ConstIdProvider(123));

    [TestCase(1, "1")]
    [TestCase(1.1d, "1.1")]
    [TestCase(1L, "1")]
    [TestCase("1", "\"1\"")]
    [TestCase(true, "True")]
    public void Serialize_NotRdxValues_Should_ReturnCorrectValue(object value, string expected)
    {
        serializer.Serialize(value).Should().Be(expected);
    }

    [TestCaseSource(nameof(RdxValueTestCaseSource))]
    public string Serialize_RdxValues_Should_ReturnCorrectValue(object value)
    {
        return serializer.Serialize(value);
    }

    public static IEnumerable<TestCaseData> RdxValueTestCaseSource()
    {
        yield return new TestCaseData(new RdxValue<int>(1, 0, 0, 0)).Returns("1@0-0");
        yield return new TestCaseData(new RdxValue<double>(1.1, 0L, 1L, 0L)).Returns("1.1@0-1");
        yield return new TestCaseData(new RdxValue<long>(1L, 0L, 2L, 0L)).Returns("1@0-2");
        yield return new TestCaseData(new RdxValue<bool>(true, 0L, 3L, 0L)).Returns("True@0-3");
        yield return new TestCaseData(new RdxValue<string>("string", 0L, 4L, 0L)).Returns("\"string\"@0-4");
    }
}