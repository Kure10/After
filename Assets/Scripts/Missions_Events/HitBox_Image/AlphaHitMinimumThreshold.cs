using UnityEngine;
using UnityEngine.UI;


public class AlphaHitMinimumThreshold : MonoBehaviour
{

    private Image mask;

    [Range(0f,1f)]
    [SerializeField] private float alphaThreshold;

    private void Awake()
    {
        mask = GetComponent<Image>();
    }

    void Start()
    {
        mask.alphaHitTestMinimumThreshold = alphaThreshold;
    }

}
