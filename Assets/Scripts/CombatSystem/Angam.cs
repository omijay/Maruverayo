using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attackstate{Idle,Windup,Impact,Cooldown}

public class Angam : MonoBehaviour

{
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;
    [SerializeField] List<AttackData> attacks;

    BoxCollider leftHandCollider;
    BoxCollider rightHandCollider;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        if (leftHand != null)
        {
            leftHandCollider = leftHand.GetComponent<BoxCollider>();
            leftHandCollider.enabled = false;
        }
        if (rightHand != null)
        {
            rightHandCollider = rightHand.GetComponent<BoxCollider>();
            rightHandCollider.enabled = false;
        }
    }
    Attackstate attackstate;
    bool doCombo;
    int comboCount = 0;
    public bool InAction { get; private set; } = false;

    public void TryToAttack()
    {
        if (!InAction)
        {
           StartCoroutine(Attack());

        }
        else if (attackstate == Attackstate.Impact || attackstate == Attackstate.Cooldown)
        {
            doCombo = true;
        }
    }
    IEnumerator Attack()
    {
        InAction = true;
        attackstate = Attackstate.Windup;


        animator.CrossFade(attacks[comboCount].AnimName, 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime= timer/animState.length;

            if (attackstate == Attackstate.Windup) 
            {
                if (normalizedTime >= attacks[comboCount].ImpactStartTime) 
                { 
                  attackstate = Attackstate.Impact;
                    leftHandCollider.enabled = true;
                    rightHandCollider.enabled = true;
                }
            }
            else if (attackstate == Attackstate.Impact)
            {
                if (normalizedTime >=attacks[comboCount].ImpactEndTime)
                {
                    attackstate = Attackstate.Cooldown;
                    rightHandCollider.enabled = false;
                    leftHandCollider.enabled = false;
                }
            }
            else if (attackstate == Attackstate.Cooldown)
            {
                if (doCombo) 
                {
                    doCombo = false;
                    comboCount = (comboCount + 1) % attacks.Count;

                    StartCoroutine(Attack());
                    yield break;
                
                }
            }
            yield return null;
        }

        attackstate = Attackstate.Idle;
        comboCount = 0;
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
        yield return new WaitForSeconds(animState.length * 0.8f);
        InAction = false;
    }
}
