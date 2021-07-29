Shader "Unlit/testerino"
{
    Properties
    {
        _x1 ("X1", Vector) = (0.0, 0.0, 0.0, 0.0)
        _x2 ("X2", Vector) = (0.0, 0.0, 0.0, 0.0)
        _x3 ("X3", Vector) = (0.0, 0.0, 0.0, 0.0)
        _x4 ("X4", Vector) = (0.0, 0.0, 0.0, 0.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Off
            CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _x1;
            float4 _x2;
            float4 _x3;
            float4 _x4;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 n = normalize(i.worldPos.xyz);
                
                float4x3 A = float4x3(_x1.xyz, _x2.xyz, _x3.xyz, _x4.xyz);
                float3x4 At = transpose(A);

                float4 An = mul(A, n);
                float R = dot(An, An);

                float3 gradR = 2.0 * mul(At, An);

                float nSqMag = dot(n, n);

                float3 gradS = (gradR - 2.0*R*n);

                return float4(abs(gradS)/2.0, 1.0);
            }
            ENDCG
        }
    }
}
