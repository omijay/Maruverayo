using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State<EnemyController>
{
    public override void Enter(EnemyController owner)
    {
        owner.VisionSensor.gameObject.SetActive(false);
        EnemyManager.i.RemoveEnemyInRange(owner);

        owner.NavAgent.enabled = false;
        // owner.CharacterController.enabled = false;

        CharacterController cc = owner.CharacterController;

      
            cc.radius = 0.01f;
            cc.height = 0.02f;
          
    }
}