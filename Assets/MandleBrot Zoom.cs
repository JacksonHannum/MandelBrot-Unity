using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MandleBrotZoom : MonoBehaviour
{
    public RawImage image;
    public Vector2 screenpos;
    public float screenscale;
    public Material MandleBrot;
    public Renderer ObjectRenderer;
    public float scint;
    public float shift;
    public float cycle = 20f;
    public float rot = 0f;

    private float pickoverlinear = -2.59026716545f;
    private float pickoverscale = 0.075f;

    public float juliaroot_x = 0.1f;
    public float juliaroot_y = 0.7f;

    public int isJulia = 0;
    public int isJuliaTarget = 0;
    public int keydowncount = 0;
    public GameObject ui;
    private bool uiactive = false;

    private bool uifadein = false;
    private bool uifadeout = false;

    private bool fractalfadein = false;
    private bool fractalfadeout = false;
    private float fractalchange = 0f;
    private float fractalfadetime = 0.3f;
    private float uichange = 0f;
    private bool uifinished = false;

    private int ispickover = 1;

    private bool first = true;

    private float initialtouchdistance = 0f;

    private int uipanel = 0;

    private float uitime = 0.3f;
    private float uifadetime = 0.3f;
    private float uiselectsize = 1.1f;

    public RectTransform mandelbrot;
    public RectTransform julia;

    private Vector3 mandelbrotsize;
    private Vector3 juliasize;

    private Vector3 mandelbrotstartsize;
    private Vector3 juliastartsize;

    public CanvasGroup canvasGroup;

    private float touchstart = 0f;
    private bool touchmoving = false;
    private bool touchfading = false;
    private float longtouchtime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        screenscale = .2f;

        screenpos = new Vector2(-25f, 0);
        MandleBrot.SetVector("_Area" , new Vector4(screenpos.x , screenpos.y , screenscale , screenscale));

        //mandelbrotsize = mandelbrot.sizeDelta;
        //juliasize = julia.sizeDelta;
        mandelbrotsize = mandelbrot.localScale;
        juliasize = julia.localScale;
    }

    public void UiGetStartSize()
    {
        mandelbrotstartsize = mandelbrot.localScale;
        juliastartsize = julia.localScale;
    }

    public void UiReset()
    {
        mandelbrot.localScale = mandelbrotsize;
        julia.localScale = juliasize;
        UiGetStartSize();
    }

    public void UiFinish()
    {
        if (isJuliaTarget == 0) {
            mandelbrot.localScale = mandelbrotsize * uiselectsize;
            julia.localScale = juliasize;
        } else {
            mandelbrot.localScale = mandelbrotsize;
            julia.localScale = juliasize * uiselectsize;
        }
    }

    public void FractalFadeUpdate()
    {
        if (fractalfadeout) {
            float t = (Time.time - fractalchange) / fractalfadetime;
            MandleBrot.SetFloat("_FractalFade" , 1 - t);
            if (Time.time - fractalchange > fractalfadetime) {
                fractalfadeout = false;
                MandleBrot.SetFloat("_FractalFade" , 0);
                isJulia = isJuliaTarget;
                fractalfadein = true;
                fractalchange = Time.time;
            }
        } else if (fractalfadein) {
            float t = (Time.time - fractalchange) / fractalfadetime;
            MandleBrot.SetFloat("_FractalFade" , t);
            if (Time.time - fractalchange > fractalfadetime) {
                fractalfadein = false;
                MandleBrot.SetFloat("_FractalFade" , 1);
            }
        }
    }

    public void UiUpdate()
    {
        if (uipanel == 0) {
            float t = (Time.time - uichange) / uitime;
            if (isJuliaTarget == 0) {
                mandelbrot.localScale = Vector3.Lerp(mandelbrotstartsize,
                            mandelbrotsize * uiselectsize, t);
                julia.localScale = Vector2.Lerp(juliastartsize, juliasize, t);
            } else {
                mandelbrot.localScale = Vector2.Lerp(mandelbrotstartsize, mandelbrotsize, t);
                julia.localScale = Vector2.Lerp(juliastartsize, juliasize * uiselectsize, t);
            }
            Debug.Log("t = " + t + " mandelbrot.localScale = " + mandelbrot.localScale + " julia.localScale = " + julia.localScale);
        }
    }

    public void MandleBrotToggled(bool value)
    {
        if (uifadein || uifadeout || fractalfadein || fractalfadeout) {
            return;
        }
        if (isJulia == 1)
        {
            isJuliaTarget = 0;
            //MandleBrot.SetInt("_IsJulia" , isJulia);
            uichange = Time.time;
            uifinished = false;
            UiGetStartSize();

            fractalchange = Time.time;
            fractalfadeout = true;
        }
    }

    public void JuliaToggled(bool value)
    {
        if (uifadein || uifadeout || fractalfadein || fractalfadeout) {
            return;
        }
        if (isJulia == 0)
        {
            isJuliaTarget = 1;
            //MandleBrot.SetInt("_IsJulia" , isJulia);
            uichange = Time.time;
            uifinished = false;
            UiGetStartSize();

            fractalchange = Time.time;
            fractalfadeout = true;
        }
    }

    public void CloseUi(bool value)
    {
        if (uifadein || uifadeout || fractalfadein || fractalfadeout) {
            return;
        }
        uifadeout = true;
        canvasGroup.alpha = 1;
        uiactive = false;
        uichange = Time.time;
        uifinished = false;
    }


    // Update is called once per frame
    void Update()
    {
        /*
           image.uvRect.xMax = screenpos.x + screenscale.x;
           image.uvRect.xMin = screenpos.x - screenscale.x;
           image.uvRect.yMax = screenpos.y + screenscale.y;
           image.uvRect.yMin = screenpos.y - screenscale.y;
           */

        if (uiactive || uifadein || uifadeout || fractalfadein || fractalfadeout) {
            if (fractalfadein || fractalfadeout) {
                FractalFadeUpdate();
            }
            if (uifadein) {
                float t = (Time.time - uichange) / uifadetime;
                canvasGroup.alpha = t;
                if (Time.time - uichange > uifadetime) {
                    uifadein = false;
                    uichange = Time.time;
                }
            } else if (uifadeout) {
                float t = (Time.time - uichange) / uifadetime;
                canvasGroup.alpha = 1 - t;
                if (Time.time - uichange > uifadetime) {
                    uifadeout = false;
                    uichange = Time.time;
                    ui.SetActive(false);
                }
            } else {
                if (Time.time - uichange > uitime && !uifinished) {
                    UiFinish();
                    uifinished = true;
                } else if (Time.time - uichange < uitime) {
                    UiUpdate();
                    //uiactive = false;
                    //ui.SetActive(false);
                }
            }
        }
        //if (uiactive && Time.time - uichange < 2) {
        //    UiUpdate();
            //uiactive = false;
            //ui.SetActive(false);
        //} 

        bool scint_changed = false;

        // Keyboard zoom 
        if (Input.GetKey("i"))
        {
            scint -= Time.deltaTime;
            scint_changed = true;
        }
        if (Input.GetKey("o"))
        {
            scint += Time.deltaTime;
            scint_changed = true;
        }

        // touch zoom
        if (Input.touchCount == 2) {

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            float currentdistance = Vector2.Distance(touch0.position, touch1.position);
            touchmoving = true;
            touchfading = false;

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began) {
                initialtouchdistance = Vector2.Distance(touch0.position, touch1.position);
            } else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved) {
                // TODO: not sure if this is correct
                float zoomfactor = currentdistance / initialtouchdistance;
                scint -= Time.deltaTime * Mathf.Log(zoomfactor);
                scint_changed = true;
            }
        }


        if (Input.GetKey("q"))
        {
            rot += Time.deltaTime;
        }
        if (Input.GetKey("e"))
        {
            rot -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            isJulia = 1 - isJulia;
            keydowncount++;
            //Debug.Log("Set isJulia to " + isJulia + " keydowncount = " + keydowncount);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (!uifadein && !uifadeout && !fractalfadein && !fractalfadeout) {
                uiactive = !uiactive;
                if (uiactive) {
                    uifadein = true;
                    canvasGroup.alpha = 0;
                    ui.SetActive(true);
                } else {
                    uifadeout = true;
                    canvasGroup.alpha = 1;
                }
               
                if (uiactive) {
                    UiReset();
                }
                //ui.SetActive(uiactive);
                uichange = Time.time;
                uifinished = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ispickover = 1 - ispickover;
            MandleBrot.SetInt("_IsPickover" , ispickover);
        }


        if (Input.GetKey(KeyCode.P)) {

            if (Input.GetKey(KeyCode.LeftShift)) {
                pickoverlinear -= Time.deltaTime;
            } else {
                pickoverlinear += Time.deltaTime;
            }

            pickoverscale = Mathf.Exp(pickoverlinear);

        } else if (first) {
            pickoverscale = Mathf.Exp(pickoverlinear);
        }


        //screenscale = Mathf.Pow(2.718281828f , scint);
        if (scint_changed || first) {
            screenscale = Mathf.Exp(scint);
            MandleBrot.SetInt("_IsPickover" , ispickover);
        }

        first = false;
        Vector2 move = new Vector2(Input.GetAxis("Horizontal") * Time.deltaTime , Input.GetAxis("Vertical") * Time.deltaTime) * screenscale * 10;

        if (!uiactive && !uifadein && !uifadeout) {
            if (Input.touchCount == 1) {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) {
                    touchstart = Time.time;
                    touchmoving = false;
                    touchfading = false;
                }
                if (!touchfading && touch.phase == TouchPhase.Moved) {
                    Vector2 delta = touch.deltaPosition;
                    move = -delta * Mathf.Exp(scint) * 0.0070f;
                    touchmoving = true;
                    touchfading = false;
                    //move = delta * screenscale;
                    //move = touch.deltaPosition * screenscale * 0.01f;
                }
                if (!touchfading && !touchmoving && touch.phase == TouchPhase.Stationary) {
                    if (Time.time - touchstart > longtouchtime) {
                        uifadein = true;
                        uichange = Time.time;
                        uifinished = false;
                        canvasGroup.alpha = 0;
                        ui.SetActive(true);
                        touchfading = true;
                    }
                }
            }
        }

        move = rotate(move , new Vector2(0 , 0) , rot);

        screenpos += move;
        //Debug.Log("Screenpos is " + screenpos + " move is " + move);
        MandleBrot.SetVector("_Area" , new Vector4(screenpos.x , screenpos.y , screenscale , screenscale));
        MandleBrot.SetFloat("_ColorShift" , shift);
        MandleBrot.SetFloat("_ColorCycle" , cycle);
        MandleBrot.SetFloat("_Rot" , rot);
        MandleBrot.SetFloat("_PickoverScale" , pickoverscale);
        MandleBrot.SetInt("_IsJulia" , isJulia);

        shift += Time.deltaTime * 1;
    }

    Vector2 rotate(Vector2 pt , Vector2 pv , float ang)
    {
        Vector2 p = pt - pv;
        float s = Mathf.Sin(ang);
        float c = Mathf.Cos(ang);
        p = new Vector2(p.x * c - p.y * s , p.x * s + p.y * c);
        p += pv;
        return p;
    }
}
