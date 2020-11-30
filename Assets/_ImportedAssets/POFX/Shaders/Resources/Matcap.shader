Shader "POFX/Matcap" {
	Properties
	{
		_MainTex("Maintex", 2D) = "white" {}		
		_Color("Color", Color) = (1,1,1,1)
		_outline("Outline", float) = 0.0
		_Intensity("Intensity", Float) = 1
		_Mix("Mix", Float) = 1
		_AddMul("Add/Mul", Float) = 1
	}

		SubShader{

		Tags{ "RenderType" = "Opaque" "Queue" = "Transparent+1" }
		LOD 200
		Cull off
		//ZTest Always
		//ZTest Greater

		//Zwrite Off
		Offset -1,-1

		GrabPass
		{
		}

		
		//Cull back
		//ZTest LEqual
		
		//Zwrite On
		
		CGPROGRAM
#pragma surface surf NoLighting nolightmap
#pragma vertex vertInflate
//#define UNITY_PASS_FORWARDBASE
#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
#pragma target 3.0
#include "POFX.cginc"


	sampler2D _GrabTexture;
	sampler2D _MainTex;
	float _Mix;
	float _AddMul;
	//float4 _GrabTexture_TexelSize;
	
	void surf(Input IN, inout SurfaceOutput o) {

		float4 screenUV = IN.grabPos;

		half3 n = normalize(IN.worldNormal);
		half3 v = normalize(IN.viewDir);

		float4 matcap = tex2D( _MainTex, IN.customUV.xy );
		float4 screen = tex2Dproj(_GrabTexture, screenUV);
		float4 color = lerp(screen, lerp(matcap, lerp( screen+matcap, screen*matcap, _AddMul), _Mix ), GetIntensity(IN));

		o.Emission = color.rgb;

	}
	ENDCG
	

		

	}
}