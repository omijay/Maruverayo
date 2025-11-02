using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angam : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public bool InAction { get; private set; } = false;

    public void TryToAttack()
    {
        if (!InAction)
        {
           StartCoroutine(Attack());

        }
    }
    IEnumerator Attack()
    {
        InAction = true;
        animator.CrossFade("Attack_Left_Middle_Punch", 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);
        yield return new WaitForSeconds(animState.length);
        InAction = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hitbox" && !InAction) 
        {
            StartCoroutine(PlayHitReaction());

        }
            
    }
    IEnumerator PlayHitReaction()
    {
        InAction = true;
        animator.CrossFade("Damage_Front_Small_ver_C", 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);
        yield return new WaitForSeconds(animState.length);
        InAction = false;
    }
}
