using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State<EnemyController>
{
    [SerializeField] float attackDistance = 1f;

    bool isAttacking;

    EnemyController enemy;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;

        enemy.NavAgent.stoppingDistance = attackDistance;
    }

    public override void Execute()
    {
        if (isAttacking) return;

        enemy.NavAgent.SetDestination(enemy.Target.transform.position);

        if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= attackDistance + 0.03f)
            StartCoroutine(Attack());

    }
    IEnumerator Attack()
    {
        isAttacking = true;
        enemy.Animator.applyRootMotion = true;

        enemy.Fighter.TryToAttack();
        yield return new WaitUntil(() => enemy.Fighter.Attackstate == Attackstates.Idle);

        enemy.Animator.applyRootMotion = false;
        isAttacking = false;
    }
}