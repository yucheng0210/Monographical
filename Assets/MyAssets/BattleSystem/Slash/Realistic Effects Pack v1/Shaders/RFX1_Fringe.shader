Shader "KriptoFX/RFX1/Fringe" {
Properties {
	[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Speed("Distort Speed", Float) = 1
	_Scale("Distort Scale", Float) = 1
	_CutoutMap("Cuout Tex (r)", 2D) = "black" {}
	_Cutout("Cutout", Range(0, 1)) = 1
	_InvFade ("Soft Particles Factor", Float) = 3

}

Category {
	Tags { "Queue"="Transparent-150" "IgnoreProjector"="True" "RenderType"="Transparent"}
	Blend SrcAlpha One
	
	Cull Off
	ZWrite Off

	SubShader {
		Pass {

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_instancing

			#pragma target 4.6

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/AtmosphericScattering/AtmosphericScattering.hlsl"

			sampler2D _MainTex;
			sampler2D _CutoutMap;
			half4 _TintColor;
			half _Cutout;
			half _Speed;
			half _Scale;
			float4x4 _InverseTransformMatrix;
		
			struct appdata_t {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				float4 screenPos : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 _MainTex_ST;
			float4 _CutoutMap_ST;

			inline float4 ComputeNonStereoScreenPos(float4 pos) {
				float4 o = pos * 0.5f;
				o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
				o.zw = pos.zw;
				return o;
			}

			inline float4 ComputeScreenPos(float4 pos) {
				float4 o = ComputeNonStereoScreenPos(pos);
#if defined(UNITY_SINGLE_PASS_STEREO)
				o.xy = TransformStereoScreenSpaceTex(o.xy, pos.w);
#endif
				return o;
			}

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.screenPos = ComputeScreenPos (o.vertex);

				_TintColor.rgb = _TintColor.rgb * _TintColor.rgb * 8;
				o.color = v.color * _TintColor;
				o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				float2 pos = mul(_InverseTransformMatrix, float4(v.vertex.xyz, 1)).xz;
				o.texcoord.zw = (pos - 0.5) * _CutoutMap_ST.xy + _CutoutMap_ST.zw;
			
				return o;
			}


			float _InvFade;

			half4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				//#ifdef SOFTPARTICLES_ON
				float2 screenUV = i.screenPos.xy / i.screenPos.w;
				float depth = SampleCameraDepth(screenUV);
				float sceneZ = LinearEyeDepth(depth, _ZBufferParams);
				float partZ = i.screenPos.w;
				float fade = 1 - saturate (_InvFade * (sceneZ-partZ));
				float fade2 = 1 - saturate(_InvFade * (sceneZ - partZ) * 5);

				float2 tex1 = tex2D(_MainTex, i.texcoord + _Time.x * _Speed * half2(0, -0.5)).xy;
				float tex2 = tex2D(_MainTex, i.texcoord + tex1.xy * _Scale + _Time.x * _Speed * half2(-2, 3)).b;
				half4 col = fade2 * i.color + i.color * tex2 * tex2 * fade * 2;
				half cutoutAlpha = tex2D(_CutoutMap, i.texcoord.zw).r;
				half alpha = (pow(1 - cutoutAlpha + _Cutout, 50));
				col.a *= saturate(alpha * pow(_Cutout, .2));
				

				half3 fogColor;
				half3 fogOpacity;
				PositionInputs posInput = GetPositionInput(i.vertex.xy, _ScreenSize.zw, i.vertex.z, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
				EvaluateAtmosphericScattering(posInput, 1, fogColor, fogOpacity);
				col.rgb = col.rgb * (1 - fogOpacity) + fogColor;
				col.a = col.a * (1 - fogOpacity.x);

				return col;

			}
			ENDHLSL
		}
	}
}
}
