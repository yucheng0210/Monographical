using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthController : MonoBehaviour
{
    private void Awake()
    {
        healthSlider = GetComponent<Slider>();
        healthSlider.value = 1;
    }

    private Slider healthSlider;
    public Image fillImage;
    public float r, g, b;
    public RectTransform LifePoint, Hurt;

    [SerializeField]
    private bool isDiscolor;

    private void Update()
    {
        Hurt.anchorMax = new Vector2(
            Mathf.Lerp(Hurt.anchorMax.x, LifePoint.anchorMax.x, 1 * Time.deltaTime * 5),
            Hurt.anchorMax.y
        );
        if (isDiscolor)
            fillImage.color = new Color(r / healthSlider.value, g * healthSlider.value * 2, b);
    }
}
