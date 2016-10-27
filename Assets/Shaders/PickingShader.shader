Shader "DustSucker/PickingShader" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0.0
		_PickColor ("Pick Color", Color) = (1, 1, 1, 1)
		_PickRadius ("Pick Radius", Float) = 0.2
		_MousePosition ("Mouse Position", Vector) = (0, 0, 0, 0)
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _PickColor;
		half _PickRadius;

		// World mouse position
		half4 _MousePosition;

		/*
		 * Normalizes the given value between the minimum and maximum boundary.
		 * Returns a float between 0 and 1.
		 */
		float normalizeBetween(float min, float max, float value) {
			return (value - min) / (max - min);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			float dist = distance(IN.worldPos, _MousePosition);

			if(dist < _PickRadius) {
				float normalizedDistance = normalizeBetween(0.0, _PickRadius, dist);
				c = lerp(c, _PickColor, 1.0 - normalizedDistance);
			}

			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
