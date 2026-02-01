Shader "Custom/Reveal2D"
{
    Properties
    {

        _CircleIndex ("Circle Index", Int) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
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
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR; // Vertex color field
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 worldXY : TEXCOORD1;
                fixed4 color : COLOR; // Color passed to fragment shader
            };

            sampler2D _MainTex;
            float4 _Color; // Define the variable for the color property

            int _CircleIndex;
            int _CircleCount;
            float4 _CircleCenters[10];
            float _CircleRadii[10];

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldXY = mul(unity_ObjectToWorld, v.vertex).xy;
                o.uv = v.uv;
                o.color = v.color; // Pass the vertex color
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (_CircleIndex < 0 || _CircleIndex >= _CircleCount)
                    discard;

                float2 center = _CircleCenters[_CircleIndex].xy;
                float radius = _CircleRadii[_CircleIndex];

                if (distance(i.worldXY, center) > radius)
                    discard;

                fixed4 c = tex2D(_MainTex, i.uv) * i.color;
                return fixed4(c.rgb * 3.0f, c.a);
            }
            ENDCG
        }
    }
}
