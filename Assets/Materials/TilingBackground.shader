Shader "Custom/TilingBackground"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _TileScale ("Tile Scale", Vector) = (1, 1, 0, 0)
        _ScrollSpeed ("Scroll Speed", Vector) = (0, 0, 0, 0)
        _ParallaxFactor ("Parallax Factor", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Opaque" }
        LOD 100

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float4 _TileScale;
            float4 _ScrollSpeed;
            float _ParallaxFactor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Get camera position for parallax
                float2 camPos = _WorldSpaceCameraPos.xy * _ParallaxFactor;

                // Calculate tiled UVs based on world position
                float2 tiledUV = (i.worldPos - camPos) * _TileScale.xy;

                // Add scrolling
                tiledUV += _Time.y * _ScrollSpeed.xy;

                // Sample texture with tiled UVs
                fixed4 col = tex2D(_MainTex, tiledUV);
                return col * _Color;
            }
            ENDCG
        }
    }
}
