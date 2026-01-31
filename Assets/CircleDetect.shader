Shader "Custom/CircleDetect"
{
    Properties
    {
        _InsideColor ("Inside Color", Color) = (0, 1, 0, 1)
        _OutsideColor ("Outside Color", Color) = (1, 0, 0, 1)
        _CircleCenter ("Circle Center", Vector) = (0, 0, 0, 0)
        _CircleRadius ("Circle Radius", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _InsideColor;
            float4 _OutsideColor;
            float4 _CircleCenter;
            float _CircleRadius;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = distance(i.worldPos.xy, _CircleCenter.xy);
                if (dist > _CircleRadius)
                    discard; // Make outside pixels invisible
                return _InsideColor;
            }
            ENDCG
        }
    }
}
