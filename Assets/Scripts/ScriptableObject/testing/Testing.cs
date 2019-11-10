using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Testing", fileName = "Testing")]
public class Testing : ScriptableObject
{

    [SerializeField] private GameObject prefab;



    public GameObject Prefab { get { return prefab; } set { prefab = value; } }

}
