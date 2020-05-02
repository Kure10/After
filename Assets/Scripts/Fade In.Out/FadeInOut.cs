using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    private SpriteRenderer renderer;

    [SerializeField]
    bool isFadingIn = true;

    [SerializeField]
    float fadeSpeed = 0.05f;


    [SerializeField]
    float fadeStrength = 0.05f;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        if (isFadingIn)
        {
            Color c = renderer.material.color;
            c.a = 0f;
            renderer.material.color = c;
        }

    }

    IEnumerator FadeIn()
    {
        for (float f = 0.05f; f < 1; f =+ fadeStrength)
        {
            Color c = renderer.material.color;
            c.a = f;
            renderer.material.color = c;
            yield return new WaitForSeconds(fadeSpeed);
        }
    }

    IEnumerator FadeOut()
    {
        for (float f = 1f; f >= -0.05f; f = -fadeStrength)
        {
            Color c = renderer.material.color;
            c.a = f;
            renderer.material.color = c;
            yield return new WaitForSeconds(fadeSpeed);
        }
    }

    public void StartFadeIn()
    {
        StartCoroutine("FadeIn");
    }

    public void StartFadeOut()
    {
        StartCoroutine("FadeOut");
    }


}
