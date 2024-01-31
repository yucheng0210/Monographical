// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Edo Factory/Cave"
{
	Properties
	{
		_WallAlbedo("Wall Albedo", 2D) = "white" {}
		_WallNormal("Wall Normal", 2D) = "bump" {}
		_WallMask("Wall Mask", 2D) = "white" {}
		_GroundAlbedo("Ground Albedo", 2D) = "white" {}
		_GroundNormal("Ground Normal", 2D) = "bump" {}
		_GroundMask("Ground Mask", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _WallNormal;
		uniform float4 _WallNormal_ST;
		uniform sampler2D _GroundNormal;
		uniform float4 _GroundNormal_ST;
		uniform sampler2D _WallAlbedo;
		uniform float4 _WallAlbedo_ST;
		uniform sampler2D _GroundAlbedo;
		uniform float4 _GroundAlbedo_ST;
		uniform sampler2D _WallMask;
		uniform float4 _WallMask_ST;
		uniform sampler2D _GroundMask;
		uniform float4 _GroundMask_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_WallNormal = i.uv_texcoord * _WallNormal_ST.xy + _WallNormal_ST.zw;
			float2 uv_GroundNormal = i.uv_texcoord * _GroundNormal_ST.xy + _GroundNormal_ST.zw;
			float3 lerpResult9 = lerp( UnpackNormal( tex2D( _WallNormal, uv_WallNormal ) ) , UnpackNormal( tex2D( _GroundNormal, uv_GroundNormal ) ) , i.vertexColor.r);
			o.Normal = lerpResult9;
			float2 uv_WallAlbedo = i.uv_texcoord * _WallAlbedo_ST.xy + _WallAlbedo_ST.zw;
			float2 uv_GroundAlbedo = i.uv_texcoord * _GroundAlbedo_ST.xy + _GroundAlbedo_ST.zw;
			float4 lerpResult4 = lerp( tex2D( _WallAlbedo, uv_WallAlbedo ) , tex2D( _GroundAlbedo, uv_GroundAlbedo ) , i.vertexColor.r);
			o.Albedo = lerpResult4.rgb;
			float2 uv_WallMask = i.uv_texcoord * _WallMask_ST.xy + _WallMask_ST.zw;
			float2 uv_GroundMask = i.uv_texcoord * _GroundMask_ST.xy + _GroundMask_ST.zw;
			float4 lerpResult12 = lerp( tex2D( _WallMask, uv_WallMask ) , tex2D( _GroundMask, uv_GroundMask ) , i.vertexColor.r);
			float4 break13 = lerpResult12;
			o.Metallic = break13;
			o.Smoothness = break13.a;
			o.Occlusion = break13.g;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
8;81;1208;910;2363.443;754.9313;3.144218;True;True
Node;AmplifyShaderEditor.SamplerNode;31;-1232.885,564.4954;Inherit;True;Property;_GroundMask;Ground Mask;5;0;Create;True;0;0;0;False;0;False;-1;fb987432770f82a49af2a3142b2c2ddb;fb987432770f82a49af2a3142b2c2ddb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;28;-1248.133,-13.74756;Inherit;True;Property;_WallMask;Wall Mask;2;0;Create;True;0;0;0;False;0;False;-1;5e31c64ea76eac84b98ac06796a758b9;5e31c64ea76eac84b98ac06796a758b9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;1;-851.0728,312.5277;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;12;-546.7184,406.4425;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;30;-1239.572,372.5;Inherit;True;Property;_GroundNormal;Ground Normal;4;0;Create;True;0;0;0;False;0;False;-1;026e3f35954422b40beb92c374dbf5fa;026e3f35954422b40beb92c374dbf5fa;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;29;-1239.731,181.3409;Inherit;True;Property;_GroundAlbedo;Ground Albedo;3;0;Create;True;0;0;0;False;0;False;-1;9dc4908c4bb2fb344966cad8428ad54d;9dc4908c4bb2fb344966cad8428ad54d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;27;-1247.419,-204.0592;Inherit;True;Property;_WallNormal;Wall Normal;1;0;Create;True;0;0;0;False;0;False;-1;058deb6d87c5435419de54b632d18911;058deb6d87c5435419de54b632d18911;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;26;-1257.89,-400.05;Inherit;True;Property;_WallAlbedo;Wall Albedo;0;0;Create;True;0;0;0;False;0;False;-1;0525493e17c7e7843929b5f56e135db4;0525493e17c7e7843929b5f56e135db4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;4;-534.2972,139.6403;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;9;-530.7377,263.3477;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;13;-262.4963,405.3227;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-13.56536,246.7955;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Edo Factory/Cave;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;28;0
WireConnection;12;1;31;0
WireConnection;12;2;1;1
WireConnection;4;0;26;0
WireConnection;4;1;29;0
WireConnection;4;2;1;1
WireConnection;9;0;27;0
WireConnection;9;1;30;0
WireConnection;9;2;1;1
WireConnection;13;0;12;0
WireConnection;0;0;4;0
WireConnection;0;1;9;0
WireConnection;0;3;13;0
WireConnection;0;4;13;3
WireConnection;0;5;13;1
ASEEND*/
//CHKSM=E3BB889FA62B5C3DD4924EC6BCCB94FE9691E8D9