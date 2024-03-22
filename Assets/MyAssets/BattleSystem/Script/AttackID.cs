using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackID : MonoBehaviour
{
    [SerializeField]
    private int id;
    public Character AttackData { get; private set; }
    private void OnEnable()
    {
        AttackData = DataManager.Instance.CharacterList[id];
    }
}
