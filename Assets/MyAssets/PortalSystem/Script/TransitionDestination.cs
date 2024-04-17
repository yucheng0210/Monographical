using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDestination : MonoBehaviour
{
    public enum DestinationTag
    {
        Enter,
        A,
        B,
        C,
        Empire
    }

    [SerializeField]
    private DestinationTag destinationTag;
    public DestinationTag Tag
    {
        get { return destinationTag; }
    }
}
