Shader "DustSucker/SimpleTerrain" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0.0

		_Heightmap ("Heightmap", 2D) = "black" {}
		_HeightStrength ("Height Strength", Float) = 1.0

		_SobelStrength ("Normal Strength", Float) = 0.2
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
		#pragma target 3.0
		#include "Includes/Heightmap.cginc"

		struct Input {
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		sampler2D _Heightmap;

		float4 _Heightmap_TexelSize;

		half _HeightStrength;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _SobelStrength;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			float3 normal = height2NormalSobel(img3x3(_Heightmap, IN.uv_MainTex, 0, _Heightmap_TexelSize.xy));
			o.Normal = normalize(float3(normal.xy, normal.z * _SobelStrength));

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}

		void vert (inout appdata_full v) {
			float4 heightmap = tex2Dlod(_Heightmap, float4(v.texcoord.xy, 0, 0));
			float4 pos = mul(unity_ObjectToWorld, v.vertex);

			pos.y += heightmap.r * _HeightStrength;
			v.vertex = mul(unity_WorldToObject, pos);
      }
		ENDCG
	}
	FallBack "Diffuse"
}
