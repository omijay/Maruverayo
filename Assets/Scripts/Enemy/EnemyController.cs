using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { Idle, CombatMovement }

public class EnemyController : MonoBehaviour
{
    [field: SerializeField] public float Fov { get; private set; } = 180f;
    public List<Angam> TargetsInRange { get; set; } = new List<Angam>();

    public Angam Target { get; set; }
    public StateMachine<EnemyController> StateMachine { get; private set; }

    Dictionary<EnemyStates, State<EnemyController>> stateDict;
    public NavMeshAgent NavAgent { get; private set; }
    public Animator Animator { get; private set; }

    private void Start()
    {
        Animator = GetComponent<Animator>();
        NavAgent = GetComponent<NavMeshAgent>();
        stateDict = new Dictionary<EnemyStates, State<EnemyController>>();
        stateDict[EnemyStates.Idle] = GetComponent<IdleState>();
        stateDict[EnemyStates.CombatMovement] = GetComponent<CombatMovementState>();

        StateMachine = new StateMachine<EnemyController>(this);
        StateMachine.ChangeState(stateDict[EnemyStates.Idle]);
    }

    public void ChangeState(EnemyStates state)
    {
        StateMachine.ChangeState(stateDict[state]);
    }

    private void Update()
    {
        StateMachine.Execute();
        Animator.SetFloat("moveAmount", NavAgent.velocity.magnitude / NavAgent.speed);
    }
}
