Shader "POFX/Refraction" {
	Properties{
		_MainTex("Maintex", 2D) = "white" {}
		_DistorsionTex("DistorsionTex", 2D) = "white" {}
		_Distorsion("Distorsion", Range(0.0, 1.0)) = 1.0
		_Refraction("Refraction", Range(0.00, 100.0)) = 1.0
		_Fresnel("Fresnel Coefficient", float) = 1.0
		_Intensity("Intensity", Range(0.0, 1.0)) = 1.0			
		_Color("Color", Color) = (1,1,1,1)
		_Outline("Outline", float) = 0.0
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
	sampler2D _DistorsionTex;
	float2 _DistorsionTexScale;
	float2 _DistorsionTexOffset;
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
/*	 
	void vertOutline2(inout appdata_base v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
	v.vertex.xyz += v.normal*_Outline;

}*/

	void surf(Input IN, inout SurfaceOutput o) {
		
		float4 screenUV = IN.grabPos;

		float4 mask = tex2D(_MainTex, IN.uv_MainTex);
		clip(mask.a - .1);
	
		half3 n = normalize(IN.worldNormal);
		//float3 distort = (tex2D(_DistorsionTex, (IN.grabPos) * _DistorsionTexScale + _DistorsionTexOffset) - float3(.5,.5,.5))*2;
		float2 distord2D = tex2D(_DistorsionTex, (IN.uv_MainTex) * _DistorsionTexScale + _DistorsionTexOffset);
		float3 distort = ( distord2D.xyx - float3(.5, .5, .5)) * 2;

		//float3 distort = (normalize(distord2D.xyx + distord2D.yxy) - float3(.5, .5, .5)) * 2;
		n = normalize(lerp(n,n+distort,_Distorsion));

		half3 v = normalize(IN.viewDir);

		half fpow = lerp(10,1, _Fresnel);
		half fr = pow(1.0f - dot(v, n),  fpow  );		
		
		float2 offset =  _Refraction * _GrabTexture_TexelSize.xy;


		screenUV.xy = offset * fr *_Intensity + screenUV.xy;
		float4 refrColor = tex2Dproj(_GrabTexture, screenUV);
		
	//float4 refrColor = _Color;

		o.Alpha = refrColor.a;
		o.Emission = refrColor.rgb * lerp( float3(1,1,1),_Color.rgb,_Color.a*GetIntensity(IN));

	}
	ENDCG
	}
}