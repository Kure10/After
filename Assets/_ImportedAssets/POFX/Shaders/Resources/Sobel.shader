Shader "POFX/Sobel" {
	Properties{
		
		_MainTex("Maintex", 2D) = "BLack" {}		
		_Intensity("Intensity", Range(0.0, 1.0)) = 1.0			
		_Color("Color", Color) = (1,1,1,1)
		_Outline("Outline", float) = 0.0
		_Invert("Invert", Range(0.0, 1.0)) = 1.0
		_MixColor("Mix color", Range(0.0, 1.0)) = 1.0
	}

		SubShader{

		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+25" }
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
	half _Invert;
	half _MixColor;
	//float4 _GrabTexture_TexelSize;	
	
	struct Inputa
	 {
	 float2 uv_MainTex;
	 float4 grabPos;
	 half3 viewDir;
	 half3 worldNormal;
	 
	 };

	float Sbl(in float4 pixel) {
		return sqrt((pixel.r*pixel.r) + (pixel.g*pixel.g) + (pixel.b*pixel.b));
	}
	float3 sobel(float dx, float dy, float4 pixel)
	{	
		//http://en.wikipedia.org/wiki/Sobel_operator
		//      X            Y
		//    1 0 -1     -1 -2 -1
		//    2 0 -2      0  0  0
		//    1 0 -1      1  2  1

		float TL = Sbl(tex2Dproj(_GrabTexture, pixel + float4(-dx, dy,0,0)));
		float L  = Sbl(tex2Dproj(_GrabTexture, pixel + float4(-dx, 0,0,0)));
		float BL = Sbl(tex2Dproj(_GrabTexture, pixel + float4(-dx, -dy,0,0)));
		float T  = Sbl(tex2Dproj(_GrabTexture, pixel + float4(0, dy,0,0)));
		float B  = Sbl(tex2Dproj(_GrabTexture, pixel + float4(0, -dy,0,0)));
		float TR = Sbl(tex2Dproj(_GrabTexture, pixel + float4(dx, dy,0,0)));
		float R  = Sbl(tex2Dproj(_GrabTexture, pixel + float4(dx, 0,0,0)));
		float BR = Sbl(tex2Dproj(_GrabTexture, pixel + float4(dx, -dy,0,0)));	
		//filter
		float x = TL + 2.0*L + BL - TR - 2.0*R - BR;
		float y = -TL - 2.0*T - TR + BL + 2.0 * B + BR;
		float color = sqrt((x*x) + (y*y));
		return float3(color, color, color);
	}


	void surf(Input IN, inout SurfaceOutput o) {
		
		float4 screenUV = IN.grabPos;

		float4 mask = tex2D(_MainTex, IN.uv_MainTex);
		clip(mask.a - .1);

		half3 n = normalize(IN.worldNormal);
		half3 v = normalize(IN.viewDir);
		
		float4 color = tex2Dproj(_GrabTexture, screenUV);
		float4 sBL = color;

		sBL.rgb = sobel(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y, screenUV);
		sBL.rgb = lerp( float3(1,1,1) - sBL.rgb, sBL.rgb, _Invert );				
		
		sBL.rgb = lerp(sBL.rgb, sBL.rgb *_Color.rgb, _Color.a);
		o.Emission = lerp(color,  lerp( sBL.rgb , color * sBL.rgb , _MixColor ), GetIntensity(n,v));
		
	}
	ENDCG
	}
}