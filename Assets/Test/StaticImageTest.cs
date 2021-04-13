using UnityEngine;
using Unity.Barracuda;
using UI = UnityEngine.UI;

namespace MediaPipe {

public sealed class StaticImageTest : MonoBehaviour
{
    [SerializeField] NNModel _model;
    [SerializeField] Texture2D _image;
    [SerializeField] UI.RawImage _previewUI;
    [SerializeField] RectTransform _markerPrefab;

    void Start()
    {
        _previewUI.texture = _image;

        // Input image -> Tensor (1, 64, 64, 3)
        var source = new float[64 * 64 * 3];

        for (var y = 0; y < 64; y++)
        {
            for (var x = 0; x < 64; x++)
            {
                var i = (y * 64 + x) * 3;
                var p = _image.GetPixel(x, 63 - y);
                source[i + 0] = p.r * 2 - 1;
                source[i + 1] = p.g * 2 - 1;
                source[i + 2] = p.b * 2 - 1;
            }
        }

        // Inference
        var model = ModelLoader.Load(_model);
        using var worker = WorkerFactory.CreateWorker(model);

        using (var tensor = new Tensor(1, 64, 64, 3, source))
            worker.Execute(tensor);

        // Visualization
        var contours = worker.PeekOutput("output_eyes_contours_and_brows");
        var size = ((RectTransform)_previewUI.transform).rect.size;

        for (var i = 0; i < 71; i++)
        {
            var x = (     contours[0, 0, 0, i * 3 + 0]) / 64 * size.x;
            var y = (63 - contours[0, 0, 0, i * 3 + 1]) / 64 * size.y;
            var m = Instantiate(_markerPrefab, _previewUI.transform);
            ((RectTransform)m.transform).anchoredPosition = new Vector2(x, y);
        }

        var iris = worker.PeekOutput("output_iris");

        for (var i = 0; i < 5; i++)
        {
            var x = (     iris[0, 0, 0, i * 3 + 0]) / 64 * size.x;
            var y = (63 - iris[0, 0, 0, i * 3 + 1]) / 64 * size.y;
            var m = Instantiate(_markerPrefab, _previewUI.transform);
            ((RectTransform)m.transform).anchoredPosition = new Vector2(x, y);
        }
    }
}

} // namespace MediaPipe
