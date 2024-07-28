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
    // Start is called before the first frame update
    void Start()
    {
        screenscale = .2f;

        screenpos = new Vector2(0 , 0);
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

    if (Input.GetKey("i"))
    {
        scint -= Time.deltaTime;
    }
    if (Input.GetKey("o"))
    {
        scint += Time.deltaTime;
    }

    if (Input.GetKey("q"))
    {
        rot += Time.deltaTime;
    }
    if (Input.GetKey("e"))
    {
        rot -= Time.deltaTime;
    }

    screenscale = Mathf.Pow(2.718281828f , scint);
    Vector2 move = new Vector2(Input.GetAxis("Horizontal") * Time.deltaTime , Input.GetAxis("Vertical") * Time.deltaTime) * screenscale * 10;

    move = rotate(move , new Vector2(0 , 0) , rot);

    screenpos += move;
    MandleBrot.SetVector("_Area" , new Vector4(screenpos.x , screenpos.y , screenscale , screenscale));
    MandleBrot.SetFloat("_ColorShift" , shift);
    MandleBrot.SetFloat("_ColorCycle" , cycle);
    MandleBrot.SetFloat("_Rot" , rot);

    shift += Time.deltaTime * 2;

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
