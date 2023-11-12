Shader "hahahoho/grid-worldxy"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _LineColor ("Line Color", Color) = (0,0,0,1)
        _ScreenWidth ("Screen Width", float) = 0.1
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {   
        ZWrite Off
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        // #pragma surface surf Standard vertex:vert fullforwardshadows
        #pragma surface surf NoLighting vertex:vert fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float3 worldPos;
        };

        void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
        }
        
        fixed4 _Color;
        fixed4 _LineColor;
        fixed _ScreenWidth;
        half _Glossiness;
        half _Metallic;

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) 
        { 
            fixed4 c; 
            c.rgb = s.Albedo; 
            c.a = s.Alpha; 
            return c; 
        }

        // void surf (Input IN, inout SurfaceOutputStandard o)
        void surf (Input IN, inout SurfaceOutput o)
        {
            // rounding must be after interpolation
            float4 clip_pos = mul(UNITY_MATRIX_VP, float4(IN.worldPos, 1));
            clip_pos.xyz = clip_pos.xyz / clip_pos.w;
            float3 rounded_world_pos = float3(round(IN.worldPos.xy), IN.worldPos.z);
            float4 rounded_clip_pos = mul(UNITY_MATRIX_VP, float4(rounded_world_pos, 1));
            rounded_clip_pos.xyz = rounded_clip_pos.xyz / rounded_clip_pos.w;

            clip_pos.xy = clip_pos.xy * _ScreenParams.xy;
            rounded_clip_pos.xy = rounded_clip_pos.xy * _ScreenParams.xy;
            
            float2 diff3 = abs(rounded_clip_pos.xy - clip_pos.xy);
            float diff = min(diff3.x, diff3.y);
            float step01 = step(diff, _ScreenWidth);
            o.Albedo = lerp(_Color, _LineColor, step01);
            o.Alpha = 1;
            // o.Metallic = _Metallic;
            // o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
