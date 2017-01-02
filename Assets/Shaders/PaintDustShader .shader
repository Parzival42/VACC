Shader "DustSucker/PaintDustShader" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("First Texture", 2D) = "white" {}
		[NoScaleOffset]
		_GlossMap ("First Gloss Map (R)", 2D) = "white" {}
		[NoScaleOffset]
		_NormalMap ("Normal map", 2D) = "bump" {}
		[NoScaleOffset]
		_AO ("AO", 2D) = "white" {}
		_NormalStrength ("Normal Strength", Range(0, 2)) = 1.0
		_Glossiness ("Smoothness", Range(0, 30)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0.0

		_OtherTexture ("Other Texture", 2D) = "white" {}
		[NoScaleOffset]
		_OtherTexGloss ("Other Gloss Texture", 2D) = "black" {}
		[NoScaleOffset]
		_OtherTexNormal ("Other Normal Texture", 2D) = "bump" {}
		_OtherNormalStrength ("Other Normal Strength", Range(0, 2)) = 1.0
		_OtherGlossiness ("Other Smoothness", Range(0, 1)) = 0.5
		_OtherMetallic ("Other Metallic", Range(0, 1)) = 0.0

		[HideInInspector]
		_Mask ("Mask", 2D) = "white" {}
		_MaskStrength ("Mask Strength", Range(0, 1)) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque"}
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
			float2 uv_Mask;
			float2 uv_OtherTexture;
		};

		sampler2D _MainTex;
		sampler2D _GlossMap;
		sampler2D _Mask;
		sampler2D _NormalMap;
		sampler2D _AO;
		sampler2D _OtherTexture;
		sampler2D _OtherTexGloss;
		sampler2D _OtherTexNormal;

		half _OtherNormalStrength;
		half _NormalStrength;
		half _MaskStrength;
		half _Glossiness;
		half _OtherGlossiness;
		half _Metallic;
		half _OtherMetallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed mask = tex2D(_Mask, IN.uv_Mask).r * _MaskStrength;
			half4 otherTex = tex2D(_OtherTexture, IN.uv_OtherTexture);
			half4 otherNormal = tex2D(_OtherTexNormal, IN.uv_OtherTexture);
			otherNormal.xy *= _OtherNormalStrength;
			half otherGloss = tex2D(_OtherTexGloss, IN.uv_OtherTexture).r * _OtherGlossiness;

			// Main color texture
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			// Glossiness
			fixed mainGloss = tex2D(_GlossMap, IN.uv_MainTex).r * _Glossiness;
			// Normal map
			float3 mainNormal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			mainNormal.xy *= _NormalStrength;

			half mainOcclusion = tex2D(_AO, IN.uv_MainTex).r;

			o.Albedo = lerp(mainTex, otherTex, mask);
			//o.Occlusion = lerp(mainOcclusion, 0, mask);
			o.Smoothness = lerp(mainGloss, otherGloss, mask);
			o.Occlusion = mainOcclusion;
			//o.Smoothness = mainGloss;
			o.Normal = lerp(mainNormal, otherNormal, mask);

			o.Metallic = lerp(_Metallic, _OtherMetallic, mask);
			//o.Alpha = mainTex.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
