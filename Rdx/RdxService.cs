using Rdx.Objects;
using Rdx.Primitives;
using Rdx.Serialization;

namespace Rdx;

public class RdxService
{
    public readonly RdxBuffer buffer;

    public RdxService(int bufferSize = 1048576)
    {
        buffer = new RdxBuffer(bufferSize, new RdxSerializer());
    }

    public TValue Merge<TValue>(RdxObject[] objects)
        where TValue: RdxObject
    {
        var slices = buffer.AppendObjects(objects);
        return default;
    }
}