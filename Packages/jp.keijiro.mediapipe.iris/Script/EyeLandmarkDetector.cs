using Unity.Barracuda;
using UnityEngine;

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
    #region Public accessors

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

    #region Compile-time constants

    // Input image size (defined by the model)
    const int ImageSize = 64;

    // Output tensors
    const string IrisOutputName = "output_iris";
    const string ContourOutputName = "output_eyes_contours_and_brows";

    #endregion

    #region Private objects

    ResourceSet _resources;
    IWorker _worker;
    (Tensor tensor, ComputeTensorData data) _preprocess;
    GraphicsBuffer _output;
    ReadCache _readCache;

    void AllocateObjects(ResourceSet resources)
    {
        // NN model
        var model = ModelLoader.Load(resources.model);

        // Private objects
        _resources = resources;
        _worker = model.CreateWorker(WorkerFactory.Device.GPU);

        // Preprocessing buffer
#if BARRACUDA_4_0_0_OR_LATER
        var shape = new TensorShape(1, 3, ImageSize, ImageSize);
        _preprocess.data = new ComputeTensorData(shape, "Input", false);
        _preprocess.tensor = TensorFloat.Zeros(shape);
        _preprocess.tensor.AttachToDevice(_preprocess.data);
#else
        var shape = new TensorShape(1, ImageSize, ImageSize, 3);
        _preprocess.data = new ComputeTensorData
          (shape, "Input", ComputeInfo.ChannelsOrder.NHWC, false);
        _preprocess.tensor = new Tensor(shape, _preprocess.data);
#endif

        // Output buffer
        _output = BufferUtil.NewStructured<Vector4>(VertexCount);

        // Read cache
        _readCache = new ReadCache(_output);
    }

    void DeallocateObjects()
    {
        _worker?.Dispose();
        _worker = null;

        _preprocess.tensor?.Dispose();
        _preprocess = (null, null);

        _output?.Dispose();
        _output = null;
    }

    #endregion

    #region Neural network inference function

    void RunModel(Texture source)
    {
#if BARRACUDA_4_0_0_OR_LATER
        const int PrePassNum = 1;
#else
        const int PrePassNum = 0;
#endif

        // Preprocessing
        var pre = _resources.preprocess;
        pre.SetTexture(PrePassNum, "_Texture", source);
        pre.SetBuffer(PrePassNum, "_Tensor", _preprocess.data.buffer);
        pre.Dispatch(PrePassNum, ImageSize / 8, ImageSize / 8, 1);

        // Run the BlazeFace model.
        _worker.Execute(_preprocess.tensor);

        // Postprocessing
        var post = _resources.postprocess;
        var irisRT = _worker.CopyOutputToTempRT(IrisOutputName, 3, IrisVertexCount);
        var contRT = _worker.CopyOutputToTempRT(ContourOutputName, 3, ContourVertexCount);
        post.SetTexture(0, "_IrisTensor", irisRT);
        post.SetTexture(0, "_ContourTensor", contRT);
        post.SetBuffer(0, "_Vertices", _output);
        post.Dispatch(0, 1, 1, 1);
        RenderTexture.ReleaseTemporary(irisRT);
        RenderTexture.ReleaseTemporary(contRT);

        // Cache data invalidation
        _readCache.Invalidate();
    }

    #endregion
}

} // namespace MediaPipe.Iris
