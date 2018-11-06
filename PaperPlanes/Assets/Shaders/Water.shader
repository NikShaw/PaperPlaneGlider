Shader "Custom/Water"
{
	Properties
	{
		_Col1("Water Colour", Color) = (1, 1, 1, 1)
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
#pragma target 3.0
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
				float2 uv : TEXCOORD1;
				//float4 vertex : SV_POSITION;
				float4 pos : SV_POSITION;
				//float4 worldpos : TEXCOORD3;
				float4 normal : NORMAL;
				//nointerpolation fixed4 diff : COLOR0;
				//nointerpolation fixed3 ambient : COLOR1;
				//SHADOW_COORDS(1) // put shadows data into TEXCOORD1
//LIGHTING_COORDS(0,1)
				//UNITY_FOG_COORDS(1)
			};

			float4 _Col1;

			float GetYAt(float2 pos) {
				float y = 0;
				y = sin(pos.x + pos.y + _Time.y) * 1;
				return y;
			}

			v2f vert(appdata v) {
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				float3 v1 = v.vertex.xyz + float3(1, 0, 0);
				float3 v2 = v.vertex.xyz + float3(0, 0, 1);
				v.vertex.y = GetYAt(v.vertex.xz);
				o.pos = UnityObjectToClipPos(v.vertex);
				
				
				//v1.y = GetYAt(v1.xz);
				//v2.y = GetYAt(v2.xz);
				//float3 vn = cross(v2 - v.vertex.xyz, v1 - v.vertex.xyz);
				//v.normal =  float4(normalize(vn).xyz, 1);
				//o.worldpos = v.vertex;
				//o.pos = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _Tex1);
				//o.normal = v.normal;
				//half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				//half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				//float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				//o.diff = nl * _LightColor0;
				//o.ambient = ShadeSH9(half4(worldNormal, 1));
				//o.worldRefl = reflect(-worldViewDir, worldNormal);
				// compute shadows data
				//if (o.dist < _ShadDist) {
				//TRANSFER_SHADOW(o)
					//}
					//UNITY_TRANSFER_FOG(o, o.pos);
				//half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				//half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				//o.diff = nl * _LightColor0;
				//o.ambient = ShadeSH9(half4(worldNormal, 1));
				// compute shadows data
				//TRANSFER_SHADOW(o)
				//	TRANSFER_VERTEX_TO_FRAGMENT(o)
				//UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				/*float height = i.worldpos.y;
				float slope = 1.0 - i.normal.y;
				float blendAmount = 0.0f;*/
				fixed4 col1 = _Col1;
				//fixed4 col2;

				// Determine which texture to use based on height.
				/*if (slope < 0.5) {
					blendAmount = (slope / 0.7f);// *(1.0f / (0.4f - 0.2f));
					col1 = lerp(tex2D(_Tex1, i.uv*24), tex2D(_Tex2, i.uv*24), blendAmount);
				} else {
					col1 = tex2D(_Tex2, i.uv);
				}
				if (height > _SnowHeightMin) {
					col2 = lerp(tex2D(_Tex3, i.uv*24), tex2D(_Tex4, i.uv*24), (height - _SnowHeightMin) / (_SnowHeightMax - _SnowHeightMin));
					col1 = lerp (col1, col2, ((height - _SnowHeightMin) / (_SnowHeightMax - _SnowHeightMin)) * (1.0f - slope));
				}*/
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
				//fixed shadow = SHADOW_ATTENUATION(i);
				//float atten = LIGHT_ATTENUATION(i);
				// darken light's illumination with shadow, keep ambient intact
			   // fixed3 lighting = i.diff * atten + i.ambient;
				//col1.rgb *= lighting;
				//UNITY_APPLY_FOG(i.fogCoord, col1);
				return col1;
			}
			ENDCG
		}
	}
}
