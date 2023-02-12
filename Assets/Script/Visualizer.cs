using UnityEngine;
using UnityEngine.UI;
using Klak.TestTools;
using MediaPipe.Iris;

public sealed class Visualizer : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] ImageSource _source = null;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] RawImage _previewUI = null;
    [SerializeField] Shader _shader = null;
    [SerializeField] RectTransform _markerPrefab = null;

    #endregion

    #region Private members

    EyeLandmarkDetector _detector;
    RectTransform[] _markers = new RectTransform[5];
    Material _material;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // Eye landmark detector initialization
        _detector = new EyeLandmarkDetector(_resources);

        // Marker population
        for (var i = 0; i < _markers.Length; i++)
            _markers[i] = Instantiate(_markerPrefab, _previewUI.transform);

        // Shader initialization
        _material = new Material(_shader);
    }

    void OnDestroy()
    {
        _detector.Dispose();
        Destroy(_material);
    }

    void LateUpdate()
    {
        // Eye landmark detection
        _detector.ProcessImage(_source.Texture);

        // Marker update
        var scale = ((RectTransform)_previewUI.transform).rect.size;
        _markers[0].anchoredPosition = _detector.GetIrisCenter() * scale;
        _markers[1].anchoredPosition = _detector.GetEyelidLeft() * scale;
        _markers[2].anchoredPosition = _detector.GetEyelidRight() * scale;
        _markers[3].anchoredPosition = _detector.GetEyelidUpper() * scale;
        _markers[4].anchoredPosition = _detector.GetEyelidLower() * scale;

        // Contour line draw
        var bounds = new Bounds(Vector3.zero, Vector3.one);
        _material.SetBuffer("_Vertices", _detector.VertexBuffer);
        Graphics.DrawProcedural(_material, bounds, MeshTopology.Lines, 64, 1);

        // UI update
        _previewUI.texture = _source.Texture;
    }

    #endregion
}
