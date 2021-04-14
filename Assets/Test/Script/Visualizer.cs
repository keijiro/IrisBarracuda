using UnityEngine;
using UI = UnityEngine.UI;
using System.Linq;

namespace MediaPipe.Iris {

public sealed class Visualizer : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] Texture2D _image = null;
    [SerializeField] WebcamInput _webcam = null;
    [Space]
    [SerializeField] UI.RawImage _previewUI = null;
    [Space]
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] Shader _shader = null;
    [SerializeField] RectTransform _markerPrefab = null;

    #endregion

    #region Private members

    IrisDetector _detector;
    Material _material;
    RectTransform[] _markers;

    void RunDetector(Texture input)
    {
        _detector.ProcessImage(input);

        _material.SetBuffer("_Vertices", _detector.VertexBuffer);

        var scale = ((RectTransform)_previewUI.transform).rect.size;
        _markers[0].anchoredPosition = _detector.GetIrisCenter() * scale;
        _markers[1].anchoredPosition = _detector.GetEyelidLeft() * scale;
        _markers[2].anchoredPosition = _detector.GetEyelidRight() * scale;
        _markers[3].anchoredPosition = _detector.GetEyelidUpper() * scale;
        _markers[4].anchoredPosition = _detector.GetEyelidLower() * scale;

        _previewUI.texture = input;
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _detector = new IrisDetector(_resources);
        _material = new Material(_shader);
        _markers = Enumerable.Range(0, 5)
          .Select(x => Instantiate(_markerPrefab, _previewUI.transform))
          .ToArray();

        if (_image != null) RunDetector(_image);
    }

    void OnDestroy()
    {
        _detector.Dispose();
        Destroy(_material);
    }

    void LateUpdate()
    {
        if (_webcam != null) RunDetector(_webcam.Texture);

        var bounds = new Bounds(Vector3.zero, Vector3.one);
        Graphics.DrawProcedural(_material, bounds, MeshTopology.Lines, 64, 1);
    }

    #endregion
}

} // namespace MediaPipe
