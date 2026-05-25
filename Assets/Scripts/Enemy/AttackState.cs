using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State<EnemyController>
{
    [SerializeField] float attackDistance = 1f;

    bool isAttacking;

    EnemyController enemy;

    Coroutine attackRoutine;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;

        enemy.NavAgent.stoppingDistance = attackDistance;

        isAttacking = false;
    }

    public override void Execute()
    {
        // hit reaction eka athara attack nathara ganna
        if (enemy.Fighter.IsTakingHit)
        {
            StopAttack();
            return;
        }

        if (isAttacking) return;

        enemy.NavAgent.SetDestination(enemy.Target.transform.position);

        if (Vector3.Distance(enemy.Target.transform.position,
            enemy.transform.position) <= attackDistance + 0.03f)
        {
            attackRoutine =
                StartCoroutine(Attack(Random.Range(1,
                enemy.Fighter.Attacks.Count + 1)));
        }
    }

    IEnumerator Attack(int comboCount = 1)
    {
        isAttacking = true;

        enemy.Animator.applyRootMotion = true;

        enemy.Fighter.TryToAttack(enemy.Target);

        for (int i = 1; i < comboCount; i++)
        {
            yield return new WaitUntil(() =>
                enemy.Fighter.Attackstate == Attackstates.Cooldown);

            // hit unoth attack cancel
            if (enemy.Fighter.IsTakingHit)
            {
                StopAttack();
                yield break;
            }

            enemy.Fighter.TryToAttack(enemy.Target);
        }

        yield return new WaitUntil(() =>
            enemy.Fighter.Attackstate == Attackstates.Idle
            || enemy.Fighter.IsTakingHit);

        // hit unoth attack cancel
        if (enemy.Fighter.IsTakingHit)
        {
            StopAttack();
            yield break;
        }

        enemy.Animator.applyRootMotion = false;

        isAttacking = false;

        if (enemy.IsInState(EnemyStates.Attack))
        {
            enemy.ChangeState(EnemyStates.RetreatAfterAttack);
        }
    }

    void StopAttack()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        enemy.Animator.applyRootMotion = false;

        enemy.NavAgent.ResetPath();

        isAttacking = false;
    }

    public override void Exit()
    {
        StopAttack();
    }
}