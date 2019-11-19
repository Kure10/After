using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheatControler : MonoBehaviour , IPointerClickHandler
{
    
    enum ResourceType { food, civil, tech, military }

    [SerializeField]
    ResourceType type = ResourceType.food;

    private ResourceManager resourceManager;

    // Start is called before the first frame update
    void Start()
    {
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
    }

    // Update is called once per frame
    void Update()
    {
       // PressedKeyCode();
    }

    // Obsolete
    //private void PressedKeyCode()
    //{
    //    if (EventSystem.current.IsPointerOverGameObject())
    //    {
    //        if (Input.GetKey(KeyCode.LeftControl))
    //        {
    //            IsCtrPressed = true;
    //        }
    //        else if (Input.GetKey(KeyCode.LeftAlt))
    //        {
    //            IsAltPressed = true;
    //        }
    //        else
    //        {
    //            IsAltPressed = false;
    //            IsCtrPressed = false;
    //        }
    //    }
    //}

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button ==  PointerEventData.InputButton.Left)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                AddResource((int)type, 10);
            }
            else
            {
                AddResource((int)type, 1);
            }
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                AddResource((int)type , -10);
            }
            else
            {
                AddResource((int)type, -1);
            }
        }

    }

    private void AddResource(int resourceType, int value)
    {
        switch (resourceType)
        {
            case 0:
                resourceManager.IncPotraviny(value);
                    break;
            case 1:
                resourceManager.IncCivilniMaterial(value);
                    break;
            case 2:
                resourceManager.IncTechnickyMaterial(value);
                break;
            case 3:
                resourceManager.IncVojenskyMaterialy(value);
                break;
            default:
                break;
        }
    }
}


