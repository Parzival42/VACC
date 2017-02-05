Shader "Hidden/WeirdShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Intensity("Intensity", Float) = 1
		_Hueoffset("Hueoffset", Float) = 0
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
			half _Hueoffset;

			// source: http://gamedev.stackexchange.com/questions/59797/glsl-shader-change-hue-saturation-brightness
			fixed3 rgb2hsv(fixed3 c)
			{
				fixed4 K = fixed4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				fixed4 p = lerp(fixed4(c.bg, K.wz), fixed4(c.gb, K.xy), step(c.b, c.g));
				fixed4 q = lerp(fixed4(p.xyw, c.r), fixed4(c.r, p.yzx), step(p.x, c.r));

				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return fixed3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			fixed3 hsv2rgb(fixed3 c)
			{
				fixed4 K = fixed4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				fixed3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
				return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 p = -1. + 2.*i.uv;
				float t = _Time * 20;
				float an = t*.5;
				float2 r = float2(p.x*cos(an) - p.y*sin(an), p.x*sin(an) + p.y*cos(an)),
				uv = float2(.25*r.x / abs(r.y), 0.5*t + .01 / abs(r.y));
				uv = frac(uv);

				uv = i.uv + _Intensity * (uv - i.uv); // linear interpolation

				fixed4 c = tex2D(_MainTex, uv);

				fixed3 cHSV = rgb2hsv(c);
				cHSV.x += frac(t * _Hueoffset) * _Hueoffset;
				c = fixed4(hsv2rgb(cHSV), 1.0);

				// just invert the colors
				//col = 1 - col;

				return c;
			}
			ENDCG
		}
	}
}
