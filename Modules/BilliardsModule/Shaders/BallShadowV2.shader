Shader "WangQAQ/BallShadowV2"
{
    Properties
    {
        [Header(Shadow Settings)]
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.5)
        _Floor ("Ground Offset", Float) = 0.01
        _ShadowFade ("Shadow Fade Distance", Float) = 2.0 // ��Ӱ��������
    }
    SubShader
    {
        Tags { 
            "LightMode" = "ForwardBase" 
            "Queue" = "AlphaTest" 
            "RenderType" = "AlphaTest"
        }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            fixed4 _ShadowColor;
            float _Floor;
            float _ShadowFade;

            struct appdata {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float heightFade : TEXCOORD1;
            };

            v2f vert(appdata v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);

                // ��ȡ��Ƥ�任�����������
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

                // ͶӰ������ (Y������)
                float3 shadowPos = float3(worldPos.x, _Floor, worldPos.z);
                o.pos = mul(UNITY_MATRIX_VP, float4(shadowPos, 1.0));
                o.worldPos = shadowPos;

                // �ؼ��޸ģ����߶�ʱ������Ӱ
                float height = worldPos.y - _Floor;
                o.heightFade = saturate(1.0 - height/_ShadowFade) * step(0, height);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // ���������ɫ
                fixed4 col = _ShadowColor;
                col.a *= i.heightFade;  // �߶�Ϊ��ʱalpha=0
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}