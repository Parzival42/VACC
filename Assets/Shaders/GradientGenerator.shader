Shader "Hiddem/GradientGenerator"
{
	Properties{
		_Falloff("falloff", float) = 1.0
		_InnerColor("inner color", Color) = (1.0,1.0,1.0,1.0)
		_OuterColor("outer color", Color) = (0.0,0.0,0.0,0.0)
		[Enum(UnityEngine.Rendering.BlendMode)] _Blend("Blend mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _Blend2("Blend mode 2", Float) = 1
		[Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("Blend Op", Float) = 1
	}

		SubShader
	{
		Tags{ "Queue" = "Transparent" }

		Pass
		{
			ZWrite Off
			BlendOp[_BlendOp]
			Blend[_Blend][_Blend2]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			fixed _Falloff;
			fixed4 _InnerColor;
			fixed4 _OuterColor;

			struct vertexInput {
				float4 vertex : POSITION;
				float2 texcoord0 : TEXCOORD0;
			};

			struct fragmentInput {
				float4 position : SV_POSITION;
				float2 texcoord0 : TEXCOORD0;
			};

			fragmentInput vert(vertexInput i) {
				fragmentInput o;
				o.position = mul(UNITY_MATRIX_MVP, i.vertex);
				o.texcoord0 = i.texcoord0;
				return o;
			}

			fixed4 frag(fragmentInput i) : SV_Target{
				fixed d = length(i.texcoord0.xy - fixed2(0.5,0.5));
				float t = pow(1.0 - min(0.5, d) * 2.0, _Falloff);
				return lerp(_OuterColor, _InnerColor, t);
			}

			ENDCG
		}
	}
}
