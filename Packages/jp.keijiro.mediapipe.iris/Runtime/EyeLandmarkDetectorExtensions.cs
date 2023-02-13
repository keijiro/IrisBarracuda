using UnityEngine;
using System;

namespace MediaPipe.Iris {

//
// Extension helper class for EyeLandmarkDetector
//
public static class EyeLandmarkDetectorExtensions
{
    #region Keypoint accessors

    public static Vector2 GetIrisCenter(this EyeLandmarkDetector detector)
      => detector.VertexArray[0];

    public static Vector2 GetEyelidLeft(this EyeLandmarkDetector detector)
      => detector.VertexArray[5];

    public static Vector2 GetEyelidRight(this EyeLandmarkDetector detector)
      => detector.VertexArray[13];

    public static Vector2 GetEyelidLower(this EyeLandmarkDetector detector)
      => (detector.VertexArray[8] +
          detector.VertexArray[9]) * 0.5f;

    public static Vector2 GetEyelidUpper(this EyeLandmarkDetector detector)
      => (detector.VertexArray[17] +
          detector.VertexArray[18]) * 0.5f;

    #endregion

    #region Partial vertex array accessors

    public static ReadOnlySpan<Vector4>
      IrisVertexArray(this EyeLandmarkDetector detector)
      => detector.VertexArray.Slice(0, 5);

    public static ReadOnlySpan<Vector4>
      LowerEyelidVertexArray(this EyeLandmarkDetector detector)
      => detector.VertexArray.Slice(5, 8);

    public static ReadOnlySpan<Vector4>
      UpperEyelidVertexArray(this EyeLandmarkDetector detector)
      => detector.VertexArray.Slice(13, 7);

    #endregion
}

} // namespace MediaPipe.Iris
