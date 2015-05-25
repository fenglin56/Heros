Shader "Effect/Alpha Blended_Queue-1" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent-1" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	

	SubShader {
		Pass {
			SetTexture [_MainTex] {
				constantColor [_TintColor]
				combine constant * primary
			}
			SetTexture [_MainTex] {
				combine texture * previous DOUBLE
			}

		}
	}
		SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
}
