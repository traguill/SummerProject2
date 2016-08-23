Shader "FogOfWar/FogOfWar_Proj"
{
	Properties{
		_Color("Fog Color", Color) = (1, 1, 1, 1)
		_Attenuation("Falloff", Range(0.0, 1.0)) = 1.0
		_ShadowTex("Cookie", 2D) = "gray" {}
	}
		Subshader{
		Tags{ "Queue" = "Transparent" }
		Pass{
			ZWrite Off
			ColorMask RGB
			Blend One OneMinusSrcAlpha 
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float2 uvShadow : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			float4x4 _Projector;
			float4x4 _ProjectorClip;
			float4 _ShadowTex_ST;

			v2f vert(float4 vertex : POSITION)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, vertex);
				o.uvShadow = TRANSFORM_TEX(mul(_Projector, vertex).xy, _ShadowTex);
				return o;
			}

			sampler2D _ShadowTex;
			fixed4 _Color;

			fixed4 frag(v2f i) : COLOR
			{

				fixed4 texCookie = tex2D(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				
				if (texCookie.g == 1)
				{
						texCookie = fixed4(0.0, 0.0, 0.0, 0.0); //Transparent
				}	
				else
				{
					if (texCookie.g == 0)
						texCookie = _Color; //Full fog
					else
						texCookie = fixed4(_Color.r, _Color.g, _Color.b, (texCookie.b) * _Color.a);
				}
					

				

				return texCookie;

			}
				ENDCG
		}
	}
}