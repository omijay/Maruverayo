using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State<EnemyController>
{
    public override void Enter(EnemyController owner)
    {
        Debug.Log("Entered Chase State");
    }

    public override void Execute()
    {
        Debug.Log("Executing Chase State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Chase State");
    }
}