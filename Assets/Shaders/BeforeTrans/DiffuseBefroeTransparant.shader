Shader "BeforeTransparant/Diffuse Texture Before Transparant" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _Cutoff ("Alpha cutoff", Range (0,1)) = 0.01
    }
    SubShader {
      Tags { 
      		"Queue" = "Transparent-100"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			}	
      CGPROGRAM
      #pragma surface surf Lambert alphatest:_Cutoff
      
      struct Input {
          float2 uv_MainTex;
      };
      sampler2D _MainTex;
      void surf (Input IN, inout SurfaceOutput o) 
      {
      	  
      	  half4 c = tex2D (_MainTex, IN.uv_MainTex);
          o.Albedo = c.rgb;
          o.Alpha = c.a;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }
