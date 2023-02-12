using Unity.Barracuda;
using UnityEngine;

namespace MediaPipe.Iris {

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

} // namespace MediaPipe.Iris
