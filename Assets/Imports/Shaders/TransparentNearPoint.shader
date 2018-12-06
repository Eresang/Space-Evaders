// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/TransparentNearPoint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		_Point ("Location", Vector) = (0,0,0,1)
		_Range ("Range", float) = 1
		_Fade ("Fade distance", float) = 0.5
		_NoiseTex ("Noise", 2D) = "white" {}
		_MoveSpeed ("Move Speed", float) = 2
		_ObjectPosition ("Object Position", Vector) = (0,0,0,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha One
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uvNoise : TEXCOORD2;
				float4 worldPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Point;
			float _Range;
			float _Fade;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			float _MoveSpeed;
			float4 _ObjectPosition;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex) - _ObjectPosition;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvNoise = pow(abs(1-(TRANSFORM_TEX(v.uv, _MainTex) * 2)), 2) * _NoiseTex_ST.xy + (_NoiseTex_ST.zw * _Time * _MoveSpeed);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float dist = distance(i.worldPos, _Point);
				fixed4 col = tex2D(_MainTex, i.uv);
				if(dist< _Range){
				col = tex2D(_NoiseTex, i.uvNoise);
				col.a = 0.03;
				} else if (dist > _Range && dist < (_Range + _Fade)){
				col.a =1- (dist - _Range) * (1/_Fade);
				} else if (dist < 2) {
				col.a = 0;
				} else {
				col.a = 0;
				}
				return col;
			}
			ENDCG
		}
	}
}
