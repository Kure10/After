using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScript : MonoBehaviour
{

    public GameObject Panel;

    public void OpenPanel()
    {
        if(Panel !=  null)
        {
            Animator animator = Panel.GetComponent<Animator>();

            if(animator != null)
            {
                bool isOpen = animator.GetBool("isOpen");
                animator.SetBool("isOpen", !isOpen);
            }
        }
    }

}

