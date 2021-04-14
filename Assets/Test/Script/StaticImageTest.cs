using UnityEngine;
using UI = UnityEngine.UI;
using System.Linq;

namespace MediaPipe.Iris {

public sealed class StaticImageTest : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] ResourceSet _resources = null;
    [SerializeField] Texture2D _image = null;
    [SerializeField] UI.RawImage _previewUI = null;
    [SerializeField] Shader _shader = null;
    [SerializeField] RectTransform _markerPrefab = null;

    #endregion

    #region Private members

    IrisDetector _detector;
    Material _material;
    Bounds _bounds = new Bounds(Vector3.zero, Vector3.one);

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _previewUI.texture = _image;

        _detector = new IrisDetector(_resources);
        _detector.ProcessImage(_image);

        _material = new Material(_shader);
        _material.SetBuffer("_Vertices", _detector.VertexBuffer);

        var size = ((RectTransform)_previewUI.transform).rect.size;

        Instantiate(_markerPrefab, _previewUI.transform)
          .anchoredPosition = _detector.GetIrisCenter() * size;

        Instantiate(_markerPrefab, _previewUI.transform)
          .anchoredPosition = _detector.GetEyelidLeft() * size;

        Instantiate(_markerPrefab, _previewUI.transform)
          .anchoredPosition = _detector.GetEyelidRight() * size;

        Instantiate(_markerPrefab, _previewUI.transform)
          .anchoredPosition = _detector.GetEyelidUpper() * size;

        Instantiate(_markerPrefab, _previewUI.transform)
          .anchoredPosition = _detector.GetEyelidLower() * size;
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
