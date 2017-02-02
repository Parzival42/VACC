Shader "DustSucker/LineUI" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Emission ("Emission Color", Color) = (1,1,1,1)
		_EmissionStrength ("Emission Strength", Float) = 0
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_GlossinessTex ("Smoothness Tex", 2D) = "white" {}
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MetallicTex ("Metallic Tex", 2D) = "white" {}
		_NormalTex ("Normal Tex", 2D) = "bump" {}

		_Highlight ("Highlight Color", Color) = (1,1,1,1)
		_LineColorMultiplier ("Highlight Color Multiplier", Float) = 1.0
		[Toggle] _IsHighlighted("Is Highlighted", Float) = 0
		_LineStrength ("Line Strength", Float) = 200
		_LineSpace ("Line Space", Float) = 4
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _GlossinessTex;
		sampler2D _MetallicTex;
		sampler2D _NormalTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float4 screenPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _Emission;
		fixed4 _Highlight;
		half _LineColorMultiplier;
		half _EmissionStrength;
		half _IsHighlighted;
		half _LineStrength;
		half _LineSpace;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = tex2D (_MetallicTex, IN.uv_MainTex) * _Metallic;
			o.Smoothness = tex2D (_GlossinessTex, IN.uv_MainTex) * _Glossiness;
			o.Alpha = c.a;
			o.Emission = _Emission.rgb * _EmissionStrength;

			o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex));

			float dist = distance(IN.worldPos.xz, half2(0,0));
			float2 uv = (IN.screenPos.xy/IN.screenPos.w);

			if(_IsHighlighted){
				if(fmod(floor(_LineStrength * (uv.x + uv.y + _Time.x)), _LineSpace) == 0)
					o.Albedo = _Highlight.rgb * _LineColorMultiplier;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}
