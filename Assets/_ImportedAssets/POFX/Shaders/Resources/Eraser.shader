Shader "POFX/Eraser" {
	Properties
	{
			_MainTex("Maintex", 2D) = "white" {}
			//_Intensity("Intensity", Range(0.0, 1.0)) = 1.0
			_Outline("Outline", float) = 0.001
			_Color("Color", Color) = (1,1,1,1)
	}


		SubShader{

		
		Tags{ "RenderType" = "Opaque" "Queue" = "Transparent" }
		LOD 200
		zwrite off
		Offset -1,-1

		GrabPass{
				"_Grab"
			}

				
		Tags{ "RenderType" = "Opaque" "Queue" = "Overlay" }
		Cull off
		zwrite on

				GrabPass{

			}

	CGPROGRAM
#pragma surface surf NoLighting nolightmap
#pragma vertex vertInflate
//#define UNITY_PASS_FORWARDBASE
#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
#pragma target 3.0
#include "POFX.cginc"


	sampler2D _Grab;
	sampler2D _GrabTexture;
	sampler2D _GrabAfter;
	sampler2D _MainTex;
	//float4 _GrabTexture_TexelSize;

	void surf(Input IN, inout SurfaceOutput o) {
		
		float4 screenUV = IN.grabPos;

		float4 mask = tex2D(_MainTex, IN.uv_MainTex);
		clip(mask.a - .1);

		half3 n = normalize(IN.worldNormal);
		half3 v = normalize(IN.viewDir);

		float4 color = tex2Dproj(_Grab, screenUV) * _Color;
		float4 colorOriginal = tex2Dproj(_GrabTexture, screenUV) * _Color;

		o.Alpha = .1;
		o.Emission = lerp(colorOriginal, color.rgb, GetIntensity(n,v));

	}
	ENDCG
	}
			
}