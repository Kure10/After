Shader "POFX/Rim" {
	Properties{
		_MainTex("Maintex", 2D) = "white" {}
		_Intensity("Intensity", Range(0.0, 1.0)) = 1.0			
		_Color("Color", Color) = (1,1,1,1)
		_Outline("Outline", float) = 0.0
		_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
	}

		SubShader{

		Tags{ "RenderType" = "Transparent+1" "Queue" = "Transparent+6" }
		LOD 100
		Cull off
		Zwrite on
		ZTest LEqual
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
	float4 _RimColor;
	half _RimPower;	
	//float4 _GrabTexture_TexelSize;		

	void surf(Input IN, inout SurfaceOutput o) 
	{
	
		float4 mask = tex2D(_MainTex, IN.uv_MainTex);
		clip(mask.a - .1);

		float4 screenUV = IN.grabPos;

		half3 n = normalize(IN.worldNormal);
		half3 v = normalize(IN.viewDir);

		float4 color = tex2Dproj(_GrabTexture, screenUV);
		half rim = 1.0 - saturate(dot (v, n));
        float3 rimColor = _Color.rgb * pow (rim, _RimPower);
		
		
		o.Emission = lerp( color, rimColor, GetIntensity(n,v)*pow (rim, _RimPower) );		

		o.Alpha = color.a;

	}
	ENDCG


		

		
	}
}