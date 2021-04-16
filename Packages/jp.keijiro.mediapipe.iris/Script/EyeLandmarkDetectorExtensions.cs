using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MediaPipe.Iris {

//
// Extension helper class for EyeLandmarkDetector
//
public static class EyeLandmarkDetectorExtensions
{
    #region Keypoint accessors

    public static Vector2 GetIrisCenter(this EyeLandmarkDetector detector)
      => detector.VertexArray.ElementAt(0);

    public static Vector2 GetEyelidLeft(this EyeLandmarkDetector detector)
      => detector.VertexArray.ElementAt(5);

    public static Vector2 GetEyelidRight(this EyeLandmarkDetector detector)
      => detector.VertexArray.ElementAt(13);

    public static Vector2 GetEyelidLower(this EyeLandmarkDetector detector)
      => (detector.VertexArray.ElementAt(8) +
          detector.VertexArray.ElementAt(9)) * 0.5f;

    public static Vector2 GetEyelidUpper(this EyeLandmarkDetector detector)
      => (detector.VertexArray.ElementAt(17) +
          detector.VertexArray.ElementAt(18)) * 0.5f;

    #endregion

    #region Partial vertex array accessors

    public static IEnumerable<Vector4>
      IrisVertexArray(this EyeLandmarkDetector detector)
      => detector.VertexArray.Take(5);

    public static IEnumerable<Vector4>
      LowerEyelidVertexArray(this EyeLandmarkDetector detector)
      => detector.VertexArray.Skip(5).Take(8);

    public static IEnumerable<Vector4>
      UpperEyelidVertexArray(this EyeLandmarkDetector detector)
      => detector.VertexArray.Skip(13).Take(7);

    #endregion
}

} // namespace MediaPipe.Iris
