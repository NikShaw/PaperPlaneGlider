Shader "Custom/Water2"
{
	Properties
	{
		_WaveTex("Wave Tex", 2D) = "white" {}
	    _Col1("Water Colour", Color) = (1, 1, 1, 1)
		_Col2("Water Colour2", Color) = (1, 1, 1, 1)
		_WaveHeight("Wave Height", float) = 0.25
		_WaveDir("Wave Direction", vector) = (0, 1, 0, 0)
		_WavePos("Wave Position", vector) = (0, 0, 0, 0)
		_WaveSpeed("Wave Speed", float) = 1
		_WaveLength("Wave Length", float) = 1
	}
	SubShader
	{
		Tags { "Queue" = "Transparent"  "RenderType"="Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
#pragma target 3.5
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
#ifdef UNITY_COMPILER_HLSL2GLSL
#define nointerpolation flat
#endif
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				nointerpolation fixed4 diff : COLOR0;
				nointerpolation fixed3 ambient : COLOR1;
				SHADOW_COORDS(4)
				UNITY_FOG_COORDS(5)
			};

			sampler2D _WaveTex;
			float4 _WaveTex_ST;
			float4 _Col1;
			float4 _Col2;
			float2 _WavePos;
			float2 _WaveDir;
			float _WaveHeight;
			float _WaveSpeed;
			float _WaveLength;

			float GetYAtTex(float2 pos) {
				float y = 0;
				float2 wavePos = pos + _WavePos;

				y = tex2Dlod(_WaveTex, float4(wavePos.xy, 0, 0) / 256 * (1/ _WaveLength)).r * _WaveHeight;

				return y;
			}

			float GetYAt(float2 pos) {
				float y = 0;
				float2 waveDir = normalize(_WaveDir);
				y = sin((((pos.x * waveDir.x)*_WaveLength) + _WavePos.x) + (((pos.y * waveDir.y)*_WaveLength) + _WavePos.y) + (_Time.y*_WaveSpeed)) * _WaveHeight;
				return y;
			}

			float randy(float3 co) {
				float3 tmpCo = co + (float3(_WavePos.x, 0, _WavePos.y)/10000);
				return frac(sin(dot(tmpCo.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
			}

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				v.vertex.y += GetYAtTex(worldPos.xz);
				o.vertex = UnityObjectToClipPos(v.vertex);

				float3 v1 = worldPos.xyz - float3(1000, 1000, 1000) + float3(-20, 0, 0);
				float3 v2 = worldPos.xyz - float3(1000, 1000, 1000) + float3(0, 0, -20);
				v1.y = GetYAtTex(v1.xz);
				v2.y = GetYAtTex(v2.xz);
				float3 vn = cross(v2 - v.vertex.xyz, v1 - v.vertex.xyz);
				v.normal =  float4(normalize(vn).xyz, 1);

				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl * _LightColor0;
				o.ambient = ShadeSH9(half4(worldNormal, 1));
				TRANSFER_SHADOW(o)
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = _Col1 +_Col2;
			    fixed shadow = SHADOW_ATTENUATION(i);
				fixed3 lighting = 0;
				lighting = clamp((i.diff * 1) * shadow + i.ambient, 0.5, 1);
				col.rgb *= lighting;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
