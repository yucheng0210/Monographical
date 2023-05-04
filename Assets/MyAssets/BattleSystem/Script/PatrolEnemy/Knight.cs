using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//using RootMotion.FinalIK;

public class Knight : PatrolEnemy
{
    public void JumpAttackColliderSwitch(int count)
    {
        if (count == 1)
            RockBreak.GetComponent<BoxCollider>().enabled = true;
        else
            RockBreak.GetComponent<BoxCollider>().enabled = false;
    }
}
