Shader "POFX/Hsl" {
	Properties{
		_MainTex("Maintex", 2D) = "white" {}
		_Intensity("Intensity", Range(0.0, 1.0)) = 1.0
		_Hue("Hue", Range(0.0, 1.0)) = 1.0
		_Brightness("Brightness", Range(0.0, 1.0)) = 1.0
		_Contrast("Contrast", Range(0.0, 1.0)) = 0.0
		_Saturation("Saturation", Range(0.0, 1.0)) = 0.0
		_Color("Color", Color) = (1,1,1,1)
		_Outline("Outline", float) = 0.0
		_Invert("Invert", Range(0.0, 1.0)) = 1.0
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
	half _Contrast;
	half _Brightness;
	half _Hue;
	half _Saturation;
	half _Invert;
	//float4 _GrabTexture_TexelSize;	

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

		float4 mask = tex2D(_MainTex, IN.uv_MainTex);
		clip(mask.a - .1);

		float4 color = tex2Dproj(_GrabTexture, screenUV);
		float3 c= color;
		float l = Luminance(c);
		
		c += l * _Brightness;
		c += ((c - 0.5) * _Contrast);		
		c += l != 0 ? (saturate(c - l) * _Saturation) / l : 0;
		c = Hue(c, _Hue);

		o.Alpha = color.a;
		
		c = lerp(c, float3(1, 1, 1) - c, _Invert);
		o.Emission = lerp(color.rgb, c, GetIntensity(IN));
		
	}
	ENDCG
	}
}