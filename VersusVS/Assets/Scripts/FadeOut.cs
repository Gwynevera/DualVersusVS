using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public float timeToFade = 10;

    float fadeRate;

    int totalAlpha = 1; //255

    SpriteRenderer s;

    // Start is called before the first frame update
    void Start()
    {
        s = GetComponent<SpriteRenderer>();
        fadeRate = Time.fixedDeltaTime * (totalAlpha / timeToFade);
    }

    void CalculateFadeRate()
    {
        //s = GetComponent<SpriteRenderer>();
        //fadeRate = Time.fixedDeltaTime * (totalAlpha / timeToFade);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (s != null && s.color.a > 0)
        {
            float a = s.color.a - fadeRate;
            s.color = new Color(s.color.r, s.color.g, s.color.b, a);
        }
    }
}
