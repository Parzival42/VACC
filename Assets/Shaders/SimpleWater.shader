/* This shader uses a Grab pass for Refraction
 * This shader is not Deferred!
 */
Shader "DustSucker/SimpleWater" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		[NoScaleOffset]
		_GlossMap ("Gloss Map", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0.0

		[Normal][NoScaleOffset]
		_WaterNormalMap ("Water Normal", 2D) = "bump" {}
		_WaterNormalStrength ("Normal Strength", Float) = 1.0

		[NoScaleOffset]
		_Heightmap ("Heightmap", 2D) = "black" {}
		_HeightStrength ("Height Strength", Float) = 1.0

		_SobelStrength ("Normal Strength", Float) = 0.2

		_WaterWaveColor ("Wave Color", Color) = (1, 1, 1, 1)
		_WaveStrength ("Wave Strength", Range(-1, 1)) = 1.0

		[NoScaleOffset]
		_FoamTex("Foam Texture", 2D) = "white" {}
		_FoamBlendOffset("Foam Edge Blending value", Range(0, 1)) = 0.01
		_UpVector ("Up Vector", Vector) = (0, 1, 0)

		_RefractionStrength("Refraction Strength", Float) = 20.0
		_RimStrength("Rim Strength", Range(0, 2)) = 1.0
	}
	Category {
		SubShader {
			GrabPass { "_GrabTexture" }

			Tags { "Queue" = "Transparent"}	// With this, the water is rendered in "Forward Pass"!
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
			#pragma target 3.0
			#include "Includes/TextureLookup.cginc"
			#include "Includes/Util.cginc"
			#include "Includes/Heightmap.cginc"

			struct Input {
				float2 uv_MainTex;
				float2 uv_Heightmap;
				float3 viewDir;
				float4 screenPos;INTERNAL_DATA
			};

			// Sampler
			sampler2D _GrabTexture;
			sampler2D _MainTex;
			sampler2D _Heightmap;
			sampler2D _FoamTex;
			sampler2D _GlossMap;
			sampler2D _WaterNormalMap;

			float4 _Heightmap_TexelSize;
			float4 _GrabTexture_TexelSize;

			fixed4 _Color;
			fixed4 _WaterWaveColor;

			half _WaterNormalStrength;
			half _RimStrength;
			half _RefractionStrength;
			half _FoamBlendOffset;
			half _HeightStrength;
			half _Glossiness;
			half _Metallic;
			half _SobelStrength;
			half _WaveStrength;
			fixed4 _UpVector;

			half3 calculateRefraction(Input IN, half3 surfaceNormal, half4 originalColor) {
				float2 offset = surfaceNormal * _RefractionStrength * _GrabTexture_TexelSize.xy;
				IN.screenPos.xy += offset * IN.screenPos.z;

				fixed3 refractColor = tex2Dproj(_GrabTexture, IN.screenPos);
				half rimValue = 1.0 - calculateRim(IN.viewDir, surfaceNormal, _RimStrength);
				return lerp(originalColor.rgb, refractColor, rimValue);
				//return lerp(originalColor.rgb, refractColor, 1.0 - originalColor.a);
			}

			half4 calculateFoam(Input IN, half3 normalizedNormal, half4 originalWaterColor) {
				half waveStrength = dot(normalizedNormal, _UpVector.xyz);
				_WaveStrength = -_WaveStrength;

				if (waveStrength >= _WaveStrength) {
					// Only sample foam when it is needed
					// TODO: Remove magic numbers
					fixed4 foamTex = SampleMultiUvMix(_FoamTex, IN.uv_MainTex, 10.0, 6.0, 2.0);

					// Lerping calculation between normal material values and foam values
					float offset = _WaveStrength + _FoamBlendOffset;
					float blendValue = saturate(normalizeBetween(_WaveStrength, offset, 1.0 - waveStrength));
					originalWaterColor = lerp(foamTex, originalWaterColor, 1.0 - blendValue);
				}
				return originalWaterColor;
			}

			void surf (Input IN, inout SurfaceOutputStandard o) {
				float offset = _Time[1] * 0.045;
				float2 uv = float2(offset, offset);
				fixed4 waterColor = tex2D (_MainTex, IN.uv_MainTex + uv) * _Color;

				// Get water normals from texture
				float3 waterNormal = UnpackNormal(tex2D(_WaterNormalMap, IN.uv_MainTex + uv));
				waterNormal.xy *= _WaterNormalStrength;

				// Calculate the surface normals based on the heightmap (Sobel-Filter)
				half3 surfaceNormal = height2NormalSobel(img3x3(_Heightmap, IN.uv_Heightmap, 0, _Heightmap_TexelSize.xy));
				o.Normal = normalize(float3(surfaceNormal.xy, surfaceNormal.z * _SobelStrength));

				// Calculate water foam based on the steepness of the water (normal).
				waterColor = calculateFoam(IN, o.Normal, waterColor);

				o.Normal = normalize(o.Normal + waterNormal);

				// Combine the water color with refraction
				half3 colorResult = calculateRefraction(IN, o.Normal, half4(waterColor.rgb, waterColor.a));

				// Set the shader parameters
				o.Albedo = colorResult.rgb;
				o.Metallic = _Metallic;
				o.Smoothness = tex2D (_GlossMap, IN.uv_MainTex) * _Glossiness;
				o.Alpha = waterColor.a;
			}

			void vert (inout appdata_full v) {
				float4 heightmap = tex2Dlod(_Heightmap, float4(v.texcoord.xy, 0, 0));
				float4 pos = mul(unity_ObjectToWorld, v.vertex);

				pos.y += heightmap.r * _HeightStrength;
				v.vertex = mul(unity_WorldToObject, pos);
	      }
		  ENDCG
		}
	}
	FallBack "Diffuse"
}
