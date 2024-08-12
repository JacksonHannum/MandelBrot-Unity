Shader "Fractal/Fractal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Area ("Area" , vector) = (0,0,4,4)
        _Cycles ("Cycles" , float) = 200
        _ColorShift ("ColorShift" , float) = 0
        _Color ("Color" , range(0 , 1)) = 0.5
        _ColorCycle ("ColorCycle" , range(1 , 100)) = 20
        _Rot ("Rot" , range(-3.14159 , 3.14159)) = 0

    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 _Area;
            float _Cycles;
            float _ColorShift;
            float _ColorCycle;
            float _Rot;
            sampler2D _MainTex;

            float2 rotate(float2 pt , float2 pv , float ang)
            {
                float2 p = pt - pv;
                float s = sin(ang);
                float c = cos(ang);
                p = float2(p.x * c - p.y * s , p.x * s + p.y * c);
                p += pv;
                return p;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 z;
                float2 c;
                float2 zold;
                float2 escz;
                float2 linez;

                c = rotate(((i.uv)) , 0 , _Rot) * _Area.zw + _Area.xy;
                c /= 100;

                float n;
                float escaped;
                float dotline;
                float a;
                float angle;
                float time;

                while (time < _Cycles)
                {
                    zold = rotate (z , 0 , _ColorShift);
                    z = float2((z.x * z.x) - (z.y * z.y) , 2 * z.x * z.y) + c;
                  //  if ((z.x * z.x) + (z.y * z.y) > 400)
                 //   {
            //            break;
                //    }

                    if (dot(z , zold) > 400)
                    {
                        if (dotline == 0)
                        {
                            dotline = 1;
                            linez = z;
                        }
                    }
                    if (escaped == 0)
                    {
                        if (z.x * z.x + z.y * z.y > 400)
                        {
                            escaped = time;
                            escz = z;
                        }
                    }
                    if (dotline == 0)
                    {
                        n +=1;
                    }
                    time += 1;
                }
	    if (escaped == 0)
	    {
	    	return 0;
	    }

            
            if (dotline == 1)
            {
                a = log2(log(length(linez)) / log(20));

                angle = atan2(linez.x , linez.y);

                float4 col = sin((float4(0.3 , 0.45, 0.65 , 1) * n)) * 0.5 + 0.5;
                //col = tex2D(_MainTex , float2(n, _Color));

                col *= cos(angle * 2) * 0.2 + 1;
                col *= smoothstep(3 , 0 , a);

                float esca = log2(log(length(escz)) / log(20));
                col += smoothstep(0 , 3 , a) * (sin((float4(0.3 , 0.45, 0.65 , 1) * (escaped - esca + _ColorShift))) * 0.5 + 0.5);
                //col = tex2D(_MainTex , float2(n, _Color));
                if (a > 3)
                {
                    float esca = log2(log(length(escz)) / log(20));
                    float4 col = (sin((float4(0.3 , 0.45, 0.65 , 1) * (escaped - esca + _ColorShift))) * 0.5 + 0.5);
                    //col = tex2D(_MainTex , float2(n, _Color));   

                    return col;
                }
                else
                {
                    return col;
                }
            }
            else
            {
                float esca = log2(log(length(escz)) / log(20));
                float4 col = (sin((float4(0.3 , 0.45, 0.65 , 1) * (escaped - esca + _ColorShift))) * 0.5 + 0.5);
                //col = tex2D(_MainTex , float2(n, _Color));   

                return col;
            }

            
            
        
            /*
                float a = log2(log(length(escz)) / log(20));

                float4 col = sin((float4(0.3 , 0.45, 0.65 , 1) * (escaped - a + _ColorShift))) * 0.5 + 0.5;
                //col = tex2D(_MainTex , float2(n, _Color));

                

                return col;
            */
            
                
            }
            ENDCG
        }
    }
}
