using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheatControler : MonoBehaviour 
{

    //[SerializeField] private Button button;
    [SerializeField]
    private ResourceManager resourceManager;

    // Start is called before the first frame update
    void Start()
    {
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
       // button.GetComponent<Button>();
        // button.onClick.AddListener(() => resourceManager.IncPotraviny(10));
        // button.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                resourceManager.ResValue = 10;
                Debug.Log("ctr " + resourceManager.ResValue);
            }
            else if (Input.GetKey(KeyCode.LeftAlt))
            {
                resourceManager.ResValue = -10;
                Debug.Log("alt " + resourceManager.ResValue);
            }
            else
            {
                resourceManager.ResValue = 1;
                Debug.Log("nic " + resourceManager.ResValue);
            }
            
        }
    }

}
