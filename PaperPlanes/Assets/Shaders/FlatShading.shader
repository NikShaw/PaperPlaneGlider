// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Flat"
{
	Properties
	{
		_Tex1("Ground tex", 2D) = "white" {}
	_Tex2("Rock tex", 2D) = "white" {}
	_Tex3("Snow1 tex", 2D) = "white" {}
	_Tex4("Snow2 tex", 2D) = "white" {}
	_ShadDist("Shadow distance", float) = 250
		_ShadDistFade("Shadow distance fade perc", range(0, 1)) = 0.9
		_SnowHeightMin("Snow height min", float) = 10
		_SnowHeightMax("Snow height max", float) = 15
	}
		SubShader
	{
		//Tags { "RenderType"="Opaque" }
		//LOD 100

		Pass
	{
		Tags{ "LightMode" = "ForwardBase" }
		CGPROGRAM
#pragma target 2.0
#pragma vertex vert
#pragma fragment frag
		// make fog work
#pragma multi_compile_fog

#include "UnityCG.cginc"
		//  #include "UnityLightingCommon.cginc"
#include "Lighting.cginc"

		// compile shader into multiple variants, with and without shadows
		// (we don't care about any lightmaps yet, so skip these variants)
#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
		// shadow helper functions and macros
#include "AutoLight.cginc"

		struct appdata {
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float4 normal : NORMAL;
	};

	struct v2f {
		//float4 vertex : SV_POSITION;
		float4 pos : SV_POSITION;
		float4 normal : NORMAL;
		float4 normal2 : TEXCOORD6;
		float2 uv : TEXCOORD0;
		float4 worldPos : TEXCOORD2;
		float4 screenPosition : TEXCOORD3;
		half3 worldRefl : TEXCOORD4;
		float dist : TEXCOORD7;
		nointerpolation fixed4 diff : COLOR0;
		nointerpolation fixed3 ambient : COLOR1;
		SHADOW_COORDS(1) // put shadows data into TEXCOORD1
			UNITY_FOG_COORDS(5)
	};

	sampler2D _Tex1;
	float4 _Tex1_ST;
	sampler2D _Tex2;
	float4 _Tex2_ST;
	sampler2D _Tex3;
	float4 _Tex3_ST;
	sampler2D _Tex4;
	float4 _Tex4_ST;
	float _SnowHeightMin;
	float _SnowHeightMax;
	float _ShadDist;
	float _ShadDistFade;

	//UNITY_INSTANCING_CBUFFER_START(Props)
	v2f vert(appdata v) {
		v2f o;
		float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
		o.worldPos = worldPos;
		o.screenPosition = UnityObjectToClipPos(v.vertex);
		o.dist = distance(_WorldSpaceCameraPos, worldPos);
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _Tex1);
		o.normal = normalize(mul(v.normal, unity_WorldToObject));
		o.normal2 = normalize(v.normal);
		half3 worldNormal = UnityObjectToWorldNormal(v.normal);
		half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
		float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
		o.diff = nl * _LightColor0;
		o.ambient = ShadeSH9(half4(worldNormal, 1));
		//o.worldRefl = reflect(-worldViewDir, worldNormal);
		// compute shadows data
		//if (o.dist < _ShadDist) {
		TRANSFER_SHADOW(o)
			//}
			UNITY_TRANSFER_FOG(o,o.pos);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float height = i.worldPos.y;
	float slope = 1.0 - i.normal.y;
	float blendAmount = 0.0f;
	float attenuation = 1.0;
	fixed4 col1;
	fixed4 col2;

	//float4 lightDirection = normalize(_WorldSpaceLightPos0);
	//float4 diffuseTerm = saturate(dot(lightDirection, i.normal));
	//float4 diffuseLight = diffuseTerm * _LightColor0;

	//float4 cameraPosition = normalize(float4(_WorldSpaceCameraPos, 1) - i.worldPos);
	//float3 viewDirection = normalize(_WorldSpaceCameraPos - i.worldPos.xyz);
	// Blinn-Phong
	//float4 halfVector = normalize(lightDirection + cameraPosition);
	//float4 specularTerm = pow(saturate(dot(i.normal2, halfVector)), 6);


	// sample the default reflection cubemap, using the reflection vector
	//half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.worldRefl);
	// decode cubemap data into actual color
	//half3 skyColor = DecodeHDR(skyData, unity_SpecCube0_HDR);

	// Determine which texture to use based on height.
	if (slope < 0.3) {
		col1 = tex2D(_Tex1, i.uv * 1);
	} else if (slope < 0.6) {
		blendAmount = (slope - 0.3) / 0.3;// (slope / 0.7f);// *(1.0f / (0.4f - 0.2f));
		col1 = lerp(tex2D(_Tex1, i.uv * 1), tex2D(_Tex2, i.uv * 1), blendAmount * 0.5);
	} else {
		col1 = tex2D(_Tex2, i.uv);
	}
	if (height > _SnowHeightMin) {
		col2 = lerp(tex2D(_Tex3, i.uv * 1), tex2D(_Tex4, i.uv * 1), (height - _SnowHeightMin) / (_SnowHeightMax - _SnowHeightMin));
		col1 = lerp(col1, col2, ((height - _SnowHeightMin) / (_SnowHeightMax - _SnowHeightMin)) * (1.0f - slope));
	}
	//col1 = tex2D(_Tex3, i.uv*24);
	/*if ((slope < 0.7) && (slope >= 0.2f)) {
	blendAmount = (slope - 0.2f) * (1.0f / (0.7f - 0.2f));
	col1 = lerp(slopeColor, rockColor, blendAmount);
	}
	if (slope >= 0.7) {
	col1 = rockColor;
	}*/


	// sample the texture
	//fixed4 col = tex2D(_Tex1, i.uv);
	// apply fog
	// compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
	fixed shadow = SHADOW_ATTENUATION(i);

	if (i.dist >= _ShadDist) {
		shadow = 1;
	}

	//	//float3 specularReflection;
	//if (dot(i.normal2, lightDirection) < 0.0)
	// light source on the wrong side?
	//	{
	//	specularReflection = float3(0.0, 0.0, 0.0);
	// no specular reflection
	//} else // light source on the right side
	//{
	//	specularReflection = attenuation * _LightColor0.rgb
	//		* _SpecColor.rgb * pow(max(0.0, dot(
	//			reflect(-lightDirection, i.normal2),
	//			viewDirection)), _Shininess);
	//}

	// darken light's illumination with shadow, keep ambient intact
	fixed3 lighting = 0;
	/*if (i.dist < _ShadDist) {
	if (i.dist < (_ShadDist * _ShadDistFade)) {
	lighting = i.diff * shadow + i.ambient;
	} else {
	float tmpShad = lerp(shadow, 1, (i.dist - (_ShadDist * _ShadDistFade))/(_ShadDist - (_ShadDist * _ShadDistFade)));
	lighting = i.diff * tmpShad + i.ambient;
	}
	} else {
	shadow = 1;
	lighting = i.diff + i.ambient;
	}*/
	lighting = (i.diff * 1.15) * shadow + i.ambient;
	col1.rgb *= lighting;
	//col1.rgb += (specularTerm*0.05) * skyColor;
	//col1.rgb += specularTerm * shadow;
	//col1.rgb = i.normal2.xyz;
	UNITY_APPLY_FOG(i.fogCoord, col1);
	return col1;
	}
		//	UNITY_INSTANCING_CBUFFER_END
		ENDCG
	}

		// shadow casting support
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}
