using UnityEngine;
using UnityEngine.UI;

public class WindowObstacle : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] GameObject healthImageValue;
    [SerializeField] Text health;

    [Header("Image")]
    [SerializeField] Image image;

    [Header("TMP")]
    [SerializeField] Text text;


    public void UpdateHealthBar(int current, int max)
    {
        health.text = $"{current} / {max}";

        float amount = (float)current / (float)max;

        healthImageValue.transform.localScale = new Vector3(amount, 1, 1);
    }

    public void UpdateStats(Obstacle obstacle)
    {
        if (obstacle is DestroyAbleObstacle destroyAbleObs)
        {
            image.sprite = destroyAbleObs.GetSprite;
            text.text = "2";
        }
        else
        {
            image.sprite = obstacle.GetSprite;
            text.text = "1";


            if (obstacle.CanShootThrough)
                text.text = "0";
        }
    }
}
