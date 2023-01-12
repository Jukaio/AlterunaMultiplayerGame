using Alteruna;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public static class ReaderExtension
{
    public static NativeArray<T> Read<T>(this Reader reader) where T : unmanaged
    {
        var asBytes = reader.ReadByteArray();
        var bytes = new NativeArray<byte>(asBytes, Allocator.Temp);
        return bytes.Reinterpret<T>(UnsafeUtility.SizeOf<byte>());
    }
}
