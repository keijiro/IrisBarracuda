using UnityEngine;
using Unity.Barracuda;
using UI = UnityEngine.UI;

namespace MediaPipe {

public sealed class StaticImageTest : MonoBehaviour
{
    [SerializeField] Iris.ResourceSet _resources = null;
    [SerializeField] Texture2D _image = null;
    [SerializeField] UI.RawImage _previewUI = null;
    [SerializeField] RectTransform _markerPrefab = null;

    void Start()
    {
        _previewUI.texture = _image;

        using var detector = new Iris.IrisDetector(_resources);
        detector.ProcessImage(_image);

        var size = ((RectTransform)_previewUI.transform).rect.size;

        foreach (var v in detector.VertexArray)
        {
            var m = Instantiate(_markerPrefab, _previewUI.transform);
            ((RectTransform)m.transform).anchoredPosition
              = new Vector2(v.x, v.y) * size;
        }
    }
}

} // namespace MediaPipe
