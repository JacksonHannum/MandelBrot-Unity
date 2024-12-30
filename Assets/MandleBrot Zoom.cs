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
    public int keydowncount = 0;

    private bool first = true;

    private float initialtouchdistance = 0f;



    // Start is called before the first frame update
    void Start()
    {
        screenscale = .2f;

        screenpos = new Vector2(-25f, 0);
        MandleBrot.SetVector("_Area" , new Vector4(screenpos.x , screenpos.y , screenscale , screenscale));
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

    // Debug.Log("pickoverlinear = " + pickoverlinear + " pickoverscale = " + pickoverscale);

    //screenscale = Mathf.Pow(2.718281828f , scint);
    if (scint_changed || first) {
        screenscale = Mathf.Exp(scint);
    }

    first = false;
    Vector2 move = new Vector2(Input.GetAxis("Horizontal") * Time.deltaTime , Input.GetAxis("Vertical") * Time.deltaTime) * screenscale * 10;

    if (Input.touchCount == 1) {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Moved) {
            Vector2 delta = touch.deltaPosition;
            move = -delta * Mathf.Exp(scint) * 0.0070f;
            //move = delta * screenscale;
            //move = touch.deltaPosition * screenscale * 0.01f;
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
