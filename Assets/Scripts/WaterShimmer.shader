Shader "Custom/WaterShimmer"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _ShimmerAmount("Shimmer Amount", Range(0,1)) = 0.5
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
                sampler2D _MaskTex;
                float _ShimmerAmount;
                float _Time;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    half4 color = tex2D(_MainTex, i.uv);
                    half4 mask = tex2D(_MaskTex, i.uv);
                    float shimmer = sin(_Time.y * 2.0) * _ShimmerAmount;
                    color.rgb += mask.rgb * shimmer;
                    return color;
                }
                ENDCG
            }
        }
}
