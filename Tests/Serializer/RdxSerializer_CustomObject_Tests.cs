﻿using FluentAssertions;
using NUnit.Framework;
using Rdx.Objects;
using Rdx.Objects.ValueObjects;
using Rdx.Serialization;
using Rdx.Serialization.Attributes.Markup;
using Rdx.Serialization.Parser;
using Rdx.Serialization.Tokenizer;

namespace Tests.Serializer;

[TestFixture]
[Parallelizable]
public class RdxSerializer_CustomObject_Tests
{
    private readonly RdxSerializer serializer = new();

    [Test]
    public void Deserialize()
    {
        var s =
            "{<\"Inner\":{<\"bool\":True>, <\"IntegerValue\":123>, <\"RdxString\":\"string\"@0-0>}>, <\"abc\":\"abc\">, <\"RdxObj\":{@0-0 <\"Value\":0>}>}";
        var t = RdxTokenizer.Tokenize(s);
        var reader = new TokensReader(t, s);
        var parsed = RdxParser.Parse(reader);
        foreach (var token in t)     
        {
            Console.WriteLine(s[token.Start..(token.Start + token.Length)]);            
        }
    }
    
    [Test]
    public void Serialize_CustomObject_ShouldReturnCorrectString()
    {
        var obj = new TestObjectOuter();
        serializer.Serialize(obj).Should()
            .Be(
                "{<\"Inner\":{<\"bool\":True>, <\"IntegerValue\":123>, <\"RdxString\":\"string\"@0-0>}>, <\"abc\":\"abc\">, <\"RdxObj\":{@0-0 <\"Value\":0>}>}");
    } 
    
    private class TestObjectOuter
    {
        [RdxProperty]
        public TestObjectInner Inner { get; } = new();

        [RdxProperty] public string abc { get; } = "abc";

        [RdxProperty] public TestRdxObject RdxObj { get; } = new TestRdxObject(0, 0, 0);
    }

    private class TestRdxObject : RdxObject
    {
        [RdxProperty]
        public int Value { get; } = 0;
        
        public TestRdxObject(long replicaId, long version, long currentReplicaId) : base(replicaId, version, currentReplicaId)
        {
        }
    }
    
    private class TestObjectInner
    {
        [RdxProperty(propertyName: "bool")] private bool BooleanValue { get; } = true;
        [RdxProperty] private int IntegerValue { get; } = 123;
        [RdxProperty] private RdxValue<string> RdxString { get; } = new("string", 0, 0, 0);
        private RdxValue<string> NotSerializable { get; } = new("not_serializable", 0, 0, 0);
    }
}