Shader "Hidden/Shader/RadialBlur"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    float _Intensity;	// 强度
    float _SampleCount;	// 采样次数
    TEXTURE2D_X(_InputTexture);
    sampler2D _MainTex;
    sampler2D _GradTex;
    sampler2D _BlurTex;
    half4 _MainTex_TexelSize;
    float _BlurStrength;
    float2 _BlurCenter;
    float _Timer;
    float _BlurSpeed;
    float _BlurCircleRadius;
    float _BlurRange;
    float _Aspect;

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        half3 col = 0;
        // uv到中心点距离
        half2 symmetryUv = input.texcoord-0.5;
        //擴散距離方向(？)
        half distance = length(symmetryUv)+0.25;
        half factor = _Intensity / _SampleCount * distance;
        for(int i = 0; i < _SampleCount; i++)
        {
            half uvOffset = 1 - factor * i;
            half2 uv = symmetryUv * uvOffset + 0.5;
            uint2 positionSS = uv * _ScreenSize.xy;
            // 加权求和
            col += LOAD_TEXTURE2D_X(_InputTexture, positionSS).rgb;     
        }
        // 除于采样次数得到最终值
        col /= _SampleCount;
        
        return float4(col, 1);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "RadialBlur"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
            #pragma fragment CustomPostProcess
            #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}