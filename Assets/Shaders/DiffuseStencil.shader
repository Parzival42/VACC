Shader "Custom/DiffuseStencil" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Stencil ("Stencil Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue"="Transparent"
             "IgnoreProjector"="False"
             "RenderType"="Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		uniform float4 _MainTex_TexelSize;
		sampler2D _MainTex;
		sampler2D _Stencil;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		/*void vert (inout appdata_full v) {
			#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					IN.uv_MainTex.y = 1-IN.uv_MainTex.y;
			#endif
		}*/

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

			o.Alpha = 1 - tex2D (_Stencil, IN.uv_MainTex).a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
