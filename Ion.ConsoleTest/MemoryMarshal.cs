using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using static Ion.ConsoleTest.MarshalTests;

namespace Ion.ConsoleTest;

[MemoryDiagnoser]
[RPlotExporter]
[SimpleJob(RuntimeMoniker.NativeAot10_0, baseline: true, launchCount: 1, warmupCount: 1, iterationCount: 8)]
//[SimpleJob(RuntimeMoniker.NativeAot10_0)]
public class MarshalTests
{
    public unsafe struct TestStruct
    {
        public fixed uint Data1[16];
        public fixed ulong Data2[16];
    }

    private TestStruct _testStruct;

    [GlobalSetup]
    public unsafe void Setup()
    {
        _testStruct = default;

        for (int i = 0; i < 16; i++)
        {
            _testStruct.Data1[i] = BitConverter.ToUInt32(RandomNumberGenerator.GetBytes(8));
            _testStruct.Data2[i] = BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8));
        }
    }

    [Benchmark(Description = "ToBytesMarshal")]
    public byte[] ToBytesMarshal()
    {
        return MarshalMemory<TestStruct>.ToBytesMarshal(_testStruct);
    }

    [Benchmark(Description = "ToBytesUnsafe")]
    public byte[] ToBytesUnsafe()
    {
        return MarshalMemory<TestStruct>.ToBytesUnsafe(_testStruct);
    }

    [Benchmark(Description = "ToPointerNoAllocation")]
    public nint ToPointerNoAllocation()
    {
        return MarshalMemory<TestStruct>.ToPointerNoAllocation(in _testStruct);
    }
}

public static class MarshalMemory<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                                    DynamicallyAccessedMemberTypes.NonPublicConstructors)] T> where T : unmanaged
{
    public static readonly int Size = Unsafe.SizeOf<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToBytesMarshal(T value)
    {
        var data = new byte[Size];
        MemoryMarshal.Write(data, in value);
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToBytesUnsafe(T value)
    {
        var data = GC.AllocateUninitializedArray<byte>(Size);
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetArrayDataReference(data), value);
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe nint ToPointerNoAllocation(in T value)
    {
        ref T reference = ref Unsafe.AsRef(in value);
        var pointer = Unsafe.AsPointer(ref reference);
        return (nint)pointer;
    }
}