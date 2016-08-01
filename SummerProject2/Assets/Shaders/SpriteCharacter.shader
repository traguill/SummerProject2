Shader "Sprite/SpriteCharacter"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
	}
		SubShader
	{
			Pass
			{

			Tags
			{
			"Queue" = "Transparent+100"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest Always
			Blend One OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _Color;

			struct vertexInput {
				float4 vertex : POSITION;
				float4 tex : TEXCOORD0;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				float scaleX = length(mul(_Object2World, float4(1.0, 0.0, 0.0, 0.0)));
				float scaleY = length(mul(_Object2World, float4(0.0, 1.0, 0.0, 0.0)));

				output.pos = mul(UNITY_MATRIX_P,
					mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
					- float4(input.vertex.x * scaleX, input.vertex.y * scaleY, 0.0, 0.0));


				output.tex = input.tex;

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				float4 col = tex2D(_MainTex, float2(input.tex.xy));

				float4 final_col = _Color;
				 final_col *= col.a;

				return final_col;
			}
				ENDCG
			}
		Pass
		{
			
			Tags
			{
			"Queue" = "Transparent+100"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;

			struct vertexInput{
				float4 vertex : POSITION;
				float4 tex : TEXCOORD0;
			};

			struct vertexOutput{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				float scaleX = length(mul(_Object2World, float4(1.0, 0.0, 0.0, 0.0)));
				float scaleY = length(mul(_Object2World, float4(0.0, 1.0, 0.0, 0.0)));

				output.pos = mul(UNITY_MATRIX_P,
					mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
					- float4(input.vertex.x * scaleX, input.vertex.y * scaleY, 0.0, 0.0));


				output.tex = input.tex;

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				float4 col = tex2D(_MainTex, float2(input.tex.xy));

				col.rgb *= col.a;

				return col;
			}
			ENDCG
		}
	}
}
