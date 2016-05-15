Shader "Hidden/BlendModesOverlay" {
	Properties {
		_MainTex ("Screen Blended", 2D) = "" {}
		_Cutout ("Cutout Alpha Texture", 2D) = "" {}
		//_Overlay ("Color", 2D) = "grey" {}
		_Overlay ("Render Texture", 2D) = "grey" {}
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv[2] : TEXCOORD0;
	};
			
	sampler2D _Overlay;
	sampler2D _MainTex;
	sampler2D _Cutout;
	
	half _CutoutThreshold;
	half _BlendSize;
	half4 _MainTex_TexelSize;
	half4 _UV_Transform = half4(1, 0, 0, 1);
		
	v2f vert( appdata_img v ) { 
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		
		o.uv[0] = float2(
			dot(v.texcoord.xy, _UV_Transform.xy),
			dot(v.texcoord.xy, _UV_Transform.zw)
		);
		
		#if UNITY_UV_STARTS_AT_TOP
		if(_MainTex_TexelSize.y<0.0)
			o.uv[0].y = 1.0-o.uv[0].y;
		#endif
		
		o.uv[1] =  v.texcoord.xy;	
		return o;
	}

	half4 fragCutoutBlend (v2f i) : SV_Target {

		half4 cut = tex2D(_Cutout, i.uv[0]);
		half blendStart = max(0.0f, cut.a - _BlendSize);
		half blendEnd = min(1.0f, cut.a + _BlendSize);

		if(_CutoutThreshold <= blendStart)
		{
			return tex2D(_MainTex, i.uv[1]);
		}
		else if(_CutoutThreshold >= blendEnd)
		{
			return tex2D(_Overlay, i.uv[0]);
		}
		else
		{
			half4 mt = tex2D(_MainTex, i.uv[1]);
			half4 ot = tex2D(_Overlay, i.uv[0]);

			float t = (_CutoutThreshold - blendStart) / (2.0f * _BlendSize); 
			float tp = t;

			half4 main = mt;
			half3 d = mt.rgb - ot.rgb;
			if(dot(d, d) > 0.1)
			{
				//half p = max(0.0f, t - 0.2f);
				if(t <= 0.5)
				{
					main = lerp(mt, half4(1,1,1,cut.a), t * 2.0f);//mt + ot;
				}
				else
				{
					main = lerp(half4(1,1,1,cut.a), ot, (t - 0.5f) * 2.0f);
				}
			}


			return main;//lerp(main, ot, tp);		
		}
	}	


	ENDCG 
	
Subshader {
	  ZTest Always Cull Off ZWrite Off
      ColorMask RGB	  
  
 Pass {    

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragCutoutBlend
      ENDCG
  }   
}

Fallback off
	
} // shader
