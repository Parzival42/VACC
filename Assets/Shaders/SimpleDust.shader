Shader "DustSucker/SimpleDust"
{
Properties
	{
		_Iterations("Iterations", Range(1, 100)) = 10
		_ViewDistance("View Distance", Range(0, 5)) = 2
		_CloudColor("Cloud Color", Color) = (1, 1, 1, 1)
		_CloudDensity("Cloud Density", Range(0, 0.75)) = 0.5
		_NoiseOffsets ("Noise Texture", 2D) = "white" {}
	}

	SubShader
	{
	    Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _NoiseOffsets;
			float3 _CamPos;
			float3 _CamRight;
			float3 _CamUp;
			float3 _CamForward;

			int _Iterations;
			float3 _SkyColor;
			float4 _CloudColor;
			float _ViewDistance;
			float _CloudDensity;

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata_base  v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			// noise function - https://www.shadertoy.com/view/4sfGzS
			float noise(float3 x) { x *= 4.0; float3 p = floor(x); float3 f = frac(x); f = f*f*(3.0 - 2.0*f); float2 uv = (p.xy + float2(37.0, 17.0)*p.z) + f.xy; float2 rg = tex2D(_NoiseOffsets, (uv + 0.5) / 256.0).yx; return lerp(rg.x, rg.y, f.z); }
            
            // fbm noise
			float fbm(float3 pos, int octaves) { float f = 0.; for (int i = 0; i < octaves; i++) { f += noise(pos) / pow(2, i + 1); pos *= 2.01; } f /= 1 - 1 / pow(2, octaves + 1); return f; }

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv - 0.5;
				// fixed camera just to test
				_CamUp = float3(0, 1, 0);
				_CamForward = float3(0, 0, 1);
				_CamRight = float3(1, 0, 0);

				float3 pos = float3(0, 0, -5);
				float3 ray = _CamUp * uv.y + _CamRight * uv.x + _CamForward;
                
				float3 p = pos;
                // add up density
				float density = 0;
                
				for (float i = 0; i < _Iterations; i++)
				{
                    // fade clouds in and out
					float f = i / _Iterations;

					float alpha = smoothstep(0, _Iterations * 0.2, i) * (1 - f) * (1 - f);

					float denseClouds = smoothstep(0.75-_CloudDensity, 0.75, fbm(p, 5));
					float lightClouds = (smoothstep(-0.2, 1.2, fbm(p * 2, 2)) - 0.5) * 0.5;

                    // add to density variable
					density += (lightClouds + denseClouds) * alpha;
                    // move one step further away from the camera
					p = pos + ray * f * _ViewDistance;
				}

				float4 color = (_CloudColor - 0.5) * (density / _Iterations) * 20;

				return color;
			}
			ENDCG
		}
	}
}