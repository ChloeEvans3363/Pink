Shader "Hidden/VolFx/Scanlines"
{
    Properties
    {
        _Scanlines("_Scanlines", Vector) = (400, 1.3, .2, .03)
    }

    SubShader
    {
        name "Scanlines"
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 0

        ZTest Always
        ZWrite Off
        ZClip false
        Cull Off
        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            
            Texture2D    _MainTex;
            SamplerState _point_clamp_sampler;
            
            float4       _Scanlines;
            
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
                    
            half4 frag (frag_in i) : SV_Target
            {                
                float4 col = _MainTex.Sample(_point_clamp_sampler, i.uv);

                i.uv.y += _Scanlines.w / _Scanlines.x * 6;
                float2 sl = float2(sin(i.uv.y * _Scanlines.x), cos(i.uv.y * _Scanlines.x));
	            float3 scanlines = float3(sl.x, sl.y, sl.x);

                col.rgb += col.rgb * scanlines * _Scanlines.y;
                col.rgb += col.rgb * _Scanlines.z;
                
                // Output to screen
                return col;
            }
            ENDHLSL
        }
    }
}