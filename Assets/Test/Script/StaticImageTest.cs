using UnityEngine;
using UI = UnityEngine.UI;

namespace MediaPipe {

public sealed class StaticImageTest : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] Iris.ResourceSet _resources = null;
    [SerializeField] Texture2D _image = null;
    [SerializeField] UI.RawImage _previewUI = null;
    [SerializeField] Shader _shader = null;

    #endregion

    #region Private members

    Iris.IrisDetector _detector;
    Material _material;
    Bounds _bounds = new Bounds(Vector3.zero, Vector3.one);

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _previewUI.texture = _image;

        _detector = new Iris.IrisDetector(_resources);
        _detector.ProcessImage(_image);

        _material = new Material(_shader);
        _material.SetBuffer("_Vertices", _detector.VertexBuffer);
    }

    void OnDestroy()
    {
        _detector.Dispose();
        Destroy(_material);
    }

    void Update()
      => Graphics.DrawProcedural
           (_material, _bounds, MeshTopology.Lines, 64, 1);

    #endregion
}

} // namespace MediaPipe
