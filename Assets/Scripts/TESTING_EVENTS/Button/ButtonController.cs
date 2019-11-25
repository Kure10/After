using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ButtonController : MonoBehaviour //, IPointerClickHandler
{
    private GameObject instantExploreButton;

    private Image image;
    public Color idleColor;
    public Color hoverColor;
    public Color clickColor;
    [Header("Parents")]
    [SerializeField] Transform exploreButtonParent;
  //  [SerializeField] Button exploreButton;
    [Header("Prefabs")]
    //[SerializeField] Button exploreButtonPrefab;
    [SerializeField] GameObject exploreQuestionButton;

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
        CheckExploreButton();
        StartCoroutine("OneClick"); 
    }


    private IEnumerator OneClick()
    {
        image.color = clickColor;
        yield return new WaitForSeconds(0.1f);
        image.color = idleColor;
        yield return null;
    }

    public void CheckExploreButton ()
    {
        if (Input.GetKey(KeyCode.Mouse2))
        {
            Debug.Log("banik pico");
            if (instantExploreButton == null)
                DoyouWannaExplore();
        }
        else
        {
            if (instantExploreButton != null)
                instantExploreButton.SetActive(false);
        }
    }

    public void DoyouWannaExplore()
    {
        instantExploreButton = Instantiate(exploreQuestionButton, Input.mousePosition, Quaternion.identity);
        instantExploreButton.gameObject.transform.SetParent(exploreButtonParent);
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    // Debug.Log("??????");
    //    //throw new System.NotImplementedException();
    //    if (Input.GetKey(KeyCode.Mouse0))
    //    {
    //        Debug.Log("jsemtady");
    //        instantExploreButton.SetActive(false);
    //    }
    //    else
    //    {
    //        Debug.Log("jsdasdsa");
    //    }
    //}
}
