using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class ButtonController : MonoBehaviour
{

    private Image image;
    public Color idleColor;
    public Color hoverColor;
    public Color clickColor;

    public void Awake()
    {
        image = GetComponent<Image>();
    }



    public void SwitchHover(bool flag)
    {
        image.color = flag ? hoverColor : idleColor;
    }


    public void OnClick()
    {
        StartCoroutine("OneClick");
    }


    private IEnumerator OneClick()
    {
        image.color = clickColor;
        yield return new WaitForSeconds(0.1f);
        image.color = idleColor;
        yield return null;
    }


}
