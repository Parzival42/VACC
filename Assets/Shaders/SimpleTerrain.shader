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

		float3x3 img3x3(sampler2D colorSampler, float2 textureCoord, int colorChannel)
		{
			float d = _Heightmap_TexelSize.xy;
		    float3x3 c;
		    c[0][0] = tex2D(colorSampler, textureCoord + float2(-d, -d))[colorChannel];
		    c[0][1] = tex2D(colorSampler, textureCoord + float2( 0, -d))[colorChannel];
		    c[0][2] = tex2D(colorSampler, textureCoord + float2( d, -d))[colorChannel];

		    c[1][0] = tex2D(colorSampler, textureCoord + float2(-d, 0))[colorChannel];
		    c[1][1] = tex2D(colorSampler, textureCoord                )[colorChannel];
		    c[1][2] = tex2D(colorSampler, textureCoord + float2( d, 0))[colorChannel];

		    c[2][0] = tex2D(colorSampler, textureCoord + float2(-d, d))[colorChannel];
		    c[2][1] = tex2D(colorSampler, textureCoord + float2( 0, d))[colorChannel];
		    c[2][2] = tex2D(colorSampler, textureCoord + float2( d, d))[colorChannel];
		    return c;
		}

		float3 height2NormalSobel(float3x3 c) {
		    float3x3 x = float3x3(1.0, 0.0, -1.0,
		                          2.0, 0.0, -2.0,
		                          1.0, 0.0, -1.0);

		    float3x3 y = float3x3(1.0,  2.0,  1.0,
		                          0.0,  0.0,  0.0,
		                          -1.0, -2.0, -1.0);
		    x = x * c;
		    y = y * c;

		    float cx =  x[0][0] + x[0][2]
		              + x[1][0] + x[1][2]
		              + x[2][0] + x[2][2];

		    float cy =  y[0][0] + y[0][1] + y[0][2]
		              + y[2][0] + y[2][1] + y[2][2];

		    float cz =  sqrt(1 - (cx * cx + cy * cy));
		    return float3(cx, cy, cz);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			float3 normal = height2NormalSobel(img3x3(_Heightmap, IN.uv_MainTex, 0));
			o.Normal = normalize(float3(normal.xy, normal.z * _SobelStrength));

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		/*	// Dice approach --> Not working for our example
		float3 findNormal(float2 uv, float texelSize, float texelAspect) {
			float4 height;
			height[0] = tex2Dlod(_Heightmap, float4(uv + texelSize * float2(0, -1), 0, 0)).r * texelAspect;
			height[1] = tex2Dlod(_Heightmap, float4(uv + texelSize * float2(-1, 0), 0, 0)).r * texelAspect;
			height[2] = tex2Dlod(_Heightmap, float4(uv + texelSize * float2(1, 0), 0, 0)).r * texelAspect;
			height[3] = tex2Dlod(_Heightmap, float4(uv + texelSize * float2(0, 1), 0, 0)).r * texelAspect;

			float3 normal;
			normal.z = height[0] - height[3];
			normal.x = height[1] - height[2];
			normal.y = 2.0;

			return normalize(normal);
		}*/

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
