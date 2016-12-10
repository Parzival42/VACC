Shader "DustSucker/CombinedWaterTerrain" {
	Properties {
		_TerrainColor ("Terrain Color", Color) = (1, 1, 1, 1)
		_TerrainTexture ("Terrain Texture", 2D) = "white" {}
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
		_VelocityX ("X Velocity", 2D) = "black" {}
		[HideInInspector]
		_VelocityY ("Y Velocity", 2D) = "black" {}
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
		sampler2D _WaterHeight;
		sampler2D _WaterNormalMap;
		sampler2D _TerrainHeight;
		sampler2D _CombinedHeight;
		sampler2D _VelocityX;
		sampler2D _VelocityY;

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
		float _WaterFresnelPower;
		half _WaterMetallic;
		half _HeightStrength;
		float _TessellationFactor;
		float _SobelStrength;
		half4 _WaterFoamColor;
		half _WaterFoamStrength;
		// ==========================

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float offset = _Time[1] * 0.045;
			float2 uv = float2(offset, offset);

			half3 finalColor;
			half finalSmoothness;
			half finalMetallic;

			float waterHeight = tex2D(_WaterHeight, IN.uv_WaterHeight).r;
			float terrainHeight = tex2D(_TerrainHeight, IN.uv_WaterHeight).r;
			float heightDifference = abs(waterHeight - terrainHeight);

			fixed4 terrainColor = tex2D (_TerrainTexture, IN.uv_TerrainTexture) * _TerrainColor;
			fixed3 waterColor = lerp(_WaterColor, _WaterDeepColor, heightDifference / _WaterDepthBias);

			float2 velocity = float2(tex2D(_VelocityX, IN.uv_WaterHeight).r, tex2D(_VelocityY, IN.uv_WaterHeight).r);

			// Calculate values for the visible surface
			float3 surfaceNormal;
			// TODO: Eliminate magic number
			if (waterHeight + 0.0 >= terrainHeight) {
				// Get water normals from texture
				float3 waterNormal = UnpackNormal(tex2D(_WaterNormalMap, IN.uv_WaterNormalMap + uv));
				waterNormal.xy *= _WaterNormalStrength;

				surfaceNormal = height2NormalSobel(img3x3(_CombinedHeight, IN.uv_WaterHeight, 0, _WaterHeight_TexelSize.xy)) + waterNormal;
				surfaceNormal = normalize(float3(surfaceNormal.xy, surfaceNormal.z * _SobelStrength));

				float fresnel = calculateRim(IN.viewDir, surfaceNormal, _WaterFresnelPower);
				float velocityMagnitude = clamp(length(velocity) * _WaterFoamStrength, 0, 2);

				finalColor = lerp(waterColor, terrainColor, fresnel) + smoothstep(half3(0, 0, 0), _WaterFoamColor, velocityMagnitude);
				finalSmoothness = _WaterGloss;
				finalMetallic = _WaterMetallic;
			} else {
				surfaceNormal = height2NormalSobel(img3x3(_CombinedHeight, IN.uv_WaterHeight, 0, _WaterHeight_TexelSize.xy));
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
