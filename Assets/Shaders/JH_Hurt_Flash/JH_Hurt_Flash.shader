Shader "JH/MonsterHurtFlash(With RimLight)" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MainColor("Main Color", Color) = (0.6, 0.6, 0.6, 1)
		_RimLightTex("Rim Tex", 2D) = "white" {}
		_RimLightColor("Rim Color", Color) = (0, 0, 0, 0)
		_RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
		_HurtStep("Hurt Step", Range(0, 1.0)) = 0
		_HurtColor("Hurt Color", Color) = (0, 0, 0, 0)
		_DisruptStepTex("DisruptStep Tex", 2D) = "black" {}   //防护闪贴图
		_DisruptStep("Disrupt Step", Range(0, 1.0)) = 0       //防护闪的步长值
		_DisruptColor("Disrupt Color", Color) = (0, 0, 0, 0)  //防护颜色
        _HordeStep("Horde Step", Range(0, 1.0)) = 0
        _HordeColor("Horede Color", Color) = (0, 0, 0, 0)
	}
	CGINCLUDE
	
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		fixed4 _MainColor;
		sampler2D _RimLightTex;
		
		fixed4 _RimLightColor;
		float _RimPower;
		float _HurtStep;
		fixed4 _HurtColor;
		
		sampler2D _DisruptStepTex;
		half _DisruptStep;
		half4 _DisruptColor;
        
        half _HordeStep;
        half4 _HordeColor;
		
		struct appdata 
		{
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
			float3 normal : NORMAL;
		};
		
		struct v2f 
		{
			float4 pos : POSITION;
			float4  uv : TEXCOORD0;
		}; 
		
		
		v2f vert (appdata v)
		{
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex );
			o.uv.xy = v.texcoord;//TRANSFORM_TEX(v.texcoord, _MainTex);
			
			float4 viewInLocal = mul(_World2Object, float4(_WorldSpaceCameraPos, 1));
			float3 viewDir = normalize(viewInLocal.xyz - v.vertex.xyz);
			o.uv.z = dot(viewDir.xyz, v.normal);
			o.uv.z = 1.0 - saturate(o.uv.z);
			o.uv.z = pow(o.uv.z, _RimPower);
			o.uv.w = 0.0f;
			return o;
		}

		fixed4 frag (v2f i) : COLOR
		{
			fixed4 c = tex2D(_MainTex, i.uv.xy);
			
			c.xyz *= _MainColor.xyz *2;
			fixed4 rim = tex2D(_RimLightTex, float2(_HurtStep, 0));
			half4 burst = tex2D(_DisruptStepTex, float2(_DisruptStep, 0));
            half4 horde = tex2D(_DisruptStepTex, float2(_HordeStep, 0));
			
			c.xyz += _RimLightColor.xyz * i.uv.z;
			c.xyz += _HurtColor.xyz*rim.x;
			c.xyz += _DisruptColor.xyz * burst.x;
            c.xyz += _HordeColor * horde.x;
			return c;
		}
		
		
		ENDCG
		
		SubShader
		{	
			Tags { "Queue"="Geometry" }
			Pass 
			{
				//Blend SrcAlpha One
				//Cull Back 
				Lighting Off
				Blend Off
				//Blend SrcAlpha One
				
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				ENDCG
			}
		}
}
