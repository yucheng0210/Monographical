Shader "KriptoFX/RFX1/BlendCutout" {
	Properties{
		[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_CutoutMap("Cuout Tex (r)", 2D) = "black" {}
		_Cutout("Cutout", Range(0, 1)) = 1
		_InvFade("Soft Particles Factor", Float) = 3

	}

		Category{
				Tags{ "Queue" = "Transparent-150" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
				Blend SrcAlpha OneMinusSrcAlpha
			
			Cull Off
			ZWrite Off

			SubShader{
			Pass{

			HLSLPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma multi_compile_instancing

	#pragma multi_compile_particles
	float4x4 _InverseTransformMatrix;

#pragma target 4.6

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/AtmosphericScattering/AtmosphericScattering.hlsl"

		sampler2D _MainTex;
		sampler2D _CutoutMap;
		half4 _TintColor;
		half _Cutout;

		struct appdata_t {
			float4 vertex : POSITION;
			
			float2 texcoord : TEXCOORD0;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			
			float4 texcoord : TEXCOORD0;

			UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
		};

		float4 _MainTex_ST;
		float4 _CutoutMap_ST;

		v2f vert(appdata_t v)
		{
			v2f o;
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_TRANSFER_INSTANCE_ID(v, o);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

			o.vertex = TransformObjectToHClip(v.vertex.xyz);

			o.texcoord.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
			float2 pos = mul(_InverseTransformMatrix, float4(v.vertex.xyz, 1)).xz;
			o.texcoord.zw = (pos - 0.5) * _CutoutMap_ST.xy + _CutoutMap_ST.zw;
			
			return o;
		}

		float _InvFade;

		half4 frag(v2f i) : SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(i);


			half4 col = 2.0f * _TintColor * tex2D(_MainTex, i.texcoord.xy);
			half cutoutAlpha = tex2D(_CutoutMap, i.texcoord.zw).r;
			half alpha = pow(saturate(1 - cutoutAlpha + _Cutout), 50);
			col.a *= saturate(alpha * pow(saturate(_Cutout), .2));
			
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
