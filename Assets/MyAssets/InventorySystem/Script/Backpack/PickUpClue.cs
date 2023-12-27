using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PickUpClue : MonoBehaviour
{
    public Image itemImage { get; set; }
    public Text itemNameText { get; set; }
    public Text itemCountText { get; set; }
    private void Awake()
    {
        itemImage = transform.GetChild(0).GetComponent<Image>();
        itemNameText = transform.GetChild(1).GetComponent<Text>();
        itemCountText = transform.GetChild(2).GetComponent<Text>();
    }
}
