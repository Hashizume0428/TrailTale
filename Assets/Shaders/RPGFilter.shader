Shader "UI/RPGFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelSize ("Pixel Size", Float) = 4.0
        _SepiaIntensity ("Sepia Intensity", Range(0,1)) = 0.8
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PixelSize;
            float _SepiaIntensity;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ピクセル化
                float2 pixelUV = floor(i.uv * _PixelSize) / _PixelSize;
                fixed4 col = tex2D(_MainTex, pixelUV);

                // セピア変換
                float3 sepiaColor = float3(
                    dot(col.rgb, float3(0.393, 0.769, 0.189)),
                    dot(col.rgb, float3(0.349, 0.686, 0.168)),
                    dot(col.rgb, float3(0.272, 0.534, 0.131))
                );

                col.rgb = lerp(col.rgb, sepiaColor, _SepiaIntensity);
                return col;
            }
            ENDCG
        }
    }
}
