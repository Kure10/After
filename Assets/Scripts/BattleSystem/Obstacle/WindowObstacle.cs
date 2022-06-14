using UnityEngine;
using UnityEngine.UI;

public class WindowObstacle : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] GameObject healthImageValue;
    [SerializeField] Text health;

    [Header("Image")]
    [SerializeField] Image image;

    public void UpdateHealthBar(int current, int max)
    {
        health.text = $"{current} / {max}";

        float amount = (float)current / (float)max;

        healthImageValue.transform.localScale = new Vector3(amount, 1, 1);
    }

    public void UpdateStats(Obstacle obstacle)
    {
        image.sprite = obstacle.GetSprite;

        if (obstacle is IDamageable damagable)
            UpdateHealthBar(damagable.GetCurrentHealth, damagable.GetMaxHealth);
    }
}
