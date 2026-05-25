using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    
    EnemyController targetEnemy;
    public EnemyController TargetEnemy
    {
        get => targetEnemy;
        set
        {
            targetEnemy = value;

            if (targetEnemy == null)
                CombatMode = false;
        }
    }

    bool combatMode;
    public bool CombatMode
    {
        get => combatMode;
        set
        {
            combatMode = value;

            if (TargetEnemy == null)
                combatMode = false;

            animator.SetBool("CombatMode", combatMode);
        }
    }

    Angam angam;
    Animator animator;
    CameraController cam;

    private void Awake()
    {
        angam = GetComponent<Angam>();
        animator = GetComponent<Animator>();
        cam = Camera.main.GetComponent<CameraController>();
    }
    private void Start()
    {
        angam.OnGotHit += (Angam attacker) =>
        {
            if (CombatMode && attacker != TargetEnemy.Fighter)
                TargetEnemy = attacker.GetComponent<EnemyController>();
        };
    }

    // Update is called once per frame
    private void Update()
    {
        // Player dead nam mokuth karanna epa
        if (angam.Health <= 0)
        {
            return;
        }
        if (Input.GetButtonDown("Attack") && !angam.IsTakingHit )
        {
            var enemy = EnemyManager.i.GetAttackingEnemy();
            /* if (enemy != null && enemy.Fighter.IsCounterable && !angam.InAction)
             {
                 StartCoroutine(angam.PerformCounterAttack(enemy));
             }
             else
             {
                 var enemyToAttack = EnemyManager.i.GetClosesEnemyToDirection(map2PlayerController.i.GetIntentDirection());


                 angam.TryToAttack(enemyToAttack?.Fighter);
                 CombatMode = true;
             }*/
            var enemyToAttack =
            EnemyManager.i.GetClosesEnemyToDirection(map2PlayerController.i.GetIntentDirection());

            angam.TryToAttack(enemyToAttack?.Fighter);
            CombatMode = true;
            //....................................................................
        }
        if (Input.GetButtonDown("LockOn"))
        {
            CombatMode = !CombatMode;
        }
    }
    private void OnAnimatorMove()
    {
        if (!angam.InCounter) 
           transform.position += animator.deltaPosition;
        transform.rotation *= animator.deltaRotation;
    }
    public Vector3 GetTargetingDir()
    {
        if (!combatMode)
        {
            var vecFromCam = transform.position - cam.transform.position;
            vecFromCam.y = 0f;
            return vecFromCam.normalized;
        }
        else
        {
            return transform.forward;
        }
    }

}
