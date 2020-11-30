Shader "POFX/Pixelate" {
	Properties{
		_MainTex("Maintex", 2D) = "white" {}
		_Refraction("Refraction", Range(0.00, 100.0)) = 1.0
		_Fresnel("Fresnel Coefficient", float) = 5.0
		_Reflectance("Reflectance", float) = 1.0

		_Intensity("Intensity", Range(0.0, 1.0)) = 1.0		
		_PixelSnapNear("Pixel snap 1", Range(1.0, 100.0)) = 1.0
		_PixelSnapFar("Pixel snap 2", Range(1.0, 100.0)) = 1.0
		_DistanceCamFar("Distance cam Far", float) = 10.0
		_Color("Color", Color) = (1,1,1,1)
		_outline("Outline", float) = 0.0
	}

		SubShader{

		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+15" }
		LOD 100
			Cull off
			zwrite on
			ZTest LEqual
			Offset -1,-1

			GrabPass{
			//"_GrabPixel"			
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
	//sampler2D _DistortTex : register(s2);
	float _Refraction;
	half _Fresnel;
	half _Reflectance;

	//float4 _GrabTexture_TexelSize;


	float density = 4.5;
	float metasze = 0.01;
	float4  noffset = float4(10.0, 1500.0, -90.0, 0.9);
	float _outline;
	float _PixelSnapNear;	
	float _PixelSnapFar;
	float _DistanceCamFar;
	

	void surf(Input IN, inout SurfaceOutput o) {		

		float4 screenUV = IN.grabPos;

		float4 mask = tex2D(_MainTex, IN.uv_MainTex);
		clip(mask.a - .1);

		float4 back = tex2Dproj(_GrabTexture, screenUV);

		//Pixelate
		float d = _DistanceCamFar>0 ? (saturate(length(IN.worldPos.xyz - _WorldSpaceCameraPos.xyz) / abs(_DistanceCamFar))) : 1;		
		float _PixelSnap = round(lerp(_PixelSnapNear, _PixelSnapFar, d ));		

		float2 uv = screenUV.xy / screenUV.w;
		uv /= _PixelSnap / (_ScreenParams.xy);
		uv = round(uv);
		uv *= _PixelSnap / (_ScreenParams.xy);
		float4 pixel = tex2D(_GrabTexture, uv);
	
		o.Alpha = back.a;
		o.Emission = lerp(back, pixel, GetIntensity(IN)) * _Color;

	}
	ENDCG
	}
}