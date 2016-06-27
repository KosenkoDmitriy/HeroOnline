// Shader created with Shader Forge Beta 0.34 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.34;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-2-RGB,custl-2-RGB;n:type:ShaderForge.SFN_Tex2d,id:2,x:33039,y:32693,ptlb:mainText,ptin:_mainText,tex:d0d5bf46b4e75044cbb02cfe7ff1d8cb,ntxv:0,isnm:False|UVIN-15-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4,x:33829,y:32851,ptlb:mulTime,ptin:_mulTime,glob:False,v1:1;n:type:ShaderForge.SFN_Time,id:6,x:33829,y:32675;n:type:ShaderForge.SFN_TexCoord,id:7,x:33630,y:32549,uv:0;n:type:ShaderForge.SFN_Append,id:14,x:33444,y:32882|A-20-OUT,B-21-OUT;n:type:ShaderForge.SFN_Add,id:15,x:33251,y:32673|A-7-UVOUT,B-14-OUT;n:type:ShaderForge.SFN_Multiply,id:20,x:33648,y:32764|A-6-T,B-4-OUT;n:type:ShaderForge.SFN_Vector1,id:21,x:33673,y:32979,v1:0;proporder:2-4;pass:END;sub:END;*/

Shader "Shader Forge/UnlitPanUV" {
    Properties {
        _mainText ("mainText", 2D) = "white" {}
        _mulTime ("mulTime", Float ) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _mainText; uniform float4 _mainText_ST;
            uniform float _mulTime;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
                float2 node_7 = i.uv0;
                float4 node_6 = _Time + _TimeEditor;
                float2 node_15 = (node_7.rg+float2((node_6.g*_mulTime),0.0));
                float4 node_2 = tex2D(_mainText,TRANSFORM_TEX(node_15, _mainText));
                float3 finalColor = node_2.rgb;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
