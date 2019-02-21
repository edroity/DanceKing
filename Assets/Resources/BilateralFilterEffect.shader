Shader "BilateralFilterEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
    }
    
    CGINCLUDE
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

    v2f vert (appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        return o;
    }

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;
    float4 _BlurRadius;
    float _BilaterFilterFactor;

    half CompareColor(fixed4 col1, fixed4 col2)
    {
        float l1 = LinearRgbToLuminance(col1.rgb);
        float l2 = LinearRgbToLuminance(col2.rgb);
        return smoothstep(_BilaterFilterFactor, 1.0, 1.0 - abs(l1 - l2));
    }
    
    fixed4 frag_frag_bilateralcolor (v2f i, float2 delta): SV_Target 
    {
        fixed4 col = tex2D(_MainTex, i.uv);
        fixed4 col0a = tex2D(_MainTex, i.uv - delta);
        fixed4 col0b = tex2D(_MainTex, i.uv + delta);
        fixed4 col1a = tex2D(_MainTex, i.uv - 2.0 * delta);
        fixed4 col1b = tex2D(_MainTex, i.uv + 2.0 * delta);
        fixed4 col2a = tex2D(_MainTex, i.uv - 3.0 * delta);
        fixed4 col2b = tex2D(_MainTex, i.uv + 3.0 * delta);
        
        half w = 0.37004405286;
        half w0a = CompareColor(col, col0a) * 0.31718061674;
        half w0b = CompareColor(col, col0b) * 0.31718061674;
        half w1a = CompareColor(col, col1a) * 0.19823788546;
        half w1b = CompareColor(col, col1b) * 0.19823788546;
        half w2a = CompareColor(col, col2a) * 0.11453744493;
        half w2b = CompareColor(col, col2b) * 0.11453744493;
        
        half3 result;
        result = w * col.rgb;
        result += w0a * col0a.rgb;
        result += w0b * col0b.rgb;
        result += w1a * col1a.rgb;
        result += w1b * col1b.rgb;
        result += w2a * col2a.rgb;
        result += w2b * col2b.rgb;
        
        result /= w + w0a + w0b + w1a + w1b + w2a + w2b;
     
        return fixed4(result, 1.0);
    }
    fixed4 frag_bilateralcolorH (v2f i) : SV_Target
    {
        float2 delta = _MainTex_TexelSize.xy * float2 (_BlurRadius.x, 0);
        return frag_frag_bilateralcolor (i, delta);
    }
    fixed4 frag_bilateralcolorV (v2f i) : SV_Target
    {
        float2 delta = _MainTex_TexelSize.xy * float2 (0, _BlurRadius.x);
        return frag_frag_bilateralcolor (i, delta);
    }
    ENDCG
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_bilateralcolorH

            
            ENDCG
        }
        //Pass
        //{
        //    CGPROGRAM
        //    #pragma vertex vert
        //    #pragma fragment frag_bilateralcolorV

            
        //    ENDCG
        //}
    }
}
