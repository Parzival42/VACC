Shader "Hidden/WeirdShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Intensity("Intensity", Float) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
			half _Intensity;

			fixed4 frag (v2f i) : SV_Target
			{
		//		     float2 t = frac(IN.uv_MainTex*0.5)*2.0;
   //  float2 length = {1.0,1.0};
     //float2 mirrorTexCoords = length-abs(t-length);

				float2 p = -1. + 2.*i.uv; // _ScreenParams.xy;
				float t = _Time * 10;
				float an = t*.25;
				float2 r = float2(p.x*cos(an) - p.y*sin(an), p.x*sin(an) + p.y*cos(an)),
				uv = float2(.25*r.x / abs(r.y), 0.5*t + .01 / abs(r.y));
				uv = frac(uv);

				uv = i.uv + _Intensity * (uv - i.uv); // linear interpolation

				fixed4 c = tex2D(_MainTex, uv);

				//fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				//col = 1 - col;
				return c;
			}
			ENDCG
		}
	}
}
