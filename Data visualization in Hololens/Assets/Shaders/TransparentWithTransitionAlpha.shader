﻿Shader "Unlit/TransparentWithTransitionAlpha"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TransitionAlpha ("Transition Alpha", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent+100" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

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

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float _TransitionAlpha;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv) * _TransitionAlpha;
			}
			ENDCG
		}
	}
}
