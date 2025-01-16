# Сериализация в JDR
Чтобы сериализовать/десериализовать структуру в формат JDR нужно использовать RdxSerializer.

## Поддерживаемые сущности языка

### Примитивы
- bool: `True`, `False`;
- double: `123.123`;
- int, long: `123123`;
- Nullable<T>: сериализуется как тип `T` или `null`;
- string: `"Hello, World!"`;
- DateTime: `"01/16/2025 03:18:54"`;
- Guid: `"7cc583df-8a27-41d2-bae0-24e015ced8b8"`.

### Коллекции
- Dictionary<K, V>: `{key1:val1, key2:val2}`;
- HashSet<T>: `{123, 321}`;
- List<T>: `[1, 2, 3]`.

## Поддерживаемые сущности RDX
Все сущности RDX должны иметь штамп времени и ид реплики. Поэтому все они унаследованы от RdxObject. Чтобы создать свой тип RDX тип его нужно унаследовать от RdxObject.

### Примитивы
Представлены классом `RdxValue<T>`. `T` может быть `string`, `int`, `long`, `double`, `bool`, `DateTime`, `Guid`. 
Сериализуются со штампом времени. Например, `123@dead-beef` или `"7cc583df-8a27-41d2-bae0-24e015ced8b8"@1234-4321`.

### Коллекции
Мы за строгую типизацию коллекций, поэтому все объекты коллекции имеют один тип.
- RdxDictionary<K, V>: `{@dead-beef key1:val1, key2:val2}`;
- RdxSet<T>: `{@dead-beef 123, 321}`;
- RdxTuple<T1, T2>: `<@dead-beef val1:val2>`. Всегда устанавливаются скобки.
- RdxXPle<T>: `<@dead-beef val1:val2:val3:val4>`. Представляет собой кортеж n значений.
Сущности для массива нет в силу того, что для него не поддержан мерж librdx. Это значит, что не имеет смысла ставить на него штамп времени.

## Поизвольный объект
Можно сериализовать произвольный класс. Для этого предусмотрен аттрибут `RdxPropertyAttribute`. Только такие элементы будут сериализованы. Его можно повесить на св-ва и поля любого модификатора доступа. В конструктор RdxPropertyAttribute можно передать строку - имя элемента при сериализации.
Для десериализации обязательно должен присутствовать стандартный конструктор без параметров любого модификтора доступа.
### Пример
```
    public class TestObjectOuter
    {
        [RdxProperty] public TestObjectInner Inner { get; [UsedImplicitly] set; }
        [RdxProperty] public string abc { get; [UsedImplicitly] set; }
        [RdxProperty] public TestRdxObject RdxObj { get; [UsedImplicitly] set; }
    }

    public class TestRdxObject : RdxObject
    {
        public TestRdxObject() : base(0, 0, 0)
        { }

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
```
Будет сериализован примерно в {"Inner":{"bool":True, "IntegerValue":123, "RdxString":"string"@1-2}, "abc":"abc", "RdxObj":{@0-0 "Value":1}}

## Расширяемость
Можно добавить собственную реализацию сериализации/десериализации создав аттриубут унаследованный от `RdxSerializerAttribute` и повесив его на ваш класс. 
Пример. [Атрибут](https://github.com/Rispele/RdxChat/blob/master/Rdx/Serialization/Attributes/RdxTupleSerializerAttribute.cs) для [RdxTuple](https://github.com/Rispele/RdxChat/blob/master/Rdx/Objects/PlexValues/RdxTuple.cs). 



























