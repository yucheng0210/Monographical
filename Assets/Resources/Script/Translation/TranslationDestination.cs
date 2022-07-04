using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationDestination : MonoBehaviour
{
    public enum DestinationTag
    {
        Enter,
        A,
        B
    }

    [SerializeField]
    private DestinationTag destinationTag;
    public DestinationTag Tag
    {
        get { return destinationTag; }
    }
}
