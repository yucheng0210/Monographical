using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld : MonoBehaviour
{
    [SerializeField]
    private int itemIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.CompareTag("Item"))
                BackpackManager.Instance.AddItem(DataManager.Instance.ItemList[itemIndex]);
            else if (gameObject.CompareTag("Money"))
                BackpackManager.Instance.AddMoney(100);
            Destroy(gameObject);
        }
    }
}
