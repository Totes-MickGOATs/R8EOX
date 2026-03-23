Shader "R8EOX/AtmosphereBackground"
{
    Properties
    {
        _ColorDeep ("Color Deep", Color) = (0.039, 0.031, 0.059, 1)
        _ColorMid ("Color Mid", Color) = (0.059, 0.051, 0.102, 1)
        _ColorHigh ("Color High", Color) = (0.078, 0.071, 0.141, 1)
        _Speed ("Speed", Float) = 0.04
        _Scale ("Scale", Float) = 6.0
        _VignetteStrength ("Vignette Strength", Float) = 0.8
        _VignetteSoftness ("Vignette Softness", Float) = 0.6
        // Required for UI canvas stencil
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "AtmosphereBackground"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorDeep;
                float4 _ColorMid;
                float4 _ColorHigh;
                float  _Speed;
                float  _Scale;
                float  _VignetteStrength;
                float  _VignetteSoftness;
            CBUFFER_END

            // --- Noise helpers ---

            float2 Hash2(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)),
                           dot(p, float2(269.5, 183.3)));
                return frac(sin(p) * 43758.5453123);
            }

            float ValueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                // Quintic smoothstep
                float2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);

                float a = Hash2(i).x;
                float b = Hash2(i + float2(1.0, 0.0)).x;
                float c = Hash2(i + float2(0.0, 1.0)).x;
                float d = Hash2(i + float2(1.0, 1.0)).x;

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            // 3-octave FBM
            float Fbm(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float2 offset = float2(0.0, 0.0);
                UNITY_UNROLL
                for (int i = 0; i < 3; i++)
                {
                    value += amplitude * ValueNoise(p + offset);
                    p         *= 2.0;
                    amplitude *= 0.5;
                    offset    += float2(1.7, 9.2);
                }
                return value;
            }

            // --- Vertex ---

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = IN.uv;
                OUT.color       = IN.color;
                return OUT;
            }

            // --- Fragment ---

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float  t  = _Time.y * _Speed;

                // Two FBM layers with different UV offsets for parallax depth
                float2 scaledUV = uv * _Scale;
                float  layer0   = Fbm(scaledUV + float2(t, t * 0.7));
                float  layer1   = Fbm(scaledUV + float2(-t * 0.6, t * 0.4) + float2(4.3, 2.1));

                // 60/40 blend of the two layers
                float  noise = layer0 * 0.6 + layer1 * 0.4;

                // Map noise to three-stop color gradient
                float3 colorA = lerp(_ColorDeep.rgb, _ColorMid.rgb,  saturate(noise * 2.0));
                float3 colorB = lerp(_ColorMid.rgb,  _ColorHigh.rgb, saturate(noise * 2.0 - 1.0));
                float3 color  = noise < 0.5 ? colorA : colorB;

                // Radial vignette: darken toward outer edges
                float2 centeredUV  = uv * 2.0 - 1.0;
                float  dist        = length(centeredUV);
                float  vignette    = 1.0 - smoothstep(_VignetteSoftness,
                                                       _VignetteSoftness + (1.0 - _VignetteSoftness),
                                                       dist * _VignetteStrength);
                color *= vignette;

                // Respect vertex color alpha (Canvas tint / CanvasGroup fade)
                float alpha = _ColorDeep.a * IN.color.a;
                return half4(color, alpha);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
