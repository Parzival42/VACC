Shader "DustSucker/MeshMelt" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalTex ("Normal Map", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Float) = 0.5
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_MeltY("Melt Height", Float) = 0.0
		_MeltYDistance("Melt Height Distance", Float) = 0.3
		_MeltThreshold("Melt Threshold", Float) = 0.4
		_MeltCurve("Melt Curve", Range(1.0,10.0)) = 3.0
		_MeltPosition("Melt Position", Vector) = (1,0,0,0)
		_MeltStrength("Melt Strength", Float) = 1.0

		_Tess("Tessellation Amount", Range( 1, 64 )) = 10
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:disp addshadow tessellate:tessDistance nolightmap
		#pragma target 4.6
        #include "Tessellation.cginc"

		sampler2D _MainTex;
		sampler2D _NormalTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _NormalStrength;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		half _MeltY;
		half _MeltYDistance;
		half _MeltThreshold;
		half _MeltCurve;
		half4 _MeltPosition;
		half _MeltStrength;

		float _Tess;

		struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
        };

        float meltCalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess)
		{
			float3 wpos = mul(unity_ObjectToWorld,vertex).xyz;
			float dist = distance (wpos, _WorldSpaceCameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0);

			// is the object in range?
			float melt = (( wpos.y - _MeltY ) / _MeltYDistance);

			// threshold - verts near edges are tessellated
			if( melt < -_MeltThreshold || melt > _MeltThreshold )
				f = 0.01;

			return f  * tess;
		}

        // distance based tessellation
		float4 tessDistance(appdata v0, appdata v1, appdata v2)
        {
            float minDist = 10.0;
            float maxDist = 25.0;

			float3 f;
			f.x = meltCalcDistanceTessFactor (v0.vertex, minDist, maxDist, _Tess);
			f.y = meltCalcDistanceTessFactor (v1.vertex, minDist, maxDist, _Tess);
			f.z = meltCalcDistanceTessFactor (v2.vertex, minDist, maxDist, _Tess);

			return UnityCalcTriEdgeTessFactors (f);
        }

		float4 vertDisp( float4 objectSpacePosition)
		{
			float4 worldSpacePosition = mul( unity_ObjectToWorld, objectSpacePosition );

			float melt = ( worldSpacePosition.y - _MeltY ) / _MeltYDistance;

			// verts near the ground are 1
			melt = 1 - saturate( melt );
			melt = pow( melt, _MeltCurve );

			float2 disp =  _MeltPosition.xz - worldSpacePosition.xz;

			worldSpacePosition.xz += disp * melt * _MeltStrength;

			return mul( unity_WorldToObject, worldSpacePosition );
		}

		void disp( inout appdata v )
		{
			float4 vert = vertDisp(v.vertex);

			float4 bitangent = float4( cross( v.normal, v.tangent ), 0 );

			// recalculate normal
			float4 v1 = vertDisp( v.vertex + v.tangent * 0.01);
			float4 v2 = vertDisp( v.vertex + bitangent * 0.01);

			float4 newTangent = v1 - vert;
			float4 newBitangent = v2 - vert;

			v.normal = cross( normalize(newTangent), normalize(newBitangent));
			v.vertex = vert;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float3 normal = UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex)) * _NormalStrength;
			o.Normal = normal;

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
