Shader "Projector/ProjectorAdditive"
{
	Properties{
		_Color("Tint Color", Color) = (1, 1, 1, 1)
		_Attenuation("Falloff", Range(0.0, 1.0)) = 1.0
		_ShadowTex("Cookie", 2D) = "gray" {}
	}
		Subshader{
		Tags{ "Queue" = "Transparent" }
		Pass{
			ZWrite Off
			ColorMask RGB
			Blend SrcAlpha One // Additive blending
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float2 uvShadow : TEXCOORD0;
				float4 pos : SV_POSITION;
				fixed4 posProj : TEXCOORD1;
			};

			float4x4 _Projector;
			float4x4 _ProjectorClip;
			float4 _ShadowTex_ST;

			v2f vert(float4 vertex : POSITION)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, vertex);
				o.uvShadow = TRANSFORM_TEX (mul(_Projector, vertex).xy, _ShadowTex);
				o.posProj = mul(_Projector, vertex);
				return o;
			}

			sampler2D _ShadowTex;
			fixed4 _Color;

			fixed4 frag(v2f i) : COLOR
			{
					bool isInClip = i.posProj.w > 0.0 && i.posProj.x > 0 && i.posProj.x < 1 && i.posProj.y > 0 && i.posProj.y < 1;

					fixed4 texCookie = tex2D(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
					fixed4 outColor = _Color * texCookie.a;

					outColor *= clamp(1.0, 0.0, 1.0);
					outColor.a = (outColor.a) * isInClip;

					return outColor.a;
				
			}
				ENDCG
		}
	}
}