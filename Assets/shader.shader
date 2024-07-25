Shader "shader"
{
    Properties{
        //https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
       [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            StructuredBuffer<float2> verticeBuff;
            StructuredBuffer<float2> gridIndexBuff;
            uniform int uCellSize;
            uniform float unitePortion;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(uint vertexID: SV_VertexID, uint instanceID : SV_InstanceID)
            {
                v2f o;
                const float3x3 isoMat = 
                {
                    float(uCellSize)*0.5f,      -float(uCellSize)*0.5f,      0.0f,
                    -float(uCellSize)*0.25f,    -float(uCellSize)*0.25f,     0.0f,
                    0.0f,                       0.0f,                        1.0f
                };
                float2 uv = mul(uCellSize, verticeBuff[vertexID%6]);
                float2 gridIndex = gridIndexBuff[instanceID];//
                float3 center = mul(isoMat,float3(gridIndex,0.0f));
                float3 offset = mul(uCellSize, float3(-0.5f, -0.5f,0.0f));
               
                float3 pos = float3(uv+center+offset, 0.0f);
                o.pos = mul(UNITY_MATRIX_VP, float4(pos, 1.0f));
                o.uv = verticeBuff[vertexID%6];
                // o.uv = TRANSFORM_TEX(verticeBuff[vertexID%6],_MainTex);
                return o;
            }
            fixed4 frag (v2f i): SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv); // float2(0.025,0.025)
                // fixed4 col = fixed4(uv, 0, 1);
                return col;

            }
            ENDCG
        }
    }
}