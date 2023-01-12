using Alteruna;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public static class WriterExtension
{
    public static void Write<T>(this Writer writer, NativeArray<T> array) where T : unmanaged
    {
        var asBytes = array.Reinterpret<byte>(UnsafeUtility.SizeOf<T>());

        // Maybe reinterpret NativeArray directly as C# array with offset of members
        writer.Write(asBytes.ToArray());
    }
}
