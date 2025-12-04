using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    Angam angam;
    Animator animator;

    private void Awake()
    {
        angam = GetComponent<Angam>();
        animator = GetComponent<Animator>();
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
            }
        }
    }
    private void OnAnimatorMove()
    {
        if (!angam.InCounter) 
           transform.position += animator.deltaPosition;
        transform.rotation *= animator.deltaRotation;
    }
}
