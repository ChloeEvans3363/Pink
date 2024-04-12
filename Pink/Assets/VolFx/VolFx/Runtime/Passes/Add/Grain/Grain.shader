Shader "Hidden/VolFx/Grain"
{
    Properties
    {
        _Intensity("Intensity", Float) = 1
        _Response("Response", Float) = 256
        _Tint("Tint", Color) = (1, 1, 1, 1)
        _Tiling("Tiling", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 0

        ZTest Always
        ZWrite Off
        ZClip false
        Cull Off

        Pass
        {
            name "Grain"
            
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            float4 _Grain;
            float4 _Tiling;
            float4 _Tint;
            float4 _Adj;    // x - hue, y - saturation, z - brightness, w - alpha

            Texture2D    _MainTex;
            Texture2D    _GrainTex;
            Texture2D    _ResponseTex;
            SamplerState _point_repeat_sampler;

            struct vert_in
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct frag_in
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            frag_in vert (vert_in v)
            {
                frag_in o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }
            
            half luma(half3 rgb)
            {
                return dot(rgb.rgb, half3(0.299, 0.587, 0.114));
            }
            
            inline half3 applyHue(half3 aColor, half3 aHue)
            {
                half angle = aHue;
                half3 k = float3(0.57735, 0.57735, 0.57735);
                half cosAngle = cos(angle);
                
                //Rodrigues' rotation formula
                return aColor * cosAngle + cross(k, aColor) * sin(angle) + k * dot(k, aColor) * (1 - cosAngle);
            }

            inline half3 applyHSBEffect(half3 initial)
            {
                half3 result = initial;
                result.rgb = applyHue(result.rgb, _Adj.x);
                //result.rgb = (result.rgb - 0.5f) * (_Adj.w) + 0.5f;
                result.rgb = result.rgb + _Adj.z;
                result.rgb = lerp(luma(result.rgb), result.rgb, _Adj.y);
                return result;
            }

            half4 frag (frag_in i) : SV_Target
            {
                // pixel color
                half4 source = _MainTex.Sample(_point_repeat_sampler, i.uv);

                half grain = _GrainTex.Sample(_point_repeat_sampler, i.uv * _Tiling.xy + _Tiling.zw).w;
                grain = max(grain - _Grain.x, 0) * _Grain.y;
                half res = _ResponseTex.Sample(_point_repeat_sampler, float2(dot(source.rgb, half3(0.299, 0.587, 0.114)), 0));
                
                return half4(lerp(source.rgb, _Tint.rgb * applyHSBEffect(source.rgb), grain * res), source.a - _Adj.w * grain);
            }
            ENDHLSL
        }
    }
}