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
    public Color idleColor;
    public Color hoverColor;
    public Color clickColor;

    [Header("Prefabs")]
    [SerializeField] GameObject exploreQuestionButton;
    [Header("Parents")]
    [SerializeField] Transform exploreButtonParent;


    public void Awake()
    {
        this.image = GetComponent<Image>();
        this.regionControler = GetComponent<RegionControler>();
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Region region = this.regionControler.GetRegion;
            StartCoroutine(ClickFeedBack());
            if (region.IsOutOfReach || region.IsExplored) 
                return;

            DoyouWannaExplore();
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

    public void SwitchHover(bool flag)
    {
        image.color = flag ? hoverColor : idleColor;
    }

    public void DoyouWannaExplore()
    {
        instantExploreButton = Instantiate(exploreQuestionButton, Input.mousePosition, Quaternion.identity);
        instantExploreButton.gameObject.transform.SetParent(exploreButtonParent);
        uButtonExploreScript exploreButton = instantExploreButton.GetComponent<uButtonExploreScript>();
        exploreButton.In(this.regionControler);
        //  Button button = this.instantExploreButton.GetComponent<Button>();

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
