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
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            var enemy = EnemyManager.i.GetAttackingEnemy();
            if (enemy != null && enemy.Fighter.IsCounterable && !angam.InAction)
            {
                StartCoroutine(angam.PerformCounterAttack(enemy));
            }
            else
            {
                angam.TryToAttack();
                CombatMode = true;
            }
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
        var vecFromCam = transform.position - cam.transform.position;
        vecFromCam.y = 0f;
        return vecFromCam.normalized;
    }
}
