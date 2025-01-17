Shader "WangQAQ/Table/BottonmTag"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0, 0.03)) = 0.03
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurAmount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            float Remap(float value, float s1, float s2, float t1, float t2)
            {
                return t1 + (value - s1) * (t2 - t1) / (s2 - s1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                /* ���½�Բ�Ĵ�С */
                float radius = tex2D(_MainTex,float2(0.99,0.01)).r;
                float2 center = float2(0.5, 0.5); // Բ����(0,0)����ƽ�������
                float dist = length(i.uv - center);
                
                // �ü���Բ��
                if (dist > 0.5)
                    discard;

                // ������Բ�뾶����Ȧ��ɫ
                float edgeStart = Remap( radius , 0 , 1 , 0.1 , 0.5 );
                float edgeEnd = 0.5;

                if (dist <= edgeStart)
                {
                    // ����Բ����ʾ����ͼ
                    return tex2D(_MainTex, i.uv);
                }
                else if (dist <= edgeEnd)
                {
                    // ��Բ��ɫ�����ȣ���
                    float4 circleColor = tex2D(_MainTex,float2(0.01,0.01));

                    // ����Բ��Ե��Ӧ����ֵģ��Ч��
                    float t = (dist - edgeStart) / (edgeEnd - edgeStart);

                    fixed4 medianColor = tex2D(_MainTex, i.uv); 

                    fixed4 gradientColor = circleColor * t;
                    return lerp(medianColor, gradientColor, t);
                }

                return fixed4(0,0,0,0);
            }

            ENDCG
        }
    }
}