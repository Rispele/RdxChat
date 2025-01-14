﻿using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using Rdx.Objects;
using Rdx.Objects.ValueObjects;
using Rdx.Serialization;
using Rdx.Serialization.Attributes.Markup;
using Tests.Serializer.Classes.Case1;

namespace Tests.Serializer;

[TestFixture]
[Parallelizable]
public class RdxSerializer_CustomObject_Tests
{
    private readonly RdxSerializer serializer = new(new ConstIdProvider(123));

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

    [TestCase(typeof(Dictionary<string, string>), "{\"key\":\"value\", \"key2\":\"value2\"}")]
    [TestCase(typeof(ChatMessageDto),
        "{\"MessageType\":\"ChatMessage\", \"Message\":\"sdfg\", \"SenderId\":\"d503691f-fc05-4f30-9525-8a4a69b50603\", \"SenderName\":\"UserName\", \"ReceiverId\":\"abe939e3-b2c1-42f7-8ca3-f359e26d0451\", \"MessageId\":\"262dd286-ae66-4a9b-b5c0-79b816bb6299\", \"SendingTime\":\"01/14/2025 22:33:55\"}")]
    [TestCase(typeof(TestObjectOuter),
        "{<\"Inner\":{<\"bool\":True>, <\"IntegerValue\":123>, <\"RdxString\":\"string\"@1-2>}>, <\"abc\":\"abc\">, <\"RdxObj\":{@0-0 <\"Value\":0>}>}")]
    public void Deserialize_ShouldNotThrow(Type type, string jdr)
    {
        var action = () => serializer.Deserialize(type, jdr);
        action.Should().NotThrow();
    }

    [Test]
    public void Serialize_CustomObject_ShouldReturnCorrectString()
    {
        var obj = new TestObjectOuter
        {
            abc = "abc",
            Inner = new TestObjectInner(
                true,
                123,
                new RdxValue<string>("string", 1, 2, 0),
                new RdxValue<string>("not_serializable", 3, 4,
                    0),
                null),
            RdxObj = new TestRdxObject
            {
                Value = 1
            }
        };
        var serialized = serializer.Serialize(obj);
        serialized
            .Should()
            .Be(
                "{\"Inner\":{\"bool\":True, \"IntegerValue\":123, \"RdxString\":\"string\"@1-2}, \"abc\":\"abc\", \"RdxObj\":{@0-0 \"Value\":1}}");
    }

    public static IEnumerable<TestObjectOuter> TestCaseSource()
    {
        var random = new Random();
        for (var i = 0; i < 10; i++)
            yield return new TestObjectOuter
            {
                abc = random.NextInt64(10000).ToString(),
                Inner = new TestObjectInner(
                    random.Next(2) == 0,
                    random.Next(1233),
                    new RdxValue<string>(
                        random.NextInt64(10000).ToString(),
                        random.NextInt64(10),
                        random.NextInt64(10),
                        0),
                    new RdxValue<string>(
                        "not_serializable",
                        3,
                        4,
                        0),
                    random.Next(2) == 0 ? Guid.NewGuid() : null),
                RdxObj = new TestRdxObject
                {
                    Value = random.Next(10)
                }
            };
    }

    public class TestObjectOuter
    {
        [RdxProperty] public TestObjectInner Inner { get; [UsedImplicitly] set; }

        [RdxProperty] public string abc { get; [UsedImplicitly] set; }

        [RdxProperty] public TestRdxObject RdxObj { get; [UsedImplicitly] set; }
    }

    public class TestRdxObject : RdxObject
    {
        public TestRdxObject() : base(0, 0, 0)
        {
        }

        [RdxProperty] public int Value { get; [UsedImplicitly] set; }
    }

    public class TestObjectInner
    {
        [UsedImplicitly]
        private TestObjectInner()
        {
        }

        public TestObjectInner(
            bool boolValue,
            int integer,
            RdxValue<string> rdxString,
            RdxValue<string> notSerializableValue,
            Guid? guid)
        {
            BooleanValue = boolValue;
            IntegerValue = integer;
            RdxString = rdxString;
            NotSerializable = notSerializableValue;
            Guid = guid;
        }

        [RdxProperty("bool")] private bool BooleanValue { get; [UsedImplicitly] set; }
        [RdxProperty] private int IntegerValue { get; [UsedImplicitly] set; }
        [RdxProperty] private RdxValue<string> RdxString { get; [UsedImplicitly] set; }
        [RdxProperty] private Guid? Guid { get; [UsedImplicitly] set; }
        public RdxValue<string> NotSerializable { get; [UsedImplicitly] set; }

        public bool TestEquals(TestObjectInner other)
        {
            return BooleanValue == other.BooleanValue
             && IntegerValue == other.IntegerValue
             && RdxString.Value == other.RdxString.Value
             && RdxString.Version == other.RdxString.Version
             && RdxString.ReplicaId == other.RdxString.ReplicaId
             && Guid == other.Guid;
        }
    }
}