Shader "POFX/Desaturate" {
	Properties{
		_MainTex("Maintex", 2D) = "white" {}
		_Intensity("Intensity", Range(0.0, 1.0)) = 1.0			
		_Color("Color", Color) = (1,1,1,1)
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
	half _Refraction;
	half _Fresnel;
	half _Distorsion;
	//float4 _GrabTexture_TexelSize;	


	struct Inputa
	 {
	 float2 uv_MainTex;
	 float4 grabPos;
	 half3 viewDir;
	 half3 worldNormal;
	 
	 };



	void surf(Input IN, inout SurfaceOutput o)
	{
		float4 screenUV = IN.grabPos;

//#ifdef SPRITE_RENDERER
		float4 mask = tex2D(_MainTex, IN.uv_MainTex);
		clip(mask.a - .1);
//#endif

		float4 color = tex2Dproj(_GrabTexture, screenUV);
		
		float c = (color.r+color.g+color.b) / 3.0;

		float intensity = GetIntensity(IN);

		o.Alpha = color.a;
		o.Emission =lerp( color, float3(c,c,c), intensity) * lerp( float3(1,1,1),_Color.rgb,_Color.a*intensity);

	}
	ENDCG
	}
}