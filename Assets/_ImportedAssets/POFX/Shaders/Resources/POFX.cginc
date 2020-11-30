#include "UnityCG.cginc"



const float PI = 3.14159265359;
float _Outline;
float _Intensity;
float _IntensityEdges;
float4 _Color;
float4 _GrabTexture_TexelSize;

struct Input {
	float2 uv_MainTex : TEXCOORD0;
	float4 grabPos : TEXCOORD1;
	half3 viewDir : TEXCOORD2;
	float3 worldPos : TEXCOORD3;
	float3 worldNormal : TEXCOORD4;
	float4 customUV : TEXCOORD5;
};





float4x4 inverse(float4x4 input)
{
#define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
	//determinant(float3x3(input._22_23_23, input._32_33_34, input._42_43_44))

	float4x4 cofactors = float4x4(
		minor(_22_23_24, _32_33_34, _42_43_44),
		-minor(_21_23_24, _31_33_34, _41_43_44),
		minor(_21_22_24, _31_32_34, _41_42_44),
		-minor(_21_22_23, _31_32_33, _41_42_43),

		-minor(_12_13_14, _32_33_34, _42_43_44),
		minor(_11_13_14, _31_33_34, _41_43_44),
		-minor(_11_12_14, _31_32_34, _41_42_44),
		minor(_11_12_13, _31_32_33, _41_42_43),

		minor(_12_13_14, _22_23_24, _42_43_44),
		-minor(_11_13_14, _21_23_24, _41_43_44),
		minor(_11_12_14, _21_22_24, _41_42_44),
		-minor(_11_12_13, _21_22_23, _41_42_43),

		-minor(_12_13_14, _22_23_24, _32_33_34),
		minor(_11_13_14, _21_23_24, _31_33_34),
		-minor(_11_12_14, _21_22_24, _31_32_34),
		minor(_11_12_13, _21_22_23, _31_32_33)
		);
#undef minor
	return transpose(cofactors) / determinant(input);
}

float perlinf(float a)
{
	return frac(sin(dot(float2(a, a), float2(12.9898, 78.233))) * 43758.5453);
}



float2 rotuv(float2 uv, float angle, float2 center)
{
	float c = cos(angle);
	float s = sin(angle);
	return mul(float2x2(+c, -s, +s, +c), (uv - center)) + center;
}

float metaBall(float2 uv)
{
	return length(frac(uv) - float2(0.5, 0.5));
}


float RND(float seed) {
	seed = round(seed * 255) / 255;//fix
	return frac(sin(dot(float2(seed, seed*seed), float2(12.9898, 78.233))) * 43758.5453);
}

void vertInflate(inout appdata_base v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
#ifdef SPRITE_RENDERER
	//v.normal = normalize(v.vertex);
#endif

	v.vertex.xyz += normalize(v.normal)*_Outline;	
	o.customUV.xy =( mul((float3x3)UNITY_MATRIX_IT_MV,v.normal) * 0.5 + 0.5).xy ;
#if defined(PIXELSNAP_ON)
	v.vertex = UnityPixelSnap(v.vertex);	
#endif
	
	/*
	o.grabPos = ComputeScreenPos(UnityObjectToClipPos(v.vertex.xyz));
	COMPUTE_EYEDEPTH(o.grabPos.z);
	o.grabPos.xy /= o.grabPos.w;
	*/
	o.grabPos = ComputeGrabScreenPos(UnityObjectToClipPos(v.vertex.xyz));
	//v.normal = normalize( v.vertex );
	o.uv_MainTex = v.texcoord.xy;
}






void vertOutline(inout appdata_base v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
	
	//o.customUV = ComputeScreenPos(UnityObjectToClipPos(v.vertex));

#ifdef SPRITE_RENDERER
	v.normal = normalize(v.vertex);
#endif
	float d = dot(normalize(v.normal), COMPUTE_VIEW_NORMAL);

	float4 pos = UnityObjectToClipPos(v.vertex);
	float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, normalize(v.normal));
	norm = normalize(norm);
	
	o.uv_MainTex.xy = v.texcoord;

	o.customUV.xy =( mul((float3x3)UNITY_MATRIX_IT_MV,v.normal) * 0.5 + 0.5).xy ;

	float2 offset = TransformViewToProjection(norm.xy);
	pos.xy += offset * _Outline;

	v.vertex = mul(inverse(UNITY_MATRIX_MVP), pos);
#if defined(PIXELSNAP_ON)
	v.vertex = UnityPixelSnap(v.vertex);
#endif
	 
	o.grabPos = ComputeGrabScreenPos(UnityObjectToClipPos(v.vertex.xyz));
	//COMPUTE_EYEDEPTH(o.grabPos.z);
}


void vertMask(inout appdata_base v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
	v.vertex.xyz += normalize(v.normal)*.0001;

	//float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, normalize(v.normal));
	//o.customUV.xy = normalize( norm.xy );
	//o.customUV.xy =UnityObjectToClipPos(v.normal).xy;

	o.customUV.xy = (mul((float3x3)UNITY_MATRIX_IT_MV, v.normal) * 0.5 + 0.5).xy;
#if defined(PIXELSNAP_ON)
	v.vertex = UnityPixelSnap(v.vertex);
#endif

	o.grabPos = ComputeGrabScreenPos(UnityObjectToClipPos(v.vertex.xyz));
	//COMPUTE_EYEDEPTH(o.grabPos.z);
}

void vertAura(inout appdata_base v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
	
	float d = dot(normalize(v.normal), COMPUTE_VIEW_NORMAL);

	float3 center = float3(0,1.6,0);

	float4 pos = UnityObjectToClipPos(v.vertex);
	float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, normalize(v.vertex-center));
	norm = normalize(norm);
	
	o.customUV = ComputeGrabScreenPos(UnityObjectToClipPos(v.vertex));

	//o.customUV.xy = pos.xy;
	o.customUV.z = d;
	//o.customUV.w =   length(offset * (1-norm.z));
	//o.customUV = pos;

	float2 offset = TransformViewToProjection(norm.xy);
	pos.xy += offset * _Outline;

	
	v.vertex = mul(inverse(UNITY_MATRIX_MVP), pos);
	
}

fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
	fixed4 c;
	c.rgb = s.Albedo;
	c.a = s.Alpha;
	return c;
}



float GetIntensity(half3 n, half3 v)
{
	half rim = 1.0 - saturate(dot(v, n));
	float intensity = pow(rim, abs(_IntensityEdges));
	intensity = _IntensityEdges > 0 ? intensity : 1 - intensity;
	intensity = lerp(1, intensity, abs(_IntensityEdges))*_Intensity;
	return intensity;
}

float GetIntensity(Input IN)
{
	half3 n = normalize(IN.worldNormal);
	half3 v = normalize(IN.viewDir);
	return GetIntensity(n, v);
}