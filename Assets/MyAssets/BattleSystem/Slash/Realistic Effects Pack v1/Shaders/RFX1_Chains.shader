Shader "KriptoFX/RFX1/Chains" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		[HDR]_TintColor("Tint Color", Color) = (0,0,0,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		
		_BumpMap("Normal (RG)", 2D) = "bump" {}
		_ReflTex("Cubemap", CUBE) = "" {}

	}




		SubShader{

		Cull Back
		ZWrite On
		Pass{

		HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_instancing

#pragma target 4.6

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/AtmosphericScattering/AtmosphericScattering.hlsl"


		sampler2D RFX1_PointLightAttenuation;
	half4 RFX1_AmbientColor;
	float4 RFX1_LightPositions[40];
	float4 RFX1_LightColors[40];
	int RFX1_LightCount;


	sampler2D _MainTex;
	float4 _MainTex_ST;
	half4 _Color;
	half4 _TintColor;
	samplerCUBE  RFX1_Reflection;
	float _Cutoff;
	float _Scale;
	sampler2D _BumpMap;
	samplerCUBE _ReflTex;

	struct appdata_t {
		float4 vertex : POSITION;
		float4 color : COLOR0;
		half3 normal : NORMAL;
		float2 texcoord : TEXCOORD0;

		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		half3 color : COLOR0;
		float2 texcoord : TEXCOORD0;
	
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};



	v2f vert(appdata_t v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.vertex = TransformObjectToHClip(v.vertex.xyz);
		o.color = v.color;
		
		o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

		return o;
	}


	half4 frag(v2f i) : SV_Target
	{ 
		UNITY_SETUP_INSTANCE_ID(i);
		half4 tex = tex2D(_MainTex, i.texcoord);
		if (tex.a < 0.2) discard;

		half4 finalCol = 1;
		
		half3 normal = UnpackNormal(tex2D(_BumpMap, i.texcoord));
		
		finalCol.rgb = saturate(tex * dot(texCUBE(_ReflTex, normal.xyy).rgb, 0.33) * _Color.rgb);
		finalCol.rgb = finalCol.rgb * 0.75 + finalCol.rgb * finalCol.rgb;
		finalCol.rgb += _TintColor.rgb * i.color.rgb *  tex.rgb;
		
		//finalCol.rgb = saturate(envSample.rgb * _MainColor.rgb * 1.5 + fresnel * i.lightColor *  _MainColor.rgb * 0.25);
		//finalCol.rgb = envSample.rgb * _MainColor.rgb
		//o.Emission = _TintColor * c * IN.color;
		half3 fogColor;
		half3 fogOpacity;
		PositionInputs posInput = GetPositionInput(i.vertex.xy, _ScreenSize.zw, i.vertex.z, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
		EvaluateAtmosphericScattering(posInput, 1, fogColor, fogOpacity);
		finalCol.rgb = finalCol.rgb * (1 - fogOpacity) + fogColor;
		finalCol.a = finalCol.a * (1 - fogOpacity.x);

		return finalCol;
	}
		ENDHLSL
	}

	
	}

}
