using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uButton : MonoBehaviour
{

    public void Anable()
    {
        this.gameObject.SetActive(true);
    }
    public void Disable()
    {
        this.gameObject.SetActive(false);
    }
}
