using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PickUpClue : MonoBehaviour
{
    public Image ItemImage { get; set; }
    public Text ItemNameText { get; set; }
    public Text ItemCountText { get; set; }
    private void Awake()
    {
        ItemImage = transform.GetChild(0).GetComponent<Image>();
        ItemNameText = transform.GetChild(1).GetComponent<Text>();
        ItemCountText = transform.GetChild(2).GetComponent<Text>();
    }
}
