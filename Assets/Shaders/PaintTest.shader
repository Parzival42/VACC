// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Hidden/PaintTest"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_uvHit ("UV Hit", Vector) = (0, 0, 0, 0)
		//_Radius ("Radius", Float) = 0.03
		_PaintStrength ("Paint Strength", Range(-1.0, 1.0)) = 1
		_Falloff ("Falloff", Float) = 100.0
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
			
			half4 _uvHit;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			half _Radius;
			half _PaintStrength;
			float _Falloff;

			fixed4 frag (v2f i) : SV_Target
			{
			
				half3 worldpos = mul(unity_WorldToObject, _uvHit);
				_uvHit.xy = worldpos.xz/20;

				fixed4 col = tex2D(_MainTex, i.uv);

				float d = distance(i.uv, _uvHit.xy);
				float t = pow(1.0 - min(0.5, d) * 2.0, _Falloff);

				// TODO: Better way to add color?
				//if(distance(i.uv, _uvHit.xy) <= _Radius)
					col.rgb += t*_PaintStrength;

				return col;
			}
			ENDCG
		}
	}
}
