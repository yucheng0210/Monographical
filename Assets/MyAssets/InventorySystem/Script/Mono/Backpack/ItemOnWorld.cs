using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld : MonoBehaviour
{
    [SerializeField]
    private Item_SO thisItem;

    private BackpackManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<BackpackManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.CompareTag("Item"))
                manager.AddItem(thisItem);
            else if (gameObject.CompareTag("Money"))
                manager.AddMoney(100);
            Destroy(gameObject);
        }
    }
}
