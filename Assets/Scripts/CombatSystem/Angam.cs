using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attackstates {Idle,Windup,Impact,Cooldown}

public class Angam : MonoBehaviour

{
    [field: SerializeField] public float Health { get; private set; } = 25f;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;
    [SerializeField] List<AttackData> attacks;
    [SerializeField] List<AttackData> longRangeAttacks;
    [SerializeField] float longRangeAttackThreshold = 1.5f;

    [SerializeField] float rotationSpeed = 500f;

    public bool IsTakingHit { get; private set; } = false;
    public event Action<Angam> OnGotHit; 
    public event Action OnHitComplete; 

 
    SphereCollider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;

    Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        if (leftHand != null)
        {
            leftHandCollider = animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<SphereCollider>();
            rightHandCollider = animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<SphereCollider>();
            rightFootCollider = animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<SphereCollider>();
            leftFootCollider = animator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponent<SphereCollider>();
            
            DisableAllColliders();
        }
    }
    public Attackstates Attackstate {  get; private set; }
    bool doCombo;
    int comboCount = 0;
    public bool InAction { get; private set; } = false;
    public bool InCounter { get; set; } = false;

    public void TryToAttack(Angam target = null)
    {
        if (!InAction)
        {
           StartCoroutine(Attack(target));

        }
        else if (Attackstate == Attackstates.Impact || Attackstate == Attackstates.Cooldown)
        {
            doCombo = true;
        }
    }
    Angam currTarget;
    IEnumerator Attack(Angam target = null)
    {
        InAction = true;
        currTarget = target;
        Attackstate = Attackstates.Windup;
        var attack = attacks[comboCount];

        var attackDir = transform.forward;
        Vector3 startPos = transform.position;
        Vector3 targetPos = Vector3.zero;
        if (target != null)
        {
            var vecToTarget = target.transform.position - transform.position;
            vecToTarget.y = 0;
            attackDir = vecToTarget.normalized;
            float distance = vecToTarget.magnitude - attack.DistanceFromTarget;

            if (distance > longRangeAttackThreshold && longRangeAttacks.Count>0)
            {
                attack = longRangeAttacks[0];
            }
            if (attack.MoveToTarget)
            {
                if (distance <= attack.MaxMoveDistance)
                    targetPos = target.transform.position - attackDir * attack.DistanceFromTarget;
                else
                    targetPos = startPos + attackDir * attack.MaxMoveDistance;
            }
        }

        animator.CrossFade(attack.AnimName, 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        while (timer <= animState.length)
        {
            if (IsTakingHit) break;
            timer += Time.deltaTime;
            float normalizedTime= timer/animState.length;

            // Move the attacker towards the target while performing attack
            if (target != null && attack.MoveToTarget)
            {
                float percTime = (normalizedTime - attack.MoveStartTime) / (attack.MoveEndTime - attack.MoveStartTime);
                transform.position = Vector3.Lerp(startPos, targetPos, percTime);

            }

            if (attackDir != null)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attackDir), rotationSpeed * Time.deltaTime);
            }

            if (Attackstate == Attackstates.Windup) 
            {
                if (InCounter) break;
                if (normalizedTime >= attack.ImpactStartTime) 
                { 
                  Attackstate = Attackstates.Impact;
                  EnableHitBox(attack);
                }
            }
            else if (Attackstate == Attackstates.Impact)
            {
                if (normalizedTime >=attack.ImpactEndTime)
                {
                    Attackstate = Attackstates.Cooldown;
                   DisableAllColliders();
                }
            }
            else if (Attackstate == Attackstates.Cooldown)
            {
                if (doCombo) 
                {
                    doCombo = false;
                    comboCount = (comboCount + 1) % attacks.Count;

                    StartCoroutine(Attack(target));
                    yield break;
                
                }
            }
            yield return null;
        }

        Attackstate = Attackstates.Idle;
        comboCount = 0;
        InAction = false;
        currTarget = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hitbox" && !IsTakingHit && !InCounter) 
        {
            var attacker = other.GetComponentInParent<Angam>();
            if (attacker.currTarget != this)
                return;

            TakeDamage(5f);
            OnGotHit?.Invoke(attacker);

            if (Health > 0)
                StartCoroutine(PlayHitReaction(attacker));
            else
                PlayDeathAnimation(attacker);


        }
            
    }
    void TakeDamage(float damage)
    {
        Health = Mathf.Clamp(Health - damage, 0, Health);
    }
    IEnumerator PlayHitReaction(Angam attacker)
    {
        InAction = true;
        IsTakingHit = true;

        var dispVec = attacker.transform.position - transform.position;
        dispVec.y = 0f;
        transform.rotation = Quaternion.LookRotation(dispVec);



        animator.CrossFade("Damage_Front_Small_ver_C", 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);
        yield return new WaitForSeconds(animState.length * 0.8f);

        OnHitComplete?.Invoke();
        InAction = false;
        IsTakingHit = false;
    }
    void PlayDeathAnimation(Angam attacker)
    {
       
        animator.CrossFade("FallBackDeath", 0.2f);
    }
    public IEnumerator PerformCounterAttack(EnemyController opponent)
    {
        InAction = true;
        var dispVec = opponent.transform.position - transform.position;
        dispVec.y = 0f;
        transform.rotation = Quaternion.LookRotation(dispVec);
        opponent.transform.rotation = Quaternion.LookRotation(-dispVec);

        var targetPos = opponent.transform.position - dispVec.normalized * 1f;
        animator.CrossFade("CounterAttack", 0.2f);
        opponent.Animator.CrossFade("CounterAttackVictim", 0.2f);
        opponent.ChangeState(EnemyStates.Dead);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);
        float timer = 0f;
        while (timer <= animState.length)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 5 * Time.deltaTime);

            yield return null;

            timer += Time.deltaTime;
        }

        InCounter = false;
        opponent.Fighter.InCounter = false;

        InAction = false;
    }
    void EnableHitBox(AttackData attack)
    {
        switch (attack.HitboxToUse)
        {
            case AttackHitbox.LeftHand:
                leftHandCollider.enabled = true;
                break;
            case AttackHitbox.RightHand:
                rightHandCollider.enabled = true;
                break;
            case AttackHitbox.LeftFoot:
                leftFootCollider.enabled = true;
                break;
            case AttackHitbox.RightFoot:
                rightFootCollider.enabled = true;
                break;
            default:
                break;
        }
    }
    void DisableAllColliders()
    {
        if (leftHandCollider != null) 
         leftHandCollider.enabled = false;
        if (rightHandCollider != null)
         rightHandCollider.enabled = false;
        if (rightFootCollider  != null) 
          rightFootCollider.enabled = false;
        if (leftFootCollider  != null) 
         leftFootCollider.enabled = false;

    }
    public List<AttackData> Attacks => attacks;

    public bool IsCounterable => Attackstate == Attackstates.Windup && comboCount == 0; 
}
