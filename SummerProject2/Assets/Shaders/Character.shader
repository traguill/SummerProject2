Shader "Characters/Character"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,1)
	}
	
	SubShader 
	{
			Pass{
				CGPROGRAM

				#pragma vertex vert  
				#pragma fragment frag 

				uniform sampler2D _MainTex;
				fixed4 _Tint;

				struct vertexInput {
					float4 vertex : POSITION;
					float4 texcoord : TEXCOORD0;
				};
				struct vertexOutput {
					float4 pos : SV_POSITION;
					float4 tex : TEXCOORD0;
				};

				vertexOutput vert(vertexInput input)
				{
					vertexOutput output;

					output.tex = input.texcoord;			
					output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
					return output;
				}
				float4 frag(vertexOutput input) : COLOR
				{
					fixed4 c = tex2D(_MainTex, input.tex.xy);

					c *= _Tint;

					return c;
				}

					ENDCG
			}
		}
}
