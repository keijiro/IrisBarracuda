using UnityEngine;
using UI = UnityEngine.UI;

namespace MediaPipe {

public sealed class WebcamTest : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] Iris.ResourceSet _resources = null;
    [SerializeField] WebcamInput _webcam = null;
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
        _detector = new Iris.IrisDetector(_resources);
        _material = new Material(_shader);
    }

    void OnDestroy()
    {
        _detector.Dispose();
        Destroy(_material);
    }

    void LateUpdate()
    {
        _detector.ProcessImage(_webcam.Texture);

        _material.SetBuffer("_Vertices", _detector.VertexBuffer);
        Graphics.DrawProcedural(_material, _bounds, MeshTopology.Lines, 64, 1);

        _previewUI.texture = _webcam.Texture;
    }

    #endregion
}

} // namespace MediaPipe
