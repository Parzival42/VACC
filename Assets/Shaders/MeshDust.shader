Shader "DustSucker/MeshDust" {
 Properties {
		_Tint ("Tint", Color) = (1,1,1,1)
		_TimeFactor ("TimeFactor", Range(0,1)) = 0.0
		//_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Scale ("Scale", Float) = 1.5
		_DispBias ("Displacement Bias", Float) = 30
		_DispScale ("Displacement Scale", Float) = 10
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
    CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert alpha
		#pragma target 3.0

		#include "Includes/Noise.cginc"

		//sampler2D _MainTex;

		struct Input {
			//float2 uv_MainTex;
			float3 worldNormal;
			float3 viewDir;
			float4 screenPos;
			float3 texCoord3D;
			INTERNAL_DATA
		};

		half _Glossiness;
		half _Metallic;
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
			half n = heightMap(o.texCoord3D);

			half3 texturePosition = half4( half3( 1.5 - n, 1.0 - n, 0.5 - n ), 1.0 ).xyz;
			float dispFactor = _DispScale * texturePosition.x + _DispBias;
			float4 dispPos = float4( v.normal.xyz * dispFactor, 0.0 ) + pos;
			v.vertex = mul(unity_WorldToObject, dispPos);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			half n = heightMap(IN.texCoord3D);
			// 1.5 - n, 1.0 - n, 0.5 - n 
			fixed4 c = half4( half3(n, n, n ), n ) * _Tint*n;

			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
  }
  Fallback "Diffuse"
}