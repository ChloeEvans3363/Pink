Shader "Hidden/VolFx/Sharpen"
{
    Properties
    {
        _Center("Center Weight", Float) = 5
        _Side("Sample Weight", Float) = -1
        _Color("Color", Color) = (1, 1, 1, 1)
        
		[Toggle(BOX)]_Centered("Box", Float) = 1
    }

    SubShader
    {
        name "Sharpen"
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
            
			#pragma multi_compile_local __ BOX

            sampler2D       _MainTex;
            uniform float2  _Thickness;         // 1 / width, 1 / height
            
            half            _Impact;
            half            _Center;
            half            _Side;
            half4           _Color;

            //half4x4        _kernel; 
            
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
                // Sharpen calculations
                half4 initial = tex2D(_MainTex, i.uv);
                half4 result = initial * _Center;
                
                result += tex2D(_MainTex, i.uv + float2( _Thickness.x, 0)) * _Side;
                result += tex2D(_MainTex, i.uv + float2(-_Thickness.x, 0)) * _Side;
                result += tex2D(_MainTex, i.uv + float2(0, _Thickness.y)) * _Side;
                result += tex2D(_MainTex, i.uv + float2(0,-_Thickness.y)) * _Side;
#ifdef BOX
                result += tex2D(_MainTex, i.uv + float2( _Thickness.x, _Thickness.y)) * _Side;
                result += tex2D(_MainTex, i.uv + float2(-_Thickness.x, _Thickness.y)) * _Side;
                result += tex2D(_MainTex, i.uv + float2( _Thickness.x,-_Thickness.y)) * _Side;
                result += tex2D(_MainTex, i.uv + float2(-_Thickness.x,-_Thickness.y)) * _Side;
#endif

                // Output to screen
                return initial + (result - initial) * _Color;
            }
            ENDHLSL
        }
    }
}