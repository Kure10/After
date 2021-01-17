using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ContainerManager : MonoBehaviour
{
    [SerializeField]
    List<ContainerInfo> Info = new List<ContainerInfo>();
    //ContainerInfo Info;
    // Start is called before the first frame update
    void Start()
    {
        //ContainerXmlLoader xmlLoader = new ContainerXmlLoader();
        //Container startingItems = new Container();
        //startingItems = xmlLoader.GetContainerByNumber(Info[0].id);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Container GetStartContainer ()
    {
        ContainerXmlLoader xmlLoader = new ContainerXmlLoader();
        Container startingItems = new Container();
        startingItems = xmlLoader.GetContainerByNumber(Info[0].id);

        return startingItems;
    }


    [System.Serializable]
    public struct ContainerInfo
    {
        public string name;
        public long id;
    }
}
