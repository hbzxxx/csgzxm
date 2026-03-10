Shader "Hidden/TieFu"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DecalTex("_DecalTex", 2D) = "white" {}
        _ParamA("ParamA",float)=1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _DecalTex;
            float _ParamA;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a=_ParamA;
				fixed4 decal = 0.0f;
				if (i.uv.y >= 0.25f && i.uv.y <= 0.75f)
				{
					decal = tex2D(_DecalTex, half2(i.uv.x, (i.uv.y - 0.25f) * 2.0f));
                    //decal*=(1,1,1,_ParamA);
				}
                fixed4 res=lerp(col, decal, decal.a);
                res.a=_ParamA;
				return res;
            }
            ENDCG
        }
    }
}
