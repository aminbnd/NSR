﻿Shader "Silver Tau/PixelNES"
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

			static const float EPSILON = 1e-10;

            fixed4 frag (v2f_img i) : SV_Target
            {
				fixed4 tex = tex2D(_MainTex, i.uv);

				int r = (tex.r - EPSILON) * 4;
				int g = (tex.g - EPSILON) * 4;
				int b = (tex.b - EPSILON) * 4;

				return float4(r / 3.0, g / 3.0, b / 3.0, 1.0);
            }
            ENDCG
        }
    }
}
