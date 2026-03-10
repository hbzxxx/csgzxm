Shader "GUITest/Text Shader" {
    Properties{
        //这里的贴图为字体里面的FontTexture
        _MainTex("Font Texture", 2D) = "white" {}
        _Color("Text Color", Color) = (1,1,1,1)

        _Color1("Color1", Color) = (0,0,0,1)
       _LerpRate("LerpRate",Range(0,1))=1

        //Hue的值范围为0-359. 其他两个为0-1 ,这里我们设置到3，因为乘以3后 都不一定能到超过.
       _Hue("Hue", Range(0,359)) = 0
       _Saturation("Saturation", Range(0,3.0)) = 1.0
       _Value("Value", Range(0,3.0)) = 1.0

    }

        SubShader{

            Tags {
                //渲染队列-通常这个索引用来渲染透明度混合的物体
                "Queue" = "Transparent"
                //Projector为投影器，这样设置将会使该物体忽略任何投影类型的材质或贴图的影响
                "IgnoreProjector" = "True"
            //渲染透明物体时使用
            "RenderType" = "Transparent"
            //预览-平面
            "PreviewType" = "Plane"
        }
        
//MASK SUPPORT ADD
Stencil
{
    Ref [_Stencil]
    Comp [_StencilComp]
    Pass [_StencilOp] 
    ReadMask [_StencilReadMask]
    WriteMask [_StencilWriteMask]
}
ColorMask [_ColorMask]
//MASK SUPPORT END


            //关闭光照 剔除关闭(正背面全部显示) 深度测试开启 深度写入关闭
            //深度测试为当这个物体比深度缓冲中的像素靠近摄像机时显示，否则不显示
            Lighting Off Cull Off ZTest Always ZWrite Off
            //以这个物体的a值为标准，设置颜色缓冲区中的颜色为1-这个物体的a值
            Blend SrcAlpha OneMinusSrcAlpha
            //这里总体设置为不受光照影响，全部渲染。透明处理为打开深度测试，关闭深度写入自己定义颜色混合(以该物体a值为标准)

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                //大概就是vr ar，xr之类的东西了
                //https://docs.unity3d.com/Manual/SinglePassStereoRendering.html
                #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                        //UnityCG.cginc-UnityInstancing.cginc
                        //- UNITY_VERTEX_INPUT_INSTANCE_ID Declare instance ID field in vertex shader input / output struct.在a2v或者v2f里面声明id领域
        //                #ifdef SHADER_API_PSSL
        //                  #define DEFAULT_UNITY_VERTEX_INPUT_INSTANCE_ID uint instanceID;
        //              #else
        //                  #define DEFAULT_UNITY_VERTEX_INPUT_INSTANCE_ID uint instanceID : SV_InstanceID;
        //              #endif
                    };

                    struct v2f {
                        float4 vertex : SV_POSITION;
                        fixed4 color : COLOR;
                        float2 texcoord : TEXCOORD0;
                        UNITY_VERTEX_OUTPUT_STEREO
                            //UnityCG.cginc-UnityInstancing.cginc
                            //- UNITY_VERTEX_OUTPUT_STEREO Declare stereo target eye field in vertex shader output struct.
            //              #ifdef UNITY_STEREO_INSTANCING_ENABLED
            //                  #define DEFAULT_UNITY_VERTEX_OUTPUT_STEREO                          uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
            //              #elif defined(UNITY_STEREO_MULTIVIEW_ENABLED)
            //                  #define DEFAULT_UNITY_VERTEX_OUTPUT_STEREO float stereoTargetEyeIndex : BLENDWEIGHT0;
            //              #endif
                        };

                        sampler2D _MainTex;
                        uniform float4 _MainTex_ST;
                        uniform fixed4 _Color;
                        fixed _LerpRate;
                        fixed4 _Color1;
                        fixed4 _Color2;

                        half _Hue;
                        half _Saturation;
                        half _Value;


                        //RGB to HSV
                        float3 RGBConvertToHSV(float3 rgb)
                        {
                            float R = rgb.x, G = rgb.y, B = rgb.z;
                            float3 hsv;
                            float max1 = max(R, max(G, B));
                            float min1 = min(R, min(G, B));
                            if (R == max1)
                            {
                                hsv.x = (G - B) / (max1 - min1);
                            }
                            if (G == max1)
                            {
                                hsv.x = 2 + (B - R) / (max1 - min1);
                            }
                            if (B == max1)
                            {
                                hsv.x = 4 + (R - G) / (max1 - min1);
                            }
                            hsv.x = hsv.x * 60.0;
                            if (hsv.x < 0)
                                hsv.x = hsv.x + 360;
                            hsv.z = max1;
                            hsv.y = (max1 - min1) / max1;
                            return hsv;
                        }

                        //HSV to RGB
                        float3 HSVConvertToRGB(float3 hsv)
                        {
                            float R, G, B;
                            //float3 rgb;
                            if (hsv.y == 0)
                            {
                                R = G = B = hsv.z;
                            }
                            else
                            {
                                hsv.x = hsv.x / 60.0;
                                int i = (int)hsv.x;
                                float f = hsv.x - (float)i;
                                float a = hsv.z * (1 - hsv.y);
                                float b = hsv.z * (1 - hsv.y * f);
                                float c = hsv.z * (1 - hsv.y * (1 - f));
                                switch (i)
                                {
                                case 0: R = hsv.z; G = c; B = a;
                                    break;
                                case 1: R = b; G = hsv.z; B = a;
                                    break;
                                case 2: R = a; G = hsv.z; B = c;
                                    break;
                                case 3: R = a; G = b; B = hsv.z;
                                    break;
                                case 4: R = c; G = a; B = hsv.z;
                                    break;
                                default: R = hsv.z; G = a; B = b;
                                    break;
                                }
                            }
                            return float3(R, G, B);
                        }

                       
                        v2f vert(appdata_t v)
                        {
                            v2f o;
                            UNITY_SETUP_INSTANCE_ID(v);
                            // - UNITY_SETUP_INSTANCE_ID        Should be used at the very beginning of the vertex shader / fragment shader,
            //                #define DEFAULT_UNITY_SETUP_INSTANCE_ID(input)      { UnitySetupInstanceID(UNITY_GET_INSTANCE_ID(input)); 
            //                  void UnitySetupInstanceID(uint inputInstanceID)
            //                  {
            //                      #ifdef UNITY_STEREO_INSTANCING_ENABLED
            //                          // stereo eye index is automatically figured out from the instance ID
            //                          unity_StereoEyeIndex = inputInstanceID & 0x01;
            //                          unity_InstanceID = unity_BaseInstanceID + (inputInstanceID >> 1);
            //                      #else
            //                          unity_InstanceID = inputInstanceID + unity_BaseInstanceID;
            //                      #endif
            //                  }
                            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                            //o.stereoTargetEyeIndex = unity_StereoEyeIndex

                            //顶点坐标从模型空间(M)转换到观察空间(v,摄像机空间) 等同mul(Unity_MARTIX_MVP, v.vertex)
                            o.vertex = UnityObjectToClipPos(v.vertex);
                            //因为3dText的网格顶点是由TextMesh生成，TextMesh可以调整顶点颜色，这样可以使用TextMesh调整每个3d字体的特殊颜色，_Color确定总体颜色
                            
                            o.color = v.color * _Color;
                            //UnityCG.cginc #define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)
                            //主要计算_MainTex的tilling和offset但是感觉这个shader里面没有卵用。真的有人调整这个吗
                            //感觉可以直接 o.texcoord = v.texcoord
                            o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                            return o;
                        }


                        fixed4 frag(v2f i) : SV_Target
                        {

                            fixed4 color = tex2D(_MainTex,i.texcoord)*i.color;
                        fixed theA = color.a;
                  //          if (i.texcoord.y <= _LerpRate
                  //              &&color.r<=.65
                  //              && color.g <= .65
                  //              && color.b <= .65) {

                  ///*              color.r = color.r*0.1 + _Color1.r * i.texcoord.y / _LerpRate;
                  //              color.g = color.g * 0.1 + _Color1.g * i.texcoord.y / _LerpRate;
                  //              color.b = color.b * 0.1 + _Color1.b * i.texcoord.y / _LerpRate;*/

                  //           /*   color.r = color.r  * _Color1.r * i.texcoord.y / _LerpRate;
                  //              color.g = color.g  * _Color1.g * i.texcoord.y / _LerpRate;
                  //              color.b = color.b  * _Color1.b * i.texcoord.y / _LerpRate;*/

                  //              //color.r = color.r * _Color1.r;//* i.texcoord.y / _LerpRate;
                  //              //color.g = color.g * _Color1.g; //* i.texcoord.y / _LerpRate;
                  //              //color.b = color.b * _Color1.b; //* i.texcoord.y / _LerpRate;

                  //             color.r = _Color1.r;//* i.texcoord.y / _LerpRate;
                  //              color.g =  _Color1.g; //* i.texcoord.y / _LerpRate;
                  //              color.b =  _Color1.b; //* i.texcoord.y / _LerpRate;

                  //          }
                  //          return color;


                            float3 colorHSV;
                            colorHSV.xyz = RGBConvertToHSV(color.xyz);   //转换为HSV
                            colorHSV.x += _Hue; //调整偏移Hue值
                            colorHSV.x = colorHSV.x % 360;    //超过360的值从0开始

                            colorHSV.y *= _Saturation;  //调整饱和度
                            colorHSV.z *= _Value;

                    

                            color.xyz = HSVConvertToRGB(colorHSV.xyz);   //将调整后的HSV，转换为RGB颜色
                            color.a = theA;
                            return color;

              


                        /*    fixed4 col = i.color;
                            col.a *= tex2D(_MainTex, i.texcoord).a;
                            return col;*/
                        }
                        ENDCG
            }
        }
}
