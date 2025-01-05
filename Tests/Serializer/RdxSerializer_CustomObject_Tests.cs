using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using Rdx.Objects;
using Rdx.Objects.ValueObjects;
using Rdx.Serialization;
using Rdx.Serialization.Attributes.Markup;

namespace Tests.Serializer;

[TestFixture]
[Parallelizable]
public class RdxSerializer_CustomObject_Tests
{
    private readonly RdxSerializer serializer = new(replicaIdProvider: new ConstIdProvider(id: 123));

    // private readonly string jRdx = ClearJRdxRegex().Replace(jRdx.Trim(), " ");
    //
    // [GeneratedRegex("\\s{2,}")]
    // private static partial Regex ClearJRdxRegex();

    [TestCaseSource(nameof(TestCaseSource))]
    public void Serialize_Then_Deserialize_ShouldEquals(TestObjectOuter testObject)
    {
        var serialized = serializer.Serialize(testObject);
        var deserialized = serializer.Deserialize<TestObjectOuter>(serialized);
        
        deserialized.abc.Should().Be(testObject.abc);
        
        deserialized.RdxObj.Value.Should().Be(testObject.RdxObj.Value);
        deserialized.RdxObj.Version.Should().Be(testObject.RdxObj.Version);
        deserialized.RdxObj.ReplicaId.Should().Be(testObject.RdxObj.ReplicaId);
        
        deserialized.Inner.TestEquals(testObject.Inner).Should().BeTrue();
        deserialized.Inner.NotSerializable.Should().BeNull();
    }

    [Test]
    public void Deserialize_ShouldNotThrow()
    {
        var s =
            "{<\"Inner\":{<\"bool\":True>, <\"IntegerValue\":123>, <\"RdxString\":\"string\"@1-2>}>, <\"abc\":\"abc\">, <\"RdxObj\":{@0-0 <\"Value\":0>}>}";
        var action = () => serializer.Deserialize<TestObjectOuter>(jRdx: s);
        action.Should().NotThrow();
    }

    [Test]
    public void Serialize_CustomObject_ShouldReturnCorrectString()
    {
        var obj = new TestObjectOuter
        {
            abc = "abc",
            Inner = new TestObjectInner(
                boolValue: true,
                integer: 123,
                rdxString: new RdxValue<string>(value: "string", replicaId: 1, version: 2, currentReplicaId: 0),
                notSerializableValue: new RdxValue<string>(value: "not_serializable", replicaId: 3, version: 4,
                    currentReplicaId: 0)),
            RdxObj = new TestRdxObject
            {
                Value = 1
            }
        };
        serializer.Serialize(obj: obj).Should()
            .Be(
                expected:
                "{<\"Inner\":{<\"bool\":True>, <\"IntegerValue\":123>, <\"RdxString\":\"string\"@1-2>}>, <\"abc\":\"abc\">, <\"RdxObj\":{@0-0 <\"Value\":1>}>}");
    }

    public static IEnumerable<TestObjectOuter> TestCaseSource()
    {
        var random = new Random();
        for (var i = 0; i < 10; i++)
        {
            yield return new TestObjectOuter
            {
                abc = random.NextInt64(10000).ToString(),
                Inner = new TestObjectInner(
                    boolValue: random.Next(2) == 0,
                    integer: random.Next(1233),
                    rdxString: new RdxValue<string>(
                        value: random.NextInt64(10000).ToString(),
                        replicaId: random.NextInt64(10),
                        version: random.NextInt64(10),
                        currentReplicaId: 0),
                    notSerializableValue: new RdxValue<string>(
                        value: "not_serializable",
                        replicaId: 3,
                        version: 4,
                        currentReplicaId: 0)),
                RdxObj = new TestRdxObject
                {
                    Value = random.Next(10)
                }
            };
        }
    }

    public class TestObjectOuter
    {
        [RdxProperty] public TestObjectInner Inner { get; set; }

        [RdxProperty] public string abc { get; set; }

        [RdxProperty] public TestRdxObject RdxObj { get; set; }
    }

    public class TestRdxObject : RdxObject
    {
        [RdxProperty] public int Value { get; set; }

        public TestRdxObject() : base(replicaId: 0, version: 0, currentReplicaId: 0)
        {
        }
    }

    public class TestObjectInner
    {
        [RdxProperty(propertyName: "bool")] private bool BooleanValue { get; set; }
        [RdxProperty] private int IntegerValue { get; set; }
        [RdxProperty] private RdxValue<string> RdxString { get; set; }
        public RdxValue<string> NotSerializable { get; set; }

        [UsedImplicitly]
        private TestObjectInner()
        {
            
        }

        public TestObjectInner(bool boolValue, int integer, RdxValue<string> rdxString,
            RdxValue<string> notSerializableValue)
        {
            BooleanValue = boolValue;
            IntegerValue = integer;
            RdxString = rdxString;
            NotSerializable = notSerializableValue;
        }

        public bool TestEquals(TestObjectInner other)
        {
            return BooleanValue == other.BooleanValue
                   && IntegerValue == other.IntegerValue
                   && RdxString.Value == other.RdxString.Value
                   && RdxString.Version == other.RdxString.Version
                   && RdxString.ReplicaId == other.RdxString.ReplicaId;
        }
    }
}