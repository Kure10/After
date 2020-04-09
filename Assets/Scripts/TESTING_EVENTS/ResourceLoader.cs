using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ResourceLoader : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> eventResources = new List<Sprite>();
    [Space]
    [SerializeField] private Sprite defaultResource;


    // chtel bych aby list byl schovany. A resourses se nahravali do listu ze souboru.. Pomoci tlacitka Generate..

    // 1. vytvorit tlacitko
    // 2. udelat funkci na tlacitko ktera nahraje ze souboru a vyplní list.  Za prve vymaze cely list a pak nahraje znovu.

    private void Start()
    {


    }

    public Sprite FindEventResource (string resourceName)
    {
        Sprite tex = defaultResource;

        for (int i = 0; i < eventResources.Count; i++)
        {
            if(resourceName == eventResources[i].name)
            {
                tex = eventResources[i];
            } 
        }

        return tex;
    }

    public void ClearEventResources ()
    {
        eventResources.Clear();
    }

    public void AddEventResource(Sprite sprite)
    {
        eventResources.Add(sprite);
    }

    public Sprite SetDefault { set { defaultResource = value; } }
    //string txtName = "smile.png";  // your filename
    //Vector2 size = new Vector2(256, 256); // image size

    //private void Start()
    //{
    //    Texture2D texture = loadImage(size, Path.GetFullPath(txtName));
    //}


    //private static Texture2D loadImage(Vector2 size, string filePath)
    //{

    //    byte[] bytes = File.ReadAllBytes(filePath);
    //    Texture2D texture = new Texture2D((int)size.x, (int)size.y, TextureFormat.RGB24, false);
    //    texture.filterMode = FilterMode.Trilinear;
    //    texture.LoadImage(bytes);

    //    return texture;
    //}

}
