using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RegionButtonControler : MonoBehaviour , IPointerClickHandler
{
    private GameObject instantExploreButton;
    private Image image;
    private RegionControler regionControler;

    [Header("Events")]
    public UnityEvent leftClick;
    public UnityEvent middleClick;
    public UnityEvent rightClick;
    [Header("Settings")]
    public Color idleColor = new Color(1,1,1,1);
    public Color hoverColor = new Color(1,1,1,0.4f);
    public Color clickColor = new Color(0,0,0,1);

    [Header("Prefabs")]
    [SerializeField] GameObject exploreQuestionButton;
    //[Header("Parents")]
    //[SerializeField] Transform exploreButtonParent;


    public void Awake()
    {
        this.image = GetComponent<Image>();
       // this.regionControler = GetComponent<RegionControler>();
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
           // Region region = this.regionControler.GetRegion;
           
          //  if (region.IsInDarkness || region.IsExplored) 
            //    return;

            DoyouWannaExplore(eventData);
                // leftClick.Invoke();
            Debug.Log("Left Clicked");
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
          //  middleClick.Invoke();
            Debug.Log("Middle Clicked");
        } 
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
           // rightClick.Invoke();
            CloseExplorePanel();
            Debug.Log("Right Clicked");
        }  
    }

    //public void SwitchHover(bool flag)
    //{
    //    image.color = flag ? hoverColor : idleColor;
    //}

    public void DoyouWannaExplore(PointerEventData eventData)
    {
        exploreQuestionButton.SetActive(true);
        exploreQuestionButton.transform.position = Input.mousePosition;
        uButtonExploreScript exploreButton = exploreQuestionButton.GetComponent<uButtonExploreScript>();
       // exploreButton.In(this.regionControler);
    }

    private void CloseExplorePanel()
    {
        this.instantExploreButton.SetActive(false);
    }

    private IEnumerator ClickFeedBack()
    {
        image.color = clickColor;
        yield return new WaitForSeconds(0.2f);
        image.color = idleColor;
        yield return null;
    }

}
