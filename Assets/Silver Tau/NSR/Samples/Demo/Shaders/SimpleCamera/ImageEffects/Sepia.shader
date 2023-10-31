﻿Shader "Silver Tau/Sepia"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;

            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

				half3x3 sepiaMatrix = half3x3
				(
					0.393, 0.349, 0.272,
					0.769, 0.686, 0.534,
					0.189, 0.168, 0.131
				);

				half3 sepia = mul(tex.rgb, sepiaMatrix);

				return half4(sepia, tex.a);
            }
            ENDCG
        }
    }
}
