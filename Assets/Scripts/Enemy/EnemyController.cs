using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { Idle, CombatMovement, Attack }

public class EnemyController : MonoBehaviour
{
    [field: SerializeField] public float Fov { get; private set; } = 180f;
    public List<Angam> TargetsInRange { get; set; } = new List<Angam>();

    public Angam Target { get; set; }
    public float CombatMovementTimer { get; set; } = 0f;
    public StateMachine<EnemyController> StateMachine { get; private set; }

    Dictionary<EnemyStates, State<EnemyController>> stateDict;
    public NavMeshAgent NavAgent { get; private set; }
    public Animator Animator { get; private set; }
    public Angam Fighter { get; private set; }

    private void Start()
    {
        Animator = GetComponent<Animator>();
        Fighter = GetComponent<Angam>();
        NavAgent = GetComponent<NavMeshAgent>();
        stateDict = new Dictionary<EnemyStates, State<EnemyController>>();
        stateDict[EnemyStates.Idle] = GetComponent<IdleState>();
        stateDict[EnemyStates.CombatMovement] = GetComponent<CombatMovementState>();
        stateDict[EnemyStates.Attack] = GetComponent<AttackState>();

        StateMachine = new StateMachine<EnemyController>(this);
        StateMachine.ChangeState(stateDict[EnemyStates.Idle]);
    }

    public void ChangeState(EnemyStates state)
    {
        StateMachine.ChangeState(stateDict[state]);
    }

    public bool IsInState(EnemyStates state)
    {
        return StateMachine.CurrentState == stateDict[state];
    }

    private void Update()
    {
        StateMachine.Execute();
        Animator.SetFloat("moveAmount", NavAgent.velocity.magnitude / NavAgent.speed);
    }
}
