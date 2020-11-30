Shader "POFX/Blur" {
	Properties
	{
			_MainTex("Maintex", 2D) = "white" {}
			_Intensity("Intensity", Range(0.0, 1.0)) = 1.0
			_Intensity("Intensity Edges", Range(-1.0, 1.0)) = 0.0
			_BlurNear("BlurNear", Range(0.0, 1000.0)) = 200.0
			_BlurFar("BlurFar", Range(0.0, 1000.0)) = 200.0			
			_DistanceCamFar("Distance cam Far", float) = 10.0
			_Outline("Outline", float) = 0.001
			
	}

		SubShader{

		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+14" }
		LOD 200
		Cull off
		zwrite on
		Offset -1,-1

		GrabPass{ 
				//"_GrabBlur" 
				//Tags{ "Queue" = "Background" }
			}

			
			//Cull back
	CGPROGRAM
#pragma surface surf NoLighting nolightmap 
#pragma vertex vertInflate
//#define UNITY_PASS_FORWARDBASE
#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
#pragma target 3.0
#include "POFX.cginc"

	sampler2D _GrabTexture;
	//sampler2D _GrabBlur;
	sampler2D _MainTex;	
	half _BlurNear;
	half _BlurFar;
	float _DistanceCamFar;
	//float4 _GrabTexture_TexelSize;


	void surf(Input IN, inout SurfaceOutput o) {
			
		
		float4 screenUV = IN.grabPos;

		float4 mask = tex2D(_MainTex, IN.uv_MainTex);
		clip(mask.a - .1);

		half3 n = normalize(IN.worldNormal);
		half3 v = normalize(IN.viewDir);

		//float4 color = tex2Dproj(_GrabTexture, uv);
		float4 color = tex2Dproj(_GrabTexture, screenUV);

		float d = _DistanceCamFar>0 ? (saturate(length(IN.worldPos.xyz - _WorldSpaceCameraPos.xyz) / abs(_DistanceCamFar))) : 1;
		float blurFactor = 1000 / lerp(_BlurNear, _BlurFar, d);

		//blur
		float4 resolution = float4(blurFactor, blurFactor, blurFactor, blurFactor)*1/_Intensity;
		float4 direction = float4(1, 1, 1, 1);
		

		float4 blur = float4(0.0, 0.0, 0.0, 0.0);

		float4 o1 = float4(1, 1, 1, 1)*1.5 *direction;
		float4 o2 = float4(1, 1, 1, 1)*3.5 *direction;
		float4 o3 = float4(1, 1, 1, 1)*5.2 *direction;

		blur += tex2Dproj(_GrabTexture, screenUV) * 0.2;
		blur += tex2Dproj(_GrabTexture, screenUV + (o1 / resolution)) * 0.3;
		blur += tex2Dproj(_GrabTexture, screenUV - (o1 / resolution)) * 0.3;
		blur += tex2Dproj(_GrabTexture, screenUV + (o2 / resolution)) * 0.1;
		blur += tex2Dproj(_GrabTexture, screenUV - (o2 / resolution)) * 0.1;
		blur += tex2Dproj(_GrabTexture, screenUV + (o3 / resolution)) * 0.01;
		blur += tex2Dproj(_GrabTexture, screenUV - (o3 / resolution)) * 0.01;
		

		color = lerp(color, blur, GetIntensity(n,v));
		o.Alpha = color.a;
		o.Emission = color.rgb;


	}
	ENDCG
	}
}