// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "DustSucker/TerrainOffset" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_Heightmap ("Heightmap", 2D) = "black" {}
		_HeightStrength ("Height Strength", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.0


		struct Input {
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		sampler2D _Heightmap;

		half _HeightStrength;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}

		void vert (inout appdata_full v) {
			float4 heightmap = tex2Dlod(_Heightmap, float4(v.texcoord.xy, 0, 0));
			float4 pos = mul(unity_ObjectToWorld, v.vertex);

			pos.y += heightmap.r * _HeightStrength;
			v.vertex = mul(unity_WorldToObject, pos);

			//v.vertex.z += heightmap.r * _HeightStrength;
      }
		ENDCG
	}
	FallBack "Diffuse"
}
