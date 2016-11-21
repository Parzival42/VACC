Shader "DustSucker/PaintShader" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("First Texture", 2D) = "white" {}
		[NoScaleOffset]
		_GlossMap ("First Gloss Map (R)", 2D) = "white" {}
		[NoScaleOffset]
		_NormalMap ("Normal map", 2D) = "bump" {}
		_NormalStrength ("Normal Strength", Range(0, 2)) = 1.0
		_Glossiness ("Smoothness", Range(0, 30)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0.0

		_OtherTex ("Other Texture", 2D) = "white" {}
		[NoScaleOffset]
		_OtherGlossMap ("Other Gloss Map (R)", 2D) = "white" {}
		[NoScaleOffset]
		_OtherNormalMap ("Other Normal Map", 2D) = "bump" {}
		_OtherNormalStrength ("Other Normal Strength", Range(0, 2)) = 1.0
		_OtherGlossiness ("Other Smoothness", Range(0, 30)) = 0.5
		_OtherMetallic ("Other Metallic", Range(0, 1)) = 0.0

		[HideInInspector]
		_Mask ("Mask", 2D) = "white" {}
		_MaskStrength ("Mask Strength", Range(0, 1)) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0
		#include "Includes/Heightmap.cginc"

		struct Input {
			float2 uv_MainTex;
			float2 uv_OtherTex;
			float2 uv_Mask;
		};

		sampler2D _MainTex;
		sampler2D _GlossMap;
		sampler2D _Mask;
		sampler2D _NormalMap;
		sampler2D _OtherNormalMap;
		sampler2D _OtherTex;
		sampler2D _OtherGlossMap;

		half _NormalStrength;
		half _OtherNormalStrength;
		half _MaskStrength;
		half _Glossiness;
		half _OtherGlossiness;
		half _Metallic;
		half _OtherMetallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed mask = tex2D(_Mask, IN.uv_Mask).r * _MaskStrength;

			// Main color texture
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 secondaryTex = tex2D(_OtherTex, IN.uv_OtherTex) * _Color;

			// Glossiness
			fixed mainGloss = tex2D(_GlossMap, IN.uv_MainTex).r * _Glossiness;
			fixed secondaryGloss = tex2D(_OtherGlossMap, IN.uv_OtherTex).r * _OtherGlossiness;

			// Normal map
			float3 mainNormal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			mainNormal.xy *= _NormalStrength;
			float3 secondaryNormal = UnpackNormal(tex2D(_OtherNormalMap, IN.uv_OtherTex));
			secondaryNormal.xy *= _OtherNormalStrength;

			o.Albedo = lerp(mainTex, secondaryTex, mask);
			o.Smoothness = lerp(mainGloss, secondaryGloss, mask);
			o.Normal = lerp(mainNormal, secondaryNormal, mask);

			o.Metallic = lerp(_Metallic, _OtherMetallic, mask);
			o.Alpha = mainTex.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
