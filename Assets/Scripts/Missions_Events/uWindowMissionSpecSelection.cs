using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uWindowMissionSpecSelection : MonoBehaviour
{

    [SerializeField] GameObject specPrefab;

    [SerializeField] GameObject holder;

    public GameObject SpecPrefab { get { return this.specPrefab; } }

    public GameObject SpecHolder { get { return this.holder; } }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }



}
