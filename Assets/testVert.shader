
Shader "testVert"
{

    Properties{
        //https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
       [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            ZTest LEqual
            CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
            #pragma exclude_renderers gles
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR0;
                float2 uv:TEXCOORD0;
            };
            struct indexData
            {
                float2 pos;
                float4 boundary;
                float2 blank;
            };

            StructuredBuffer<float2> verticeBuff;

            StructuredBuffer<indexData> gridIndexBuff;

        

            sampler2D _MainTex;

            v2f vert(uint vertexID: SV_VertexID, uint instanceID : SV_InstanceID)
            {
                v2f o;
                const float destCellSize=64.0f;
                const float srcCellSize = 64.0f;
                const float3x3 isoMat = 
                {
                    float(destCellSize)*0.5f,      -float(destCellSize)*0.5f,      0.0f,
                    -float(destCellSize)*0.25f,    -float(destCellSize)*0.25f,     0.0f,
                    0.0f,                       0.0f,                        1.0f
                };
                //float2x3 
                indexData vertdata = gridIndexBuff[instanceID];
                // float2 gridIndex = gridIndexBuff[instanceID];
                //float4 vBoundary = float4(0,0,1.0f,1.0f);

                float2 gridIndex = float2(vertdata.pos[0], vertdata.pos[1]);
                float4 vBoundary = float4(vertdata.boundary[0], vertdata.boundary[1],vertdata.boundary[2], vertdata.boundary[3]);
                
                float2 uv = verticeBuff[vertexID%6];
                float3 center = mul(isoMat,float3(gridIndex,0.0f));
                float3 offset = float3(-0.5f*destCellSize, -0.5f*destCellSize,0.0f);
                float3 pos = float3(float2(uv[0]*destCellSize, uv[1]*destCellSize)+center+offset, 0.0f);

                
                float2 imgSize = float2(832.0f,832.0f);
                float2 uvStart = float2(vBoundary[0]*srcCellSize/imgSize[0], vBoundary[1]*srcCellSize/imgSize[1]);
                float2 uvEnd = float2((vBoundary[0]+vBoundary[2])*srcCellSize/imgSize[0], (vBoundary[1]+vBoundary[3])*srcCellSize/imgSize[1]);

                o.uv =   float2(uvStart.x+(uvEnd.x-uvStart.x)*uv.x, 1.0-uvStart.y+(uvEnd.y-uvStart.y)*uv.y);
                
                o.pos = mul(UNITY_MATRIX_VP, float4(pos, 1.0f));
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}