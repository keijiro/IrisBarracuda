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

static class IWorkerExtensions
{
    //
    // Retrieves an output tensor from a NN worker and returns it as a
    // temporary render texture. The caller must release it using
    // RenderTexture.ReleaseTemporary.
    //
    public static RenderTexture
      CopyOutputToTempRT(this IWorker worker, string name, int w, int h)
    {
        var fmt = RenderTextureFormat.RFloat;
        var rt = RenderTexture.GetTemporary(w, h, 0, fmt);
#if BARRACUDA_4_0_0_OR_LATER
        var shape = new TensorShape(1, 1, h, w);
        using (var tensor = (TensorFloat)worker.PeekOutput(name).ShallowReshape(shape))
            TensorToRenderTexture.ToRenderTexture(tensor, rt);
#else
        var shape = new TensorShape(1, h, w, 1);
        using (var tensor = worker.PeekOutput(name).Reshape(shape))
            tensor.ToRenderTexture(rt);
#endif
        return rt;
    }
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
