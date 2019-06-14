Shader "Custom/OccluderShader"
{
    Properties
    {
    }
	SubShader{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry-1" }
		ColorMask 0
		ZWrite On

		Pass {}
	}
}
