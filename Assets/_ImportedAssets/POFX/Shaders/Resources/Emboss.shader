Shader "POFX/Emboss" {
	Properties{
		_MainTex("Maintex", 2D) = "white" {}
		_Intensity("Intensity", Range(0.0, 1.0)) = 1.0			
		_Color("Color", Color) = (1,1,1,1)
		_Outline("Outline", float) = 0.0
		_Invert("Invert", Range(0.0, 1.0)) = 1.0
		_MixColor("Mix color", Range(0.0, 1.0)) = 1.0
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
#pragma multi_compile __ SPRITE_RENDERER
//#define UNITY_PASS_FORWARDBASE
#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
#pragma target 3.0
#include "POFX.cginc"

	
	sampler2D _GrabTexture;
	sampler2D _MainTex;
	half _Refraction;
	half _Fresnel;
	half _Distorsion;
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



	void surf(Input IN, inout SurfaceOutput o)
	{	
		float4 screenUV = IN.grabPos;

//#ifdef SPRITE_RENDERER
		float4 mask = tex2D(_MainTex, IN.uv_MainTex);
		clip(mask.a-.1);
//#endif
		float4 color = tex2Dproj(_GrabTexture, screenUV);
		float c = color;		
		float l = fwidth(Luminance(color));
		c *= 1.5*clamp(1.0 - 8.0*l, 0.0, 1.0)*float3(1,1,1);
		c = lerp(c, float3(1, 1, 1) - c, _Invert);

		o.Alpha = color.a;
		o.Emission =lerp( color, lerp(float3(c, c, c),color*float3(c,c,c),_MixColor), GetIntensity(IN)) * lerp( float3(1,1,1),_Color.rgb,_Color.a*_Intensity );

	}
	ENDCG
	}
}