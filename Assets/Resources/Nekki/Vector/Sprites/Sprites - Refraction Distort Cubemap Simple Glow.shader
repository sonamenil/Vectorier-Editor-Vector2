// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Nekki/Vector/Sprites/Refraction Distort Cubemap Simple Glow" {
Properties {
 _Cube ("Reflection Cubemap", CUBE) = "_Skybox" { }
 _Color ("Tint", Color) = (1.000000,1.000000,1.000000,1.000000)
 _BumpAmt ("Distortion", Range(0.000000,1.000000)) = 1.000000
 _MKGlowColor ("Glow Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _MKGlowPower ("Glow Power", Range(0.000000,2.500000)) = 1.000000
}
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="MKGlow" }
 Pass {
  Name "BASE"
  Tags { "LIGHTMODE"="Always" "QUEUE"="Transparent" "RenderType"="MKGlow" }
  ZWrite Off
  Cull Off
  Blend One OneMinusSrcAlpha
  GpuProgramID 26143
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
#define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
#define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _Color;
uniform float _BumpAmt;
uniform samplerCUBE _Cube;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float4 xlv_COLOR :COLOR;
    float4 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_COLOR :COLOR;
    float4 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float4 tmpvar_1;
    float3x3 tmpvar_2;
    tmpvar_2[0] = conv_mxt4x4_0(UNITY_MATRIX_MVP).xyz;
    tmpvar_2[1] = conv_mxt4x4_1(UNITY_MATRIX_MVP).xyz;
    tmpvar_2[2] = conv_mxt4x4_2(UNITY_MATRIX_MVP).xyz;
    tmpvar_1 = (in_v.color * _Color);
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_COLOR = tmpvar_1;
    out_v.xlv_TEXCOORD0 = in_v.texcoord;
    out_v.xlv_TEXCOORD2 = mul(tmpvar_2, in_v.normal);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    float4 tmpvar_2;
    tmpvar_2.zw = in_f.xlv_TEXCOORD0.zw;
    float4 refr_3;
    float3 refracted_4;
    float4 c_5;
    c_5.w = in_f.xlv_COLOR.w;
    c_5.xyz = (in_f.xlv_COLOR.xyz * in_f.xlv_COLOR.w);
    float3 tmpvar_6;
    tmpvar_6 = normalize(in_f.xlv_TEXCOORD2);
    float3 tmpvar_7;
    tmpvar_7 = (tmpvar_6 * abs(tmpvar_6));
    refracted_4 = tmpvar_7;
    tmpvar_2.xy = ((refracted_4.xy * _BumpAmt) + in_f.xlv_TEXCOORD0.xy);
    float4 tmpvar_8;
    tmpvar_8 = texCUBE(_Cube, tmpvar_2.xyz);
    refr_3.xyz = tmpvar_8.xyz;
    refr_3.w = 1;
    tmpvar_1 = (c_5 * refr_3);
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
}
SubShader { 
 Pass {
  Name "BASE"
  Blend DstColor Zero
  GpuProgramID 84834
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _MainTex_ST;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float4 xlv_COLOR0 :COLOR0;
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float4 tmpvar_1;
    float4 tmpvar_2;
    tmpvar_2 = clamp(float4(0, 0, 0, 1.1), 0, 1);
    tmpvar_1 = tmpvar_2;
    float4 tmpvar_3;
    tmpvar_3.w = 1;
    tmpvar_3.xyz = in_v.vertex.xyz;
    out_v.xlv_COLOR0 = tmpvar_1;
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.vertex = UnityObjectToClipPos(tmpvar_3);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
}
}