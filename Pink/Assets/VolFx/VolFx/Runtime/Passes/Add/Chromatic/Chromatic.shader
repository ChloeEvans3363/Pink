Shader "Hidden/VolFx/Chromatic"
{
    Properties
    {
        _Intensity ("Intensity", Range(0.0,1.0)) = 0.001
        _Center ("Center", Range(0.0,0.5)) = 0.0
        _Weight("Weight", Range(0, 1)) = 1
        _Radial("Radial", Range(0, 1)) = 1
        _R ("R", Vector) = (1, 0, 0, 0)
        _G ("G", Vector) = (-1, 0, 0, 0)
        _B ("B", Vector) = (0, 1, 0, 0)
    }

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always
        ZClip false
            
        Pass
        {
            Name "Chromatic"
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag


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

            uniform sampler2D _MainTex;
            fixed _Intensity;
            fixed _Center;
            fixed _Weight;
            fixed _Radial;
            fixed _Alpha;
            float2 _R;
            float2 _G;
            float2 _B;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

            half4 sample(float mov, float2 uv)
            {
                float2 uvR = uv + _R * mov;
                float2 uvG = uv + _G * mov;
                float2 uvB = uv + _B * mov;
                
                half4 colR = tex2D(_MainTex, uvR);
                half4 colG = tex2D(_MainTex, uvG);
                half4 colB = tex2D(_MainTex, uvB);
                
                return half4(colR.r, colG.g, colB.b, (colR.a + colG.a + colB.a) * _Alpha);
            }

            fixed4 frag(v2f i) : COLOR
            {
                fixed4 result;

                float mov = _Intensity + pow(distance(float2(.5f, .5f), i.uv), 3) * _Radial;
                result = sample(mov, i.uv);

                return lerp(tex2D(_MainTex, i.uv), result, _Weight);
            }
            ENDCG
        }
    }
}