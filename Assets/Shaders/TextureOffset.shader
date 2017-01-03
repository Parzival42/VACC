Shader "DustSucker/TextureOffset" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MaskOffset ("Offset", Vector) = (0, 0, 0, 0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
		};

        sampler2D _MainTex;
        half4 _MaskOffset;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex + _MaskOffset.xy);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
