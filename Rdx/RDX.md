# Работа с RDX

### Любая работа с RDX работает только под линуксом! Это ограничение оригинальной библиотеки.

## Подготовка

Для работы нужна библиотека librdx. Ее нужно поместить в сборку, где будет использоваться rdx.

Получить исходный код можно по [ссылке](https://github.com/Rispele/librdx). Как оговорено выше,
библиотека собирается только под линукс. Можно использоваться wsl. 

Запускать приложение, которое использует Rdx тоже нужно запускать под линукс.

## Работа
Для работы используются такие примитивы как RdxBuffer и RdxSlice. Первый представляет из себя
область памяти с которой будет работать оригинальная библиотека. Второй представляет собой 
отрезок на RdxBuffer, который содержит определенные данные.

Rdx объекты можно удобно создавать с помощью RdxObjectFactory

## Примеры

```
var provider = new ConstIdProvider();
var serializer = new RdxSerializer(provider);
var buffer = new RdxBuffer(1024, serializer);
var factory = new RdxObjectFactory(provider);

var t1 = factory.NewTuple(1, factory.NewValue(2));
var t2 = factory.NewTuple(factory.NewValue(1), factory.NewValue(1));

var id = Random.Shared.NextInt64();
var t3 = factory.Tuple(
    factory.Value(1, id, 1),
    factory.Value(4, id, 1),
    id,
    version: 3);
    
// Здесь мы положили объекты в буфер
var slices = buffer.AppendObjects([t1, t2, t3]);

// Здесь мы их помержили
var mergedSlice = buffer.Merge(slices);

// Дальше можно их достать, они вытаскиваются как строка, которую можно десериализовать.
Console.WriteLine(buffer.ExtractObject(slices[0]));
Console.WriteLine(buffer.ExtractObject(slices[1]));
Console.WriteLine(buffer.ExtractObject(slices[2]));
Console.WriteLine(buffer.ExtractObject(mergedSlice));
```