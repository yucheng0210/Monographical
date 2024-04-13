Shader "KriptoFX/RFX1/Particle" {
	Properties {
	[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,1)
	_MainTex ("Particle Texture", 2D) = "white" {}
	 [HideInInspector]_Cutout ("_Cutout", Float) = 0.2
	 [HideInInspector]_InvFade ("Soft Particles Factor", Float) = 1.0
	 [HideInInspector]_FresnelStr ("Fresnel Strength", Float) = 1.0
	 [HideInInspector]SrcMode ("SrcMode", int) = 5
     [HideInInspector]DstMode ("DstMode", int) = 10
	 [HideInInspector]CullMode ("Cull Mode", int) = 2 //0 = off, 2=back
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "RFX1"="Particle"}
				Blend [SrcMode] [DstMode]
				Cull [CullMode]
				ZWrite Off

	SubShader {
		Pass {

		HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6

			#pragma multi_compile_local BlendAdd BlendAlpha BlendMul BlendMul2
			#pragma multi_compile_local VertLight_OFF VertLight4_ON VertLight4Normal_ON
			#pragma shader_feature_local FrameBlend_OFF

			#pragma multi_compile_local Clip_OFF Clip_ON Clip_ON_Alpha
			#pragma multi_compile_local FresnelFade_OFF FresnelFade_ON
#pragma shader_feature_local SoftParticles_ON


			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/AtmosphericScattering/AtmosphericScattering.hlsl"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float _Cutout;
			half _FresnelStr;
			half _BloomThreshold;
			float4 _DepthPyramidScale;

			float _InvFade;

			struct appdata_t {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				half4 color : COLOR;
#ifdef FrameBlend_OFF
				float2 texcoord : TEXCOORD0;
#else
#if UNITY_VERSION == 600
				float4 texcoords : TEXCOORD0;
				float texcoordBlend : TEXCOORD1;
#else
				float2 texcoord : TEXCOORD0;
				float4 texcoordBlendFrame : TEXCOORD1;
#endif
#endif

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
#ifdef FrameBlend_OFF
				float2 texcoord : TEXCOORD0;
#else
				float4 texcoord : TEXCOORD0;
				float blend : TEXCOORD1;
#endif
			
#if  SoftParticles_ON
				float4 screenPos : TEXCOORD3;
#endif

#ifdef FresnelFade_ON
				float fresnel : TEXCOORD4;
#endif


				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO

			};

			float3 ObjSpaceViewDir(float4 v)
			{
				float3 objSpaceCameraPos = mul(UNITY_MATRIX_I_M, float4(GetCameraRelativePositionWS(_WorldSpaceCameraPos), 1)).xyz;
				return objSpaceCameraPos - v.xyz;
			}


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

			float3 GetWorldSpacePositionFromDepth(float2 uv, float deviceDepth)
			{
				float4 positionCS = float4(uv * 2.0 - 1.0, deviceDepth, 1.0);
#if UNITY_UV_STARTS_AT_TOP
				positionCS.y = -positionCS.y;
#endif
				float4 hpositionWS = mul(UNITY_MATRIX_I_VP, positionCS);
				return hpositionWS.xyz / hpositionWS.w;
			}


			v2f vert (appdata_t v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


				o.vertex = TransformObjectToHClip(v.vertex.xyz);

#if  SoftParticles_ON
				o.screenPos = ComputeScreenPos(o.vertex);
#endif

				o.color = v.color;
				//o.color.rgb *= ComputeVertexLight(v.vertex, v.normal);

#ifdef FrameBlend_OFF
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
#else
#if UNITY_VERSION == 600
				o.texcoord.xy = TRANSFORM_TEX(v.texcoords.xy, _MainTex);
				o.texcoord.zw = TRANSFORM_TEX(v.texcoords.zw, _MainTex);
				o.blend = v.texcoordBlend;
#else
				o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord.zw = TRANSFORM_TEX(v.texcoordBlendFrame.xy, _MainTex);
				o.blend = v.texcoordBlendFrame.z;
#endif
#endif
#ifdef FresnelFade_ON
				o.fresnel = abs(dot(normalize(v.normal), normalize(ObjSpaceViewDir(v.vertex))));
				o.fresnel = saturate((pow(o.fresnel, _FresnelStr)) * 2);
#endif

				return o;
			}



			half4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);

			#ifdef FrameBlend_OFF
				half4 tex = tex2D(_MainTex, i.texcoord);
			#else
				//half4 tex = Tex2DInterpolated(_MainTex, i.texcoord, _Tiling);
				half4 tex1 = tex2D(_MainTex, i.texcoord.xy);
				half4 tex2 = tex2D(_MainTex, i.texcoord.zw);
				half4 tex = lerp(tex1, tex2, i.blend);
			#endif
				_TintColor.rgb = _TintColor.rgb * _TintColor.rgb * 10;
				half4 res = 2 * tex * _TintColor;

			#ifdef Clip_ON
				res.a = step(_Cutout, tex.a) * res.a;
			#endif

			#ifdef Clip_ON_Alpha
				res.a = step(1-i.color.a + _Cutout, tex.a);
				res.rgb *= i.color.rgb;
			#endif

			#if !defined(Clip_ON_Alpha)
				res *= i.color;
			#endif

				res.a = saturate(res.a);
				//res *= i.color;
			#ifdef FresnelFade_ON
				res.a *= i.fresnel;
			#endif

#if  SoftParticles_ON
				float2 screenUV = i.screenPos.xy / i.screenPos.w;
				float depth = SampleCameraDepth(screenUV);
				float sceneZ = LinearEyeDepth(depth, _ZBufferParams);
				float partZ = i.screenPos.w;
				float fade = saturate(_InvFade * (sceneZ - partZ));
				//i.color.a *= fade;
				res.rgba *= fade;
#endif
				res.rgb *= i.color.a;

			half3 fogColor;
			half3 fogOpacity;
			PositionInputs posInput = GetPositionInput(i.vertex.xy, _ScreenSize.zw, i.vertex.z, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
			EvaluateAtmosphericScattering(posInput, 1, fogColor, fogOpacity);

			
#ifdef BlendAdd
			res.rgb = res.rgb * (1.0 - fogOpacity);
#endif
#ifdef BlendAlpha
			//UNITY_APPLY_FOG(i.fogCoord, res);
			res.rgb = res.rgb * (1 - fogOpacity.x) + fogColor;
			res.a = res.a * (1 - fogOpacity.x);
#endif
#ifdef BlendMul
			res.rgb = lerp(res.rgb, half3(1, 1, 1), fogOpacity.x);
			res.rgb = lerp(half3(1, 1, 1), res.rgb, res.a);
			
#endif
#ifdef BlendMul2
			res.rgb = lerp(res.rgb, half3(0.5, 0.5, 0.5), fogOpacity.x);
			res.rgb = lerp(half3(0.5, 0.5, 0.5), res.rgb, res.a);
#endif
				return res;
			}
			ENDHLSL
		}
	}
}
 CustomEditor "RFX1_CustomMaterialInspectorParticle"
}
