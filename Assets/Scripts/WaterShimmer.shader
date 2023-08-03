Shader "Custom/WaterShimmer"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ReflectionTex("Reflection Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _ShimmerAmount("Shimmer Amount", Range(0,1)) = 0.5
        _ReflectionAmount("Reflection Amount", Range(0,1)) = 0.5
        _ShimmerSpeed("Shimmer Speed", float) = 1.0
        _DistortionScale("Distortion Scale", float) = 0.1
        _NoiseScale("Noise Scale", float) = 10.0
        _PixelsPerUnit("Pixels Per Unit", float) = 16.0
        _WorldRight("World Right", Vector) = (1,0,0,0)
        _WorldUp("World Up", Vector) = (0,1,0,0)
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
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                sampler2D _MainTex;
                sampler2D _ReflectionTex;
                sampler2D _NoiseTex;
                float _ShimmerAmount;
                float _ReflectionAmount;
                float _ShimmerSpeed;
                float _DistortionScale;
                float _NoiseScale;
                float _PixelsPerUnit;
                float4 _WorldRight;
                float4 _WorldUp;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv * _PixelsPerUnit;
                    return o;
                }

half4 frag(v2f i) : SV_Target
{
    float2 pixelUV = i.uv / _PixelsPerUnit;

    // Continuous ripple distortion
    float timeEffect = frac(_Time.y * _ShimmerSpeed); // Wraps the timeEffect in a range of 0-1
    float2 noiseCoord = pixelUV * 0.1 + float2(timeEffect, timeEffect);
    float2 noiseOffset = tex2D(_NoiseTex, noiseCoord).rg * 2.0 - 1.0;
    noiseOffset *= _DistortionScale;
    pixelUV += noiseOffset;

    float noiseInput = frac(sin(dot(pixelUV * 0.1, float2(12.9898,78.233))) * 43758.5453);
    noiseInput *= _NoiseScale;
    float shimmer = smoothstep(0.45, 0.55, sin(noiseInput * 6.2831));

    half4 color = tex2D(_MainTex, pixelUV);
    color.rgb += shimmer * _ShimmerAmount;

    float3 worldPos = mul(unity_ObjectToWorld, i.vertex).xyz;
    float3 toCamera = _WorldSpaceCameraPos - worldPos;
    float2 reflectionUV = float2(dot(toCamera, _WorldRight), dot(toCamera, _WorldUp));
    reflectionUV /= _PixelsPerUnit;
    reflectionUV *= float2(0.1, 0.1);
    reflectionUV = frac(reflectionUV); // Wraps the UV coordinates
    reflectionUV += noiseOffset; // Apply the same distortion to reflection
    half4 reflectionColor = tex2D(_ReflectionTex, reflectionUV);

    color.rgb = lerp(color.rgb, reflectionColor.rgb, _ReflectionAmount);

    return color;
}

                ENDCG
            }
        }
}
