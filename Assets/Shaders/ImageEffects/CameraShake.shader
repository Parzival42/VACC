Shader "Hidden/CameraShake" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_OffsetX ("Offset X", Float) = 0
		_OffsetY ("Offset Y", Float) = 0
		_Zoom ("Zoom", Float) = 1
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			half _OffsetX;
			half _OffsetY;
			half _Zoom;

			fixed4 frag (v2f i) : SV_Target {
				float2 uv = i.uv + float2(_OffsetX, _OffsetY);
				uv *= _Zoom;
				fixed4 col = tex2D(_MainTex, uv);
				return col;
			}
			ENDCG
		}
	}
}
