﻿Shader "Silver Tau/Base"
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
            float4 _MainTex_ST;

            fixed4 frag (v2f_img i) : SV_Target
            {
				fixed4 tex = tex2D(_MainTex, i.uv);
				return tex;
            }
            ENDCG
        }
    }
}
