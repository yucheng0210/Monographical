using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonTrap : MonoBehaviour
{
    [SerializeField]
    private int trapID;
    public Character TrapData { get; private set; }
    protected virtual void Start()
    {
        TrapData = DataManager.Instance.CharacterList[trapID];
        Initialize();
    }
    protected abstract void Initialize();
}
