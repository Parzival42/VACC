Shader "DustSucker/CombinedWaterTerrain" {
	Properties {
		_TerrainColor ("Terrain Color", Color) = (1, 1, 1, 1)
		_TerrainTexture ("Terrain Texture", 2D) = "white" {}
		[Normal][NoScaleOffset]
		_TerrainNormalMap ("Terrain Normal", 2D) = "bump" {}
		_TerrainNormalStrength ("Terrain Normal Strength", Float) = 1.0
		_TerrainGloss ("Terrain Smoothness", Range(0, 1)) = 0.5
		_TerrainMetallic ("Terrain Metallic", Range(0, 1)) = 0.0

		_WaterColor ("Water Color", Color) = (1, 1, 1, 1)
		_WaterDeepColor ("Water Deep Ground Color", Color) = (1, 1, 1, 1)
		_WaterDepthBias ("Water Depth Bias", Float) = 1.0
		_WaterFoamColor ("Foam Color", Color) = (1, 1, 1, 1)
		_WaterFoamStrength ("Foam Strength", Float) = 1.0
		[Normal]
		_WaterNormalMap ("Water Normal", 2D) = "bump" {}
		_WaterNormalStrength ("Normal Strength", Float) = 1.0
		_WaterGloss ("Water Gloss", Range(0, 1)) = 0.7
		_WaterMetallic ("Water Metallic", Range(0, 1)) = 0.0
		_WaterFresnelPower ("Water Fresnel Power", Float) = 1.0

		// Fill the heights per script!
		[HideInInspector]
		_WaterHeight ("Water Height", 2D) = "black" {}
		[HideInInspector]
		_TerrainHeight ("Terrain Height", 2D) = "black" {}
		[HideInInspector]
		_CombinedHeight ("Combined Water Terrain height", 2D) = "black" {}
		[HideInInspector]
		_FluxLeft ("Flux Left", 2D) = "black" {}
		[HideInInspector]
		_FluxRight ("Flux Right", 2D) = "black" {}
		[HideInInspector]
		_FluxTop ("Flux Top", 2D) = "black" {}
		[HideInInspector]
		_FluxBottom ("Flux Bottom", 2D) = "black" {}
		_SobelStrength ("Normal Strength", Float) = 0.21
		_HeightStrength ("Height Strength", Float) = 6.0

		_TessellationFactor ("Tessellation", Range(1, 32)) = 3.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert tessellate:tessFixed addshadow
		#pragma target 4.6
		#include "../Includes/Heightmap.cginc"
		#include "../Includes/Util.cginc"

		// Needed for Tessellation
		struct appdata {
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};

		struct Input {
			float2 uv_TerrainTexture;
			float2 uv_WaterHeight;
			float2 uv_WaterNormalMap;
			float3 viewDir;
		};

		// ======== Sampler =========
		sampler2D _TerrainTexture;
		sampler2D _TerrainNormalMap;
		sampler2D _WaterHeight;
		sampler2D _WaterNormalMap;
		sampler2D _TerrainHeight;
		sampler2D _CombinedHeight;
		sampler2D _FluxLeft;
		sampler2D _FluxRight;
		sampler2D _FluxTop;
		sampler2D _FluxBottom;

		float4 _WaterHeight_TexelSize;
		// ==========================

		// ======= Variables ========
		fixed4 _TerrainColor;
		half _TerrainGloss;
		half _TerrainMetallic;
		half4 _WaterColor;
		half4 _WaterDeepColor;
		float _WaterDepthBias;
		half _WaterGloss;
		half _WaterNormalStrength;
		half _TerrainNormalStrength;
		float _WaterFresnelPower;
		half _WaterMetallic;
		half _HeightStrength;
		float _TessellationFactor;
		float _SobelStrength;
		half4 _WaterFoamColor;
		half _WaterFoamStrength;
		// ==========================

		// Returns flux composed in one vector.
		// R: Left, G: Right, B: Top, A: Bottom
		half4 composeFlux(Input IN) {
			half fluxLeft = tex2D(_FluxLeft, IN.uv_WaterHeight).r;
			half fluxRight = tex2D(_FluxRight, IN.uv_WaterHeight).r;
			half fluxTop = tex2D(_FluxTop, IN.uv_WaterHeight).r;
			half fluxBottom = tex2D(_FluxBottom, IN.uv_WaterHeight).r;
			return half4(fluxLeft, fluxRight, fluxTop, fluxBottom);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half3 finalColor;
			half finalSmoothness;
			half finalMetallic;

			float waterHeight = tex2D(_WaterHeight, IN.uv_WaterHeight).r;
			float terrainHeight = tex2D(_TerrainHeight, IN.uv_WaterHeight).r;
			float combinedWaterTerrain = tex2D(_CombinedHeight, IN.uv_WaterHeight).r;
			float heightDifference = abs(combinedWaterTerrain - terrainHeight);

			fixed4 terrainColor = tex2D (_TerrainTexture, IN.uv_TerrainTexture) * _TerrainColor;
			fixed3 waterColor = lerp(_WaterColor, _WaterDeepColor, heightDifference / _WaterDepthBias);

			float offset = _Time[1] * 0.025;
			float2 uv = float2(offset, offset);

			// Calculate values for the visible surface
			float3 surfaceNormal;
			if (waterHeight > 0.01) {
				half4 flux = composeFlux(IN);
				half fluxMax = max(flux.r, max(flux.g, max(flux.b, flux.a)));
				// Get water normals from texture
				float3 waterNormal = UnpackNormal(tex2D(_WaterNormalMap, IN.uv_WaterNormalMap + uv));
				waterNormal.xy *= _WaterNormalStrength;

				surfaceNormal = height2NormalSobel(img3x3(_CombinedHeight, IN.uv_WaterHeight, 0, _WaterHeight_TexelSize.xy)) + waterNormal;
				surfaceNormal = normalize(float3(surfaceNormal.xy, surfaceNormal.z * _SobelStrength));

				float fresnel = calculateRim(IN.viewDir, surfaceNormal, _WaterFresnelPower);

				finalColor = lerp(waterColor, terrainColor, fresnel) + _WaterFoamColor * fluxMax * _WaterFoamStrength;
				finalSmoothness = _WaterGloss;
				finalMetallic = _WaterMetallic;
			} else {
				float3 terrainNormal = UnpackNormal(tex2D(_TerrainNormalMap, IN.uv_TerrainTexture));
				terrainNormal.xy *= _TerrainNormalStrength;

				surfaceNormal = height2NormalSobel(img3x3(_CombinedHeight, IN.uv_WaterHeight, 0, _WaterHeight_TexelSize.xy)) + terrainNormal;
				surfaceNormal = normalize(float3(surfaceNormal.xy, surfaceNormal.z * _SobelStrength));
				finalColor = terrainColor;
				finalSmoothness = _TerrainGloss;
				finalMetallic = _TerrainMetallic;
			}

			o.Normal = surfaceNormal;
			o.Albedo = finalColor;
			o.Metallic = finalMetallic;
			o.Smoothness = finalSmoothness;
			o.Alpha = terrainColor.a;
		}

		float4 tessFixed() {
            return _TessellationFactor;
        }

		void vert (inout appdata v) {
			float combinedHeight = tex2Dlod(_CombinedHeight, float4(v.texcoord.xy, 0, 0)).r;
			v.vertex.xyz += v.normal * combinedHeight * _HeightStrength;
      	}
		ENDCG
	}
	FallBack "Diffuse"
}
