Shader "Custom/QuickOutline"
{
	Properties
	{
		_MainCol ("Colour", color) = (1, 1, 1, 1)
		_OutlineCol("Outline Colour", color) = (1, 1, 1, 1)
		_Width("Width", float) = 1
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector" = "True" "RenderType"="Transparent" }
		LOD 100


		Pass
	    {
			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha
			Cull Front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
					// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
			};

			//sampler2D _MainTex;
			float4 _MainCol;
			float4 _OutlineCol;
			float _Width;

			v2f vert(appdata v) {
				v2f o; 
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				//v.normal.y = -v.normal.y;
				o.vertex = UnityObjectToClipPos(v.vertex + (v.normal * _Width));
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = _OutlineCol;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
	    }

		Pass
		{
				ZWrite Off

				Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			//sampler2D _MainTex;
			float4 _MainCol;
			float _Width;
			
			v2f vert (appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _MainCol;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
