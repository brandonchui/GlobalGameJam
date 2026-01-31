Shader "Custom/CircleDetect"
{
    Properties
    {
        _InsideColor ("Inside Color", Color) = (0, 1, 0, 1)
        _OverlapColor ("Overlap Color (2+ circles)", Color) = (1, 1, 0, 1)
        _CircleCount ("Circle Count", Int) = 0
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
            float4 _OverlapColor;
            int _CircleCount;
            float4 _CircleCenters[10]; // Support up to 10 circles
            float _CircleRadii[10];

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                int insideCount = 0;

                for (int c = 0; c < _CircleCount; c++)
                {
                    float dist = distance(i.worldPos.xy, _CircleCenters[c].xy);
                    if (dist <= _CircleRadii[c])
                    {
                        insideCount++;
                    }
                }

                if (insideCount == 0)
                    discard;

                if (insideCount >= 2)
                    return _OverlapColor;

                return _InsideColor;
            }
            ENDCG
        }
    }
}
