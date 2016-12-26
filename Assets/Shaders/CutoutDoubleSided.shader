Shader "DustSucker/CutoutDoubleSided" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_CutoutValue ("Cutout Value", Float) = 0.5
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MetallicGloss("Metallic / Gloss (A)", 2D) = "white" {}
		_Normal("Normal map", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0.0
	}
	SubShader {
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 200
		Cull Off

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alphatest:_CutoutValue
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		sampler2D _MetallicGloss;
		sampler2D _Normal;

		//half _CutoutValue;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			half4 metallicGloss = tex2D(_MetallicGloss, IN.uv_MainTex);

			o.Albedo = c.rgb;
			o.Metallic = metallicGloss.r * _Metallic;
			o.Smoothness = metallicGloss.a * _Glossiness;
			o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_MainTex));

			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
