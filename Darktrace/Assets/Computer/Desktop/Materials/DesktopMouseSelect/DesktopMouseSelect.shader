Shader "UI/BorderImageScreenSpace"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BorderColor ("Border Color", Color) = (0,0,0,1)
        _BorderThickness ("Border Thickness (px)", Range(0,10)) = 2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        Cull Off
        ZWrite Off
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _BorderColor;
            float _BorderThickness;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // Passa posição de tela em pixels
                o.screenPos = o.vertex.xy / o.vertex.w;
                o.screenPos = 0.5 * (o.screenPos + 1.0); // [-1,1] → [0,1]
                o.screenPos *= _ScreenParams.xy;        // agora em pixels
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;

                // Calcula distância até as bordas em pixels
                float left   = i.screenPos.x;
                float right  = _ScreenParams.x - i.screenPos.x;
                float bottom = i.screenPos.y;
                float top    = _ScreenParams.y - i.screenPos.y;

                float dist = min(min(left, right), min(top, bottom));

                // Borda em pixels
                float borderMask = smoothstep(_BorderThickness + 0.5, _BorderThickness, dist);

                return lerp(_BorderColor, texColor, borderMask);
            }
            ENDCG
        }
    }
}
