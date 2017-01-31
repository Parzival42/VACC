Shader "Custom/Hatching" {
	Properties {
		_Hatch0 ("Hatch 0", 2D) = "white" {}
		_Hatch1 ("Hatch 1", 2D) = "white" {}		
		_Hatch2 ("Hatch 2", 2D) = "white" {}
		_Hatch3 ("Hatch 3", 2D) = "white" {}		
		_Hatch4 ("Hatch 4", 2D) = "white" {}		
		_Hatch5 ("Hatch 5", 2D) = "white" {}					
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf SimpleLambert

		sampler2D _Hatch0;
		sampler2D _Hatch1;
		sampler2D _Hatch2;
		sampler2D _Hatch3;
		sampler2D _Hatch4;
		sampler2D _Hatch5;

		half4 LightingSimpleLambert (inout SurfaceOutput s, half3 lightDir, half atten)
		{
			float2 uv = s.Albedo.xy;		
			s.Albedo = 0.0;			
		
			half NdotL = dot (s.Normal, lightDir);
			half diff = NdotL;// * 0.75 + 0.25;
			half4 c;
			
			half lightColor = _LightColor0.r * 0.3 + _LightColor0.g * 0.59 + _LightColor0.b * 0.11;
			half intensity = lightColor * (diff * atten * 2);
			intensity = saturate(intensity);
			
			float part = 1 / 6.0;
			// KLUDGE: with "if" instead of "else if" lines between regions are invisible		
			if (intensity <= part)
			{	
				float temp = intensity;
				temp *= 6;
				c.rgb = lerp(tex2D(_Hatch0, uv), tex2D(_Hatch1, uv), temp);			
			}
			if (intensity > part && intensity <= part * 2)
			{
				float temp = intensity - part;
				temp *= 6;				
				c.rgb = lerp(tex2D(_Hatch1, uv), tex2D(_Hatch2, uv), temp);			
			}
			if (intensity > part * 2 && intensity <= part * 3)
			{
				float temp = intensity - part * 2;
				temp *= 6;				
				c.rgb = lerp(tex2D(_Hatch2, uv), tex2D(_Hatch3, uv), temp);			
			}
			if (intensity > part * 3 && intensity <= part * 4)
			{
				float temp = intensity - part * 3;
				temp *= 6;				
				c.rgb = lerp(tex2D(_Hatch3, uv), tex2D(_Hatch4, uv), temp);			
			}
			if (intensity > part * 4 && intensity <= part * 5)
			{
				float temp = intensity - part * 4;
				temp *= 6;				
				c.rgb = lerp(tex2D(_Hatch4, uv), tex2D(_Hatch5, uv), temp);			
			}
			if (intensity > part * 5)
			{
				float temp = intensity - part * 5;
				temp *= 6;				
				c.rgb = lerp(tex2D(_Hatch5, uv), 1, temp);			
			}	

			return c;
		}

		struct Input 
		{
			float2 uv_Hatch0;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			o.Albedo.xy = 0.1 * IN.uv_Hatch0;
		}
		ENDCG
	}
    Fallback "Diffuse"
}