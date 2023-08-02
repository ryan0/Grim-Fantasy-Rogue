Shader "Custom/WaterShimmer"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ShimmerAmount("Shimmer Amount", Range(0,1)) = 0.5
        _ShimmerSpeed("Shimmer Speed", float) = 1.0
        _NoiseScale("Noise Scale", float) = 10.0
        _PixelsPerUnit("Pixels Per Unit", float) = 16.0
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" }
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
                };

                sampler2D _MainTex;
                float _ShimmerAmount;
                float _ShimmerSpeed;
                float _NoiseScale;
                float _PixelsPerUnit;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    float2 pixelUV = i.uv * _PixelsPerUnit;
                    float noiseInput = frac(sin(dot(pixelUV, float2(12.9898,78.233))) * 43758.5453);
                    noiseInput *= _NoiseScale;
                    noiseInput += _Time.y * _ShimmerSpeed;
                    float shimmer = _ShimmerAmount * frac(sin(noiseInput * 6.2831));

                    half4 color = tex2D(_MainTex, i.uv);
                    color.rgb += step(shimmer, 0.5); // Turns individual pixels to white based on threshold

                    return color;
                }
                ENDCG
            }
        }
}
