using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//using RootMotion.FinalIK;

public class Knight : PatrolEnemy
{
    [SerializeField]
    private GameObject jumpCollision;

    public void JumpAttackColliderSwitch(int count)
    {
        if (count == 1)
            jumpCollision.SetActive(true);
        else
            jumpCollision.SetActive(false);
    }
}
