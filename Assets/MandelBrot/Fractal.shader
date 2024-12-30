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
        _IsJulia ("IsJulia", int) = 0
        _PickoverScale ("PickoverScale" , range(0.0 , 100.0)) = 3.0
        _JuliaRoot ("JuliaRoot" , vector) = (0.1 , 0.7 , 0 , 0)
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
            int _IsJulia;
            float2 _JuliaRoot;
            float _PickoverScale;
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
                float2 rzold;
                float2 zold;
                float2 escz;
                float2 rlinez;
                float2 linez;

                // This is MandelBrot (!Julia)
                if (_IsJulia == 0) {
                    c = rotate((i.uv) , 0 , _Rot) * _Area.zw + _Area.xy;
                    c /= 100;
                    z = 0;

                } else {

                // Julia
                    z = rotate(((i.uv)) , 0 , _Rot) * _Area.zw + _Area.xy;
                    z /= 100;
                    c = float2(0.1 , 0.7);
                }

                float n;
                float escaped;
                float rdotline;
                float dotline;
                float a;
                float2 rz;
                float angle;
                float time;
                float2 zval;
                float2 zder;
                float mindist = 1000000;

                while (time < _Cycles)
                {


                    zold = z;
                    /*

                    zval = float2(1 , 0);
                    a = 0;
                    while (a < 5)
                    {
                        zval = float2(zold.x * zval.x - zold.y * zval.y , zold.x * zval.y + zold.y * zval.x);
                        a ++;
                    }

                    zval -= float2(1 , 0);

                    zder = float2(1 , 0);

                    a = 0;
                    while (a < 4)
                    {
                        zder = float2(zold.x * zder.x - zold.y * zder.y , zold.x * zder.y + zold.y * zder.x);
                        a ++;
                    }
                    zder *= 5;
                //    zder += float2(1 , 0);
                    z -= float2((zval.x * zder.x + zval.y * zder.y) / (zder.x * zder.x + zder.y * zder.y) , (zval.y * zder.x - zval.x * zder.y) / (zder.x * zder.x + zder.y * zder.y));
*/




                    

                    rzold = rotate (z , 0 , 2 * _ColorShift);
                    z = float2((z.x * z.x) - (z.y * z.y) , 2 * z.x * z.y) + c;
                    rz = rotate (z , 0 , 0.1 * _ColorShift + n * (3.14159 / 2));

                    if (dot(z , rzold) > 400)
                    {
                        if (rdotline == 0)
                        {
                            rdotline = 1;
                            rlinez = z;
                        }
                    }
                    if (dot(z , rzold) > 400)
                    {
                        if (dotline == 0)
                        {    
                            linez = z;  
                            dotline = 1; 
                        }
                    }
                    if (escaped == 0)
                    {
                        if (z.x * z.x + z.y * z.y > 400)
                        {
                            escaped = time;
                            escz = z;
                        }
                        if (abs(rz.x) < abs(rz.y))
                        {
                            if (abs(rz.x) < mindist)
                            {
                                mindist = abs(rz.x);
                            }
                        }
                        else
                        {
                            if (abs(rz.y) < mindist)
                            {
                                mindist = abs(rz.y);
                            }
                        }
                    }
                    if (rdotline == 0)
                    {
                        n += 1;
                    }
                    
                    time += 1;
                }

                // change the -2 to other values to make more colorful
                mindist = mindist * 50;
                float scale = 0 - _PickoverScale;

                float4 stcol = float4(scale * mindist , scale * mindist , scale * mindist , 1);
                //float4 stcol = float4(-0 * mindist , 0 , -3 * mindist , 1);

                // Returning here means 'pickover-stalk', not returning here means combined mandelBrot and Stalk
                //For Pure MandelBrot, Make stcol always 0 after line 175

                return -stcol;
                //return (0.8 + stcol);
                //float esca = log2(log(length(escz)) / log(20));
                //float4 col = (sin((float4(0.3 , 0.45, 0.65 , 1) * 2 * ((escaped - esca + _ColorShift * 5)))) * 0.5 + 0.5);
                //col = tex2D(_MainTex , float2(n, _Color));   

          //      return col;


                //float col = z.x + 2 * z.y;
                //return (sin(float4(0.3 , 0.45, 0.65 , 1) * (abs(col) * 4 + 15)));
                


            angle = atan2(escz.x , escz.y);
            a = 2 * log2(log(length(escz)) / log(20));

        
            float b = abs(abs((angle)) - 3.14159 / 2) / (a + 1.61803);
            b = (sin(sin(b * 3.14159)) - 0.5) * 0.5 + 0.5;

            
            

            
            if (rdotline == 1)
            {
                angle = atan2(rlinez.x , rlinez.y);
                a = log2(log(length(rlinez)) / log(20));



                float4 col = (sin((float4(0.3 , 0.45, 0.65 , 1) * n * 0.5))) * 0.5 + 0.5;
                //col = tex2D(_MainTex , float2(n, _Color));

                col *= cos(angle * 2) * 0.2 + 1;
                col *= smoothstep(3 , 0 , a);
                

                float esca = log2(log(length(escz)) / log(20));
                col += smoothstep(0 , 3 , a) * (sin((float4(0.3 , 0.45, 0.65 , 1) * 0.5 * (escaped - esca))) * 0.5 + 0.5);
                //col = tex2D(_MainTex , float2(n, _Color));

                
                if (a > 3)
                {
                    float esca = log2(log(length(escz)) / log(20));
                    float4 col = (sin((float4(0.3 , 0.45, 0.65 , 1) * 0.5 * (escaped - esca))) * 0.5 + 0.5);
                    //col = tex2D(_MainTex , float2(n, _Color));   

                    return (stcol + col);
                }
                else
                {
                    return (stcol + col);
                }
                
            }
            else
            {
                float esca = log2(log(length(escz)) / log(20));
                float4 col = (sin((float4(0.3 , 0.45, 0.65 , 1) * 0.5 * (escaped - esca))) * 0.5 + 0.5);
                //col = tex2D(_MainTex , float2(n, _Color));   

                return (stcol + col);
            }

            
            
        
            /*
                float a = log2(log(length(escz)) / log(20));

                float4 col = sin((float4(0.3 , 0.45, 0.65 , 1) * (escaped - a))) * 0.5 + 0.5;
                //col = tex2D(_MainTex , float2(n, _Color));

                

                return col;
            */
            
                
            }
            ENDCG
        }
    }
}
