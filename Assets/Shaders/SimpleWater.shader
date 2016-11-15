// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "DustSucker/SimpleWater" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0.0

		_Heightmap ("Heightmap", 2D) = "black" {}
		_HeightStrength ("Height Strength", Float) = 1.0

		_SobelStrength ("Normal Strength", Float) = 0.2

		_WaterWaveColor ("Wave Color", Color) = (1, 1, 1, 1)
		_WaveStrength ("Wave Strength", Range(-1, 1)) = 1.0
		_FoamTex("Foam Texture", 2D) = "white" {}
		_FoamBlendOffset("Foam Edge Blending value", Range(0, 1)) = 0.01
		_UpVector ("Up Vector", Vector) = (0, 1, 0)
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
			float3 worldNormal; INTERNAL_DATA
		};

		sampler2D _MainTex;
		sampler2D _Heightmap;
		sampler2D _FoamTex;

		float4 _Heightmap_TexelSize;

		fixed4 _Color;
		fixed4 _WaterWaveColor;

		half _FoamBlendOffset;
		half _HeightStrength;
		half _Glossiness;
		half _Metallic;
		half _SobelStrength;
		half _WaveStrength;
		fixed4 _UpVector;

		/*
		 * Normalizes the given value between the minimum and maximum boundary.
		 * Returns a float between 0 and 1.
		 */
		float normalizeBetween(float min, float max, float value) {
			return (value - min) / (max - min);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 foamTex = tex2D(_FoamTex, IN.uv_MainTex * 10.0) * _WaterWaveColor;
			o.Albedo = c.rgb;

			float3 normal = height2NormalSobel(img3x3(_Heightmap, IN.uv_MainTex, 0, _Heightmap_TexelSize.xy));
			o.Normal = normalize(float3(normal.xy, normal.z * _SobelStrength));

			float waveStrength = dot(o.Normal, _UpVector.xyz);

			if (waveStrength >= _WaveStrength) {
				// Lerping calculation between normal material values and foam values
				float offset = _WaveStrength + _FoamBlendOffset;
				float blendValue = saturate(normalizeBetween(_WaveStrength, offset, 1.0 - waveStrength));
				o.Albedo = lerp(foamTex, c, 1.0 - blendValue);

				o.Metallic = lerp(0.0, 1.0, 1.0 - blendValue);
				o.Smoothness = lerp(0.0, 1.0, 1.0 - blendValue);
			} else {
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
			}

			o.Alpha = c.a;
		}

		void vert (inout appdata_full v) {
			float4 heightmap = tex2Dlod(_Heightmap, float4(v.texcoord.xy, 0, 0));
			float4 pos = mul(unity_ObjectToWorld, v.vertex);
			//_UpVector = mul(unity_WorldToObject, _UpVector);

			pos.y += heightmap.r * _HeightStrength;
			v.vertex = mul(unity_WorldToObject, pos);
      }
		ENDCG
	}
	FallBack "Diffuse"
}
