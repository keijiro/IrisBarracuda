using Unity.Barracuda;
using UnityEngine;

namespace MediaPipe.Iris {

#region Object construction/destruction helpers

static class BufferUtil
{
    public unsafe static GraphicsBuffer NewStructured<T>(int length) where T : unmanaged
      => new GraphicsBuffer(GraphicsBuffer.Target.Structured, length, sizeof(T));
}

#endregion

#region Extension methods

static class ComputeShaderExtensions
{
    public static void DispatchThreads
      (this ComputeShader compute, int kernel, int x, int y, int z)
    {
        uint xc, yc, zc;
        compute.GetKernelThreadGroupSizes(kernel, out xc, out yc, out zc);
        x = (x + (int)xc - 1) / (int)xc;
        y = (y + (int)yc - 1) / (int)yc;
        z = (z + (int)zc - 1) / (int)zc;
        compute.Dispatch(kernel, x, y, z);
    }
}

static class IWorkerExtensions
{
    public static ComputeBuffer PeekOutputBuffer
      (this IWorker worker, string tensorName)
      => ((ComputeTensorData)worker.PeekOutput(tensorName).tensorOnDevice).buffer;
}

#endregion

#region GPU to CPU readback helpers

sealed class ReadCache
{
    public ReadCache(GraphicsBuffer source) => _source = source;
    public System.ReadOnlySpan<Vector4> Cached => Read();
    public void Invalidate() => _isCached = false;

    GraphicsBuffer _source;
    Vector4[] _cache = new Vector4[EyeLandmarkDetector.VertexCount];
    bool _isCached;

    System.ReadOnlySpan<Vector4> Read()
    {
        if (_isCached) return _cache;
        _source.GetData(_cache, 0, 0, _cache.Length);
        _isCached = true;
        return _cache;
    }
}

#endregion

} // namespace MediaPipe.Iris
