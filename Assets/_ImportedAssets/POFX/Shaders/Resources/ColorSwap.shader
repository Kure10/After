Shader "POFX/ColorSwap" {
	Properties{
		_MainTex("Maintex", 2D) = "white" {}
		_Intensity("Intensity", Range(0.0, 1.0)) = 1.0
		_Color("Color", Color) = (1,1,1,1)
		_Color2("Color2", Color) = (1,1,1,1)
		_Threshold("Threshold", float) = 0.0
		_Smooth("Smooth", float) = 0.0
		_Outline("Outline", float) = 0.0
	}

		SubShader{

		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+5" }
		LOD 100
		Cull off
		Offset -1,-1
		GrabPass
	{

	}

CGPROGRAM
#pragma surface surf NoLighting nolightmap
#pragma vertex vertInflate
//#define UNITY_PASS_FORWARDBASE
#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
#pragma target 3.0
#include "POFX.cginc"

	
	sampler2D _GrabTexture;
	sampler2D _MainTex;
	float4 _Color2;
	float _Threshold;	
	float _Smooth;
	//float4 _GrabTexture_TexelSize;

	sampler2D _AlphaTex;
	float _AlphaSplitEnabled;

	float3 Hue(float3 color, float hue)
	{
		float a = radians(hue*360);
		float ca = cos(a);
		float3 k = float3(0.5, 0.5, 0.5);		
		return color * ca + cross(k, color) * sin(a) + k * dot(k, color) * (1-ca);
	}
	

	void surf(Input IN, inout SurfaceOutput o)
	{	

		float4 screenUV = IN.grabPos;

		/*
		float4 mask = tex2D(_MainTex, IN.uv_MainTex);
		clip(mask.a - .1);	
		*/
		
		float4 screen = tex2Dproj(_GrabTexture, screenUV);
		float4 originalColor = tex2D(_MainTex, IN.uv_MainTex);
		float d = dot(normalize(originalColor.rgb), normalize(_Color.rgb));

		float4 c = screen;
		
		if (d > ( 1 - _Threshold))
			//c = _Color2 * pow(d, _Smooth*_Smooth*10.0);		
			c.rgb += lerp(_Color2.rgb - _Color.rgb, float3(0,0,0), d * (_Smooth*_Smooth));
			

		o.Emission = lerp(screen, c, GetIntensity(IN));


	}
	ENDCG
	}
}