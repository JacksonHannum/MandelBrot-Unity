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

            float2 multiply(float2 f1 , float2 f2)
            {
                float2 f3;
                f3.x = (f1.x * f2.x) - (f1.y * f2.y);
                f3.y = (f1.x * f2.y) + (f1.y * f2.x);
                return f3;
            }

            float2 divide(float2 f1 , float2 f2)
            {
                float2 conj;

                conj.x = f2.x;
                conj.y = -f2.y;

                f1 = multiply(f1 , conj);
                f2 = multiply(f2 , conj);

                f1.x /= f2.x;
                f1.y /= f2.x;

                return f1;
            }

            float2 pow3(float2 f1)
            {
                float2 f2;
                f2 = multiply(f1 , f1);
                float2 f3 = multiply(f2 , f1);
                return f3;
            }

            float2 newton(float2 f1)
            {
                float2 f2;
                f2 = pow3(f1) - float2(1 , 0);
                return f2;
            }

            float2 derivative(float2 f1)
            {
                float2 squared = multiply(f1 , f1);
                float2 three = float2(3 , 0);
                float2 f2 = multiply(three , squared);
                return f2;
            }

            float2 nextguess(float2 f1)
            {
                float2 f2 = newton(f1);
                float2 d1 = derivative(f1);
                float2 f3 = divide(f2 , d1);
                return f1 - f3;
            }

            float distance(float2 f1 , float2 f2)
            {
                float real = f1.x - f2.x;
                float imag = f1.y - f2.y;
                return sqrt(real * real + imag * imag);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 z;

                z = rotate(((i.uv)) , 0 , _Rot) * _Area.zw + _Area.xy;
                z /= 100;

                float time;
                float2 nguess;

                // roots 
                float2 root1 = float2(1 , 0);
                float2 root2 = float2(-0.5 , sqrt(3) / 2);
                float2 root3 = float2(-0.5 , -sqrt(3) / 2);

                /* Newtown's fractal 
                 * Formula: z = z^3 - 1;
                 * Derivative: 3z^2
                 * Next guess: z = z - ((z^3 - 1) / (3z^2))
                 */

                float tolerance = 0.0001;

                while (time < _Cycles)
                {
                    nguess = nextguess(z);

                    if (distance(nguess , root1) < tolerance)
                    {
                        return float4(1 , 0 , 0 , 1);
                    }

                    if (distance(nguess , root2) < tolerance)
                    {
                        return float4(0 , 1 , 0 , 1);
                    }
                    if (distance(nguess , root3) < tolerance)
                    {
                        return float4(0 , 0 , 1 , 1);
                    }

                    z = nguess;
                    time += 1;
                }

                return float4(0 , 0 , 0 , 1);
            }
            ENDCG
        }
    }
}
