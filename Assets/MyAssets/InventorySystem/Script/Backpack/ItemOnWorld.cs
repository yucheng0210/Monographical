using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld : MonoBehaviour
{
    [SerializeField]
    private int itemIndex;
    private Item thisItem;

    private void Start()
    {
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventLoadDataFinish,
            EventLoadDataFinish
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.CompareTag("Item"))
                BackpackManager.Instance.AddItem(thisItem);
            else if (gameObject.CompareTag("Money"))
                BackpackManager.Instance.AddMoney(100);
            Destroy(gameObject);
        }
    }

    private void EventLoadDataFinish(params object[] args)
    {
        thisItem = BackpackManager.Instance.ItemList[itemIndex];
    }
}
