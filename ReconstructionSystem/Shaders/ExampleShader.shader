      Shader "ExampleShader"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

          

            struct pointt
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR0;
            };

            StructuredBuffer<pointt> _Positions;
            

            pointt vert(uint vertexID: SV_VertexID, uint instanceID : SV_InstanceID)
            {
                pointt o;
                
                float3 pos = _Positions[vertexID].pos;
                float4 wpos = mul(_ObjectToWorld, float4(pos + float3(instanceID, 0, 0), 1.0f));
                o.pos = mul(UNITY_MATRIX_VP, wpos);
                o.color = float4(instanceID / _NumInstances, 0.0f, 0.0f, 0.0f);
                return o;
            }
             
            float4 frag(pointt o) : SV_Target
            {
                return o.color;
            }
            ENDCG
        }
    }
}