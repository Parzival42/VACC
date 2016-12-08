Shader "DustSucker/CombinedWaterTerrain" {
	Properties {
		_TerrainColor ("Terrain Color", Color) = (1, 1, 1, 1)
		_TerrainTexture ("Terrain Texture", 2D) = "white" {}
		_TerrainGloss ("Terrain Smoothness", Range(0, 1)) = 0.5
		_TerrainMetallic ("Terrain Metallic", Range(0, 1)) = 0.0

		_WaterColor ("Water Color", Color) = (1, 1, 1, 1)
		_WaterGloss ("Water Gloss", Range(0, 1)) = 0.7
		_WaterMetallic ("Water Metallic", Range(0, 1)) = 0.0

		// Fill the heights per script!
		[HideInInspector]
		_WaterHeight ("Water Height", 2D) = "black" {}
		[HideInInspector]
		_TerrainHeight ("Terrain Height", 2D) = "black" {}
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
		};

		// ======== Sampler =========
		sampler2D _TerrainTexture;
		sampler2D _WaterHeight;
		sampler2D _TerrainHeight;

		float4 _WaterHeight_TexelSize;
		// ==========================

		// ======= Variables ========
		fixed4 _TerrainColor;
		half _TerrainGloss;
		half _TerrainMetallic;
		half4 _WaterColor;
		half _WaterGloss;
		half _WaterMetallic;
		half _HeightStrength;
		float _TessellationFactor;
		float _SobelStrength;
		// ==========================

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 terrainColor = tex2D (_TerrainTexture, IN.uv_TerrainTexture) * _TerrainColor;
			fixed3 waterColor = _WaterColor;
			half3 finalColor;
			half finalSmoothness;
			half finalMetallic;

			float waterHeight = tex2D(_WaterHeight, IN.uv_WaterHeight).r;
			float terrainHeight = tex2D(_TerrainHeight, IN.uv_WaterHeight).r;

			// Calculate values for the visible surface
			float3 surfaceNormal;
			// TODO: Eliminate magic number
			if (waterHeight + 0.15 >= terrainHeight) {
				surfaceNormal = height2NormalSobel(img3x3(_WaterHeight, IN.uv_WaterHeight, 0, _WaterHeight_TexelSize.xy));
				finalColor = waterColor;
				finalSmoothness = _WaterGloss;
				finalMetallic = _WaterMetallic;
			} else {
				surfaceNormal = height2NormalSobel(img3x3(_TerrainHeight, IN.uv_WaterHeight, 0, _WaterHeight_TexelSize.xy));
				finalColor = terrainColor;
				finalSmoothness = _TerrainGloss;
				finalMetallic = _TerrainMetallic;
			}
			o.Normal = normalize(float3(surfaceNormal.xy, surfaceNormal.z * _SobelStrength));

			o.Albedo = finalColor;
			o.Metallic = finalMetallic;
			o.Smoothness = finalSmoothness;
			o.Alpha = terrainColor.a;
		}

		float4 tessFixed() {
            return _TessellationFactor;
        }

		void vert (inout appdata v) {
			// Read height values and add them together
			float4 waterHeight = tex2Dlod(_WaterHeight, float4(v.texcoord.xy, 0, 0));
			float4 terrainHeight = tex2Dlod(_TerrainHeight, float4(v.texcoord.xy, 0, 0));
			float height = clamp(waterHeight.r, 0, 1) + clamp(terrainHeight.r, 0, 1);

			v.vertex.xyz += v.normal * height * _HeightStrength;
      	}
		ENDCG
	}
	FallBack "Diffuse"
}
