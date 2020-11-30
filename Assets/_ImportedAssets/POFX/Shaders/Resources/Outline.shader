Shader "POFX/Outline" {
	Properties
	{
		_MainTex("Maintex", 2D) = "white" {}		
		_Color("Color", Color) = (1,1,1,1)
		_outline("Outline", float) = 0.0
		_Intensity("Intensity", Float) = 1
		_StencilRef("Stencil Ref", Float) = 128
	}

		SubShader{

		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+10" }
		LOD 200
		Cull Off
		
		ZTest Always
		//ZTest Greater

		Zwrite off
		Offset -1,-1

		GrabPass
		{
		}

		Stencil{
			Ref[_StencilRef]
			Comp always
			Pass zero
		}

		CGPROGRAM
		#pragma surface surf NoLighting nolightmap
		#pragma vertex vertOutline
		#pragma multi_compile __ SPRITE_RENDERER
		//#define UNITY_PASS_FORWARDBASE
		#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
		#pragma target 3.0
		#include "POFX.cginc"

		sampler2D _GrabTexture;
		sampler2D _MainTex;
		void surf(Input IN, inout SurfaceOutput o)
		{
			float4 screenUV = IN.grabPos;
			float4 color = tex2Dproj(_GrabTexture, screenUV);
			o.Emission = color.rgb;
		}
		ENDCG


		Stencil{
		Ref[_StencilRef]
		Comp always
		Pass replace
		}
		
		
		CGPROGRAM
		#pragma surface surf NoLighting nolightmap
		#pragma vertex vertMask
		#pragma multi_compile __ SPRITE_RENDERER
		#pragma target 2.0
		#include "POFX.cginc"

		sampler2D _GrabTexture: register(s0);
		sampler2D _MainTex;
		void surf(Input IN, inout SurfaceOutput o) {
		float2 screen = IN.grabPos.xy;
		float4 color = tex2Dproj(_GrabTexture, IN.grabPos);
		o.Emission = color.rgb;

		}
		ENDCG
	
			
		
		Stencil{
		Ref [_StencilRef]
		Comp NotEqual
		Pass keep
		//Fail keep
		}
	
		//Cull front
			Cull off
		ZTest LEqual
		
		Zwrite On
		
		CGPROGRAM
		#pragma surface surf NoLighting nolightmap
		#pragma vertex vertOutline
		#pragma multi_compile __ SPRITE_RENDERER
		#pragma target 2.0
		#include "POFX.cginc"


	sampler2D _GrabTexture;
	sampler2D _MainTex;
	
	void surf(Input IN, inout SurfaceOutput o) {
		
		float4 screenUV = IN.grabPos;

		float4 mask = tex2D(_MainTex, IN.uv_MainTex);
		clip(mask.a - 0.1);

		half3 n = normalize(IN.worldNormal);
		half3 v = normalize(IN.viewDir);

		float4 color = lerp(tex2Dproj(_GrabTexture, screenUV), _Color, _Color.a*GetIntensity(n,v));

		o.Alpha = _Color.a;
		o.Emission = color.rgb;

	}
	ENDCG
	

		

	}
}