Shader "DustSucker/PaintDustShader" {
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

		_OtherGlossiness ("Other Smoothness", Range(0, 30)) = 0.5
		_OtherMetallic ("Other Metallic", Range(0, 1)) = 0.0

		_Tint ("Tint", Color) = (1,1,1,1)
		_TimeFactor ("TimeFactor", Range(0,1)) = 0.0
		_Scale ("Scale", Float) = 1.5
		_DispBias ("Displacement Bias", Float) = 30
		_DispScale ("Displacement Scale", Float) = 10

		[HideInInspector]
		_Mask ("Mask", 2D) = "white" {}
		_MaskStrength ("Mask Strength", Range(0, 1)) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque"}
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.0
		#include "Includes/Heightmap.cginc"
		#include "Includes/Noise.cginc"

		struct Input {
			float2 uv_MainTex;
			float2 uv_Mask;
			float3 worldNormal;
			float3 viewDir;
			float4 screenPos;
			float3 texCoord3D;
		};

		sampler2D _MainTex;
		sampler2D _GlossMap;
		sampler2D _Mask;
		sampler2D _NormalMap;

		half _NormalStrength;
		half _MaskStrength;
		half _Glossiness;
		half _OtherGlossiness;
		half _Metallic;
		half _OtherMetallic;
		fixed4 _Color;

		half _TimeFactor;
		fixed4 _Tint;
		half _Scale;
		half _DispBias;
		half _DispScale;
		half3 _TexCoord3D;

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			half4 pos = mul(unity_ObjectToWorld, v.vertex);
			v.vertex = mul(unity_WorldToObject, pos);
			o.worldNormal = mul(unity_ObjectToWorld, v.normal);
			o.texCoord3D =  _Scale * ( v.vertex.xyz + half3( 0.0, -_Time.y*_Scale*_TimeFactor, 0.0) );

			// Displacement
			//half n = heightMap(o.texCoord3D);
			//half3 texturePosition = half4( half3( 1.5 - n, 1.0 - n, 0.5 - n ), 1.0 ).xyz;
			//float dispFactor = _DispScale * texturePosition.x + _DispBias;
			//float4 dispPos = float4( v.normal.xyz * dispFactor, 0.0 ) + pos;
			//v.vertex = mul(unity_WorldToObject, dispPos);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			// Noise
			half n = heightMap(IN.texCoord3D);
			fixed4 c = half4( n, n, n, n ) * _Tint;

			fixed mask = tex2D(_Mask, IN.uv_Mask).r * _MaskStrength;

			// Main color texture
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			// Glossiness
			fixed mainGloss = tex2D(_GlossMap, IN.uv_MainTex).r * _Glossiness;
			// Normal map
			float3 mainNormal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			mainNormal.xy *= _NormalStrength;

			o.Albedo = lerp(mainTex, lerp(mainTex, c, mask), c.a);
			o.Smoothness = lerp(mainGloss, _OtherGlossiness, mask);
			o.Normal = mainNormal;

			o.Metallic = lerp(_Metallic, _OtherMetallic, mask);
			o.Alpha = mainTex.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
