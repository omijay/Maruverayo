using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State<EnemyController>
{
    EnemyController enemy;
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        Debug.Log("Entered Idle State");
    }

    public override void Execute()
    {
        Debug.Log("Executing Idle State");

        if (Input.GetKeyDown(KeyCode.T))
            enemy.ChangeState(EnemyStates.Chase);
    }

    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}
