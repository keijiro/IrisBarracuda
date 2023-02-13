using Unity.Barracuda;
using UnityEngine;
using Klak.NNUtils;
using Klak.NNUtils.Extensions;

namespace MediaPipe.Iris {

//
// Eye landmark detector class
//
// The vertex array returned from this class is slightly different from
// MediaPipe's one. The vertices are concatenated into a single array.
//
// [0 - 4] : Iris vertices
// [5 - 75] : Eyelid and eyebrow vertices
//
// You can use the extension methods defined in
// EyeLandmarkDetectorExtensions.cs to get specific keypoints.
//
public sealed class EyeLandmarkDetector : System.IDisposable
{
    #region Public methods/properties

    public const int IrisVertexCount = 5;
    public const int ContourVertexCount = 71;
    public const int VertexCount = IrisVertexCount + ContourVertexCount;

    public EyeLandmarkDetector(ResourceSet resources)
      => AllocateObjects(resources);

    public void Dispose()
      => DeallocateObjects();

    public void ProcessImage(Texture image)
      => RunModel(image);

    public GraphicsBuffer VertexBuffer
      => _output;

    public System.ReadOnlySpan<Vector4> VertexArray
      => _readCache.Cached;

    #endregion

    #region Private objects

    // Input image size (defined by the model)
    const int ImageSize = 64;

    ResourceSet _resources;
    IWorker _worker;
    ImagePreprocess _preprocess;
    GraphicsBuffer _output;
    BufferReader<Vector4> _readCache;

    void AllocateObjects(ResourceSet resources)
    {
        // NN model
        var model = ModelLoader.Load(resources.model);

        // Private objects
        _resources = resources;
        _worker = model.CreateWorker(WorkerFactory.Device.GPU);

        // Preprocessing buffer
        _preprocess = new ImagePreprocess(ImageSize, ImageSize, nchwFix: true);

        // Output buffer
        _output = BufferUtil.NewStructured<Vector4>(VertexCount);

        // Read cache
        _readCache = new BufferReader<Vector4>(_output, VertexCount);
    }

    void DeallocateObjects()
    {
        _worker?.Dispose();
        _worker = null;

        _preprocess?.Dispose();
        _preprocess = null;

        _output?.Dispose();
        _output = null;
    }

    #endregion

    #region Neural network inference function

    void RunModel(Texture source)
    {
        // Preprocessing
        _preprocess.Dispatch(source, _resources.preprocess);

        // Run the BlazeFace model.
        _worker.Execute(_preprocess.Tensor);

        // Postprocessing
        var post = _resources.postprocess;
        post.SetBuffer(0, "_IrisTensor", _worker.PeekOutputBuffer("output_iris"));
        post.SetBuffer(0, "_ContourTensor", _worker.PeekOutputBuffer("output_eyes_contours_and_brows"));
        post.SetBuffer(0, "_Vertices", _output);
        post.Dispatch(0, 1, 1, 1);

        // Cache data invalidation
        _readCache.InvalidateCache();
    }

    #endregion
}

} // namespace MediaPipe.Iris
