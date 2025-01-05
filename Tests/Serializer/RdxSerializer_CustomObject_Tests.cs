using FluentAssertions;
using NUnit.Framework;
using Rdx.Objects;
using Rdx.Objects.ValueObjects;
using Rdx.Serialization;
using Rdx.Serialization.Attributes.Markup;
using Rdx.Serialization.Parser;
using Rdx.Serialization.RdxToObjectConverter;
using Rdx.Serialization.Tokenizer;

namespace Tests.Serializer;

[TestFixture]
[Parallelizable]
public class RdxSerializer_CustomObject_Tests
{
    private readonly RdxSerializer serializer = new();

    // private readonly string jRdx = ClearJRdxRegex().Replace(jRdx.Trim(), " ");
    //
    // [GeneratedRegex("\\s{2,}")]
    // private static partial Regex ClearJRdxRegex();
    
    [Test]
    public void Deserialize()
    {
        var s = "{<\"Inner\":{<\"bool\":True>, <\"IntegerValue\":123>, <\"RdxString\":\"string\"@0-0>}>, <\"abc\":\"abc\">, <\"RdxObj\":{@0-0 <\"Value\":0>}>}";
        var tokenizer = new RdxTokenizer(s);
        var t = tokenizer.Tokenize().ToList();
        var reader = new TokensReader(t, s);
        var parser = new RdxParser(reader);
        var parsed = parser.Parse();
        var converter = new SimpleConverter(new ConstIdProvider(123));
        var converted = converter.ConvertToType(typeof(TestObjectOuter), parsed);
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
        public TestObjectInner Inner { get; private set; } = new();

        [RdxProperty] public string abc { get; private set; } = "abc";

        [RdxProperty] public TestRdxObject RdxObj { get; private set; } = new TestRdxObject(0, 0, 0);
    }

    private class TestRdxObject : RdxObject
    {
        [RdxProperty]
        public int Value { get; private set; } = 0;

        private TestRdxObject() : base(0, 0, 0)
        {
            
        }
        
        public TestRdxObject(long replicaId, long version, long currentReplicaId) : base(replicaId, version, currentReplicaId)
        {
        }
    }
    
    private class TestObjectInner
    {
        [RdxProperty(propertyName: "bool")] private bool BooleanValue { get; set; } = true;
        [RdxProperty] private int IntegerValue { get;  set; } = 123;
        [RdxProperty] private RdxValue<string> RdxString { get; set; } = new("string", 0, 0, 0);
        private RdxValue<string> NotSerializable { get;  set; } = new("not_serializable", 0, 0, 0);
    }
}