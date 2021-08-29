Shader "TriangleHitImageEffects/VignetteFX" {
	Properties{
		_MainTex("Source", 2D) = "white"  { }
		_ScreenWidth("ScreenWidth", Int) = 1280
		_ScreenHeight("ScreenHeight", Int) = 800
		_VignetteAmount("Grayscale Amount", Range(0,1)) = 1.0
	}
		SubShader{
			Pass
			{
				CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"

				uniform sampler2D _MainTex;
				uniform int _ScreenWidth;
				uniform int _ScreenHeight;
				fixed _VignetteAmount;

				float magnitude(float4 v)
				{
					return v.x * v.x + v.y * v.y + v.z * v.z;
				}

				fixed4 frag(v2f_img i) : COLOR {
					fixed4 center = fixed4(_ScreenWidth * 0.5, _ScreenHeight * 0.5, 0, 0);
					float magnitude1 = magnitude(center - float4(i.uv.x * _ScreenWidth, i.uv.y * _ScreenHeight, 0, 0));
					float _magnitude = magnitude(center);
					float _value = 1 - (magnitude1 / _magnitude);
					_value = sin(3.1426159 * 0.5f * _value);
					_value = lerp(1,  _value, _VignetteAmount);
					fixed4 vignette_color = fixed4(_value, _value, _value, 1.0);
					fixed4 finalColor = tex2D(_MainTex, i.uv) * vignette_color;
					return finalColor;
				}
				ENDCG
			}

		}
			FallBack "Diffuse"
}
