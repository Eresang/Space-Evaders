// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Explosion" {
	Properties {
		_RampTex("Texture Ramp", 2D) = "white" {}
		_NoiseTex ("Noise", 2D) = "gray" {}
		_Heat ("Heat", float) = 0.75
		_Radius("Radius", float) =1
		_Frequency ("Noise Frequency", float) = 1
		_ScrollSpeed("ScrollSpeed", float) = 1
		_Alpha ("Transparency", float) = 1
		_Quality("Quality", vector) = (0.1,15,8,.15)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent+100"  "IgnoreProjector"="True" }
		LOD 1000

		Pass
		{
			Colormask 0
			Zwrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex - float4(v.normal * 0.25, 0));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(1,1,1,1);
			}
			ENDCG
		}
		
		Pass{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			//#pragma glsl
			
			sampler2D _RampTex;
			sampler2D _NoiseTex;
			float _Heat;
			float _Radius;
			float _Frequency;
			float _ScrollSpeed;
			float _Alpha;
			float4 _Quality;
			
			struct v2f {
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				float3 viewVec : TEXCOORD1;
				float4 sphere : TEXCOORD2;
			};
			
			v2f vert (appdata_base v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.viewVec = WorldSpaceViewDir(v.vertex);
				o.sphere.xyz = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
				return o;
			}
			
			float noise (float3 p){
				float f = frac(p.y);
				float i = floor(p.y);
				float2 rg = tex2Dlod(_NoiseTex, float4(p.xz + float2(37, 13) * i, 0, 0)/64).yx;
				return lerp(rg.x, rg.y, f);
			}
			
			float fbm(float3 p){
				p += _Frequency;
				float v = 0;
				float4 offset = _Time * _ScrollSpeed;
				v += noise (p + offset.y);
				p *= 2;
				v += noise(p + offset.z)/2;
				p *= 2;
				v += noise(p + offset.z)/4; p *= 2;
				v += noise(p + offset.w)/8; p *= 2;
				v += noise(p + offset.w)/16; p *= 2;
				return v;
			}
			
			float distf (float4 sphere, float3 p){
				return distance (sphere.xyz, p) - _Radius - fbm(p);
			}
			
			float4 march (float4 sphere, float3 p, float3 v){
				float dist;
				for(int i = 0; i < _Quality.y; ++i){
					dist = distf(sphere, p);
					if(dist < _Quality.x) return float4 (p, 0);
					p -= v*(dist + 0.02);
				}
				return float4 (-100,-100,-100,-100);
			}
			
	
			
			float2 heat (float4 sphere, float3 p, float3 d){
				float heat = 0;
				float dens = 0;
				float fac = .5;
				d *= _Quality.w;
				for (int i = 0; i < _Quality.z; i ++){
					float dis = distf(sphere, p);
					if(dis <= _Quality.x){
						heat += pow ((_Radius - distance(p, sphere.xyz) + 2.5) * fac * _Heat, 3);
						fac *= .25;
						dens += _Quality.w * 2;
						p -= d;	
					} else {
						p -= d * 3;
					}
				}
				return float2 (heat, dens);
			}	
		
			
			fixed4 frag (v2f i) : COLOR {
				float4 m = march (i.sphere, i.worldPos, normalize (i.viewVec));
				float2 hd = heat(i.sphere, m.xyz, normalize(i.viewVec));
				half4 col = tex2Dlod(_RampTex, float4(hd.x, 0, 0, 0));
				col.w = saturate(saturate(hd.y) * _Alpha);			
				clip(m.w);
				
				return col;
			}
			ENDCG
		}
	}
	//fallback is commented out during development
	//fallback "Diffuse"
}
