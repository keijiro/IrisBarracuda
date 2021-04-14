Shader "Hidden/MediaPipe/Iris/Visualizer"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    StructuredBuffer<float4> _Vertices;

    void Vertex(uint vid : SV_VertexID,
                out float4 position : SV_Position,
                out float4 color : COLOR)
    {
        if (vid < 32)
        {
            const int indices[] =
            {
                0,  1,  1,  2,  2,  3,  3,  4,  4,  5,  5,  6,  6, 7, 7, 8,
                8, 15, 15, 14, 14, 13, 13, 12, 12, 11, 11, 10, 10, 9, 9, 0
            };

            float2 p = _Vertices[indices[vid] + 5].xy - 0.5;

            position = UnityObjectToClipPos(float4(p, 0, 1));
            color = float4(0, 1, 1, 1);
        }
        else
        {
            float2 c = _Vertices[0].xy;
            float r = distance(_Vertices[1].xy, _Vertices[3].xy) / 2;

            float phi = UNITY_PI * 2 * (vid / 2 + (vid & 1) - 16) / 15;
            float2 p = c + float2(cos(phi), sin(phi)) * r - 0.5;

            position = UnityObjectToClipPos(float4(p, 0, 1));
            color = float4(1, 1, 0, 1);
        }
    }

    float4 Fragment(float4 position : SV_Position,
                    float4 color : COLOR) : SV_Target
    {
        return color;
    }

    ENDCG

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}
