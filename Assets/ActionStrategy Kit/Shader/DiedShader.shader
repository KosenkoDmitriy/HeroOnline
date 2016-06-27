Shader "BlueSky/DiedShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color",Color) = (1,1,1,1)
		_Timer("_Timer",float)= 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		half4 _Color;
		float _Timer;
		
		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);	
			half3 endColor =half3(Luminance(c.rgb)) + _Color;
			half3 resultColor = lerp(c,endColor,_Timer);
			o.Albedo = resultColor;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
