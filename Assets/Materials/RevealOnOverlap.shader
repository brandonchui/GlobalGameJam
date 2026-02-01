Shader "Custom/RevealOnOverlap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1, 0, 1, 1) // Purple by default
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 worldPos : TEXCOORD1;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            // Global circle data from CircleManager
            int _CircleCount;
            float4 _CircleCenters[10];
            float _CircleRadii[10];

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Need at least 2 circles for overlap
                if (_CircleCount < 2)
                    discard;

                // Check if inside circle 0
                float dist0 = distance(i.worldPos, _CircleCenters[0].xy);
                bool insideCircle0 = dist0 <= _CircleRadii[0];

                // Check if inside circle 1
                float dist1 = distance(i.worldPos, _CircleCenters[1].xy);
                bool insideCircle1 = dist1 <= _CircleRadii[1];

                // Only render if inside BOTH circles (overlap region)
                if (!insideCircle0 || !insideCircle1)
                    discard;

                // Sample texture and apply color tint
                fixed4 col = tex2D(_MainTex, i.uv);
                return col * _Color * i.color;
            }
            ENDCG
        }
    }
}
