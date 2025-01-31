Shader "Voxel"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[MaterialToggle] _TextureOn ("Texture On", float) = 1
		[MaterialToggle] _LightingOn ("Lighting On", float) = 1
		[MaterialToggle] _AOOn ("AO On", float) = 1
	}
	SubShader
	{
		Tags { "Queue" = "AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
		LOD 100
		Lighting Off
		
		Pass {
			CGPROGRAM
			#pragma vertex vertFunction
			#pragma fragment fragFunction
			#pragma target 4.5

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
				// This is the light value on this vertex
				// With x being the sky light and y being the block light
				float2 light : TEXCOORD1;
				// These are 0 or 1 for each side of the block
				// x is side1 y is side2 z is side3
				float3 sides : TEXCOORD2;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
				// sample float2 light : TEXCOORD1;
				// sample float3 sides : TEXCOORD2;
			};

			sampler2D _MainTex;

			float _TextureOn;
			float _LightingOn;
			float _AOOn;
			
			float GlobalLightLevel;
			float minGlobalLightLevel;
			float maxGlobalLightLevel;
			float4 lightColors[256];
			
			float inverseLerp (int a,int b, float t) {
				return (t - a) / (b - a);
			}

			v2f vertFunction (appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}

			fixed4 fragFunction (v2f i) : SV_Target
			{
				fixed4 col;
				if(_TextureOn)
				{
					col = tex2D(_MainTex, i.uv);
				} else
				{
					col = fixed4(1,1,1,1);
				}
				
				clip(col.a - 1);

				if (_AOOn) col *= i.color;
				
				return col;
			}



			ENDCG
		}
	}
}