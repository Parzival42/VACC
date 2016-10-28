﻿Shader "Hidden/PaintTest"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_uvHit ("UV Hit", Vector) = (0, 0, 0, 0)
		_Radius ("Radius", Float) = 0.03
		_PaintStrength ("Paint Strength", Range(-1.0, 1.0)) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Fog { Mode Off }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			half4 _uvHit;
			half _Radius;
			half _PaintStrength;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				// TODO: Better way to add color?
				if(distance(i.uv, _uvHit.xy) <= _Radius)
					col.rgb += _PaintStrength;

				return col;
			}
			ENDCG
		}
	}
}
