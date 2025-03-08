Shader "WangQAQ/BallShadowV2"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Floor("Surface Height (World Space)", Float) = 0.0
        _Scale("Ball Scale", Float) = 1.0
    }

    SubShader
    {
        Tags { 
            "Queue" = "Transparent+6" 
            "DisableBatching" = "true"
        }

        ZWrite Off
        Cull Off

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // 添加实例化编译指令
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                // 添加实例ID支持
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID // 可选，片段着色器不需要可移除
            };

            static const float BALL_RADIUS = 0.03f;

            sampler2D _MainTex;

            // 实例化属性缓冲区
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _Floor)
                UNITY_DEFINE_INSTANCED_PROP(float, _Scale)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v); // 初始化实例ID

                // 从实例化缓冲区获取属性
                float scale = UNITY_ACCESS_INSTANCED_PROP(Props, _Scale);
                float floor = UNITY_ACCESS_INSTANCED_PROP(Props, _Floor);

                float ballRadius = BALL_RADIUS * scale;
                float ballOriginY = floor + ballRadius;
                float3 shadowOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));

                float intensity = 1.0 - clamp(abs(shadowOrigin.y - ballOriginY), 0.0, ballRadius) / ballRadius;

                v2f o;
                o.vertex = UnityWorldToClipPos(float3(
                    v.vertex.x * scale + shadowOrigin.x,
                    floor,
                    v.vertex.z * scale + shadowOrigin.z
                ));
                o.uv = float3(v.uv, intensity);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv.xy) * float4(1.0, 1.0, 1.0, i.uv.z);
            }
            ENDCG
        }
    }
}