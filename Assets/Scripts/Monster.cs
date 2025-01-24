using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

enum MonsterState
{
    Idle,
    Move,
    Attack,
    Dead
}

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class Monster : MonoBehaviour
{
    [SerializeField]
    private float speed = 2f;

    [SerializeField]
    private float rotateSpeed = 10f;

    [SerializeField]
    private float attackRange = 1f;

    private Animator animator;
    private CharacterController controller;

    [HideInInspector]
    public Transform target;

    private MonsterState state;
    private Vector3 direction;
    private float attackRangeMagnitude;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        attackRangeMagnitude = attackRange * attackRange;

        var damageBody = GetComponentInChildren<DamageBody>();
        damageBody.onDeath.AddListener(() => OnDead());
        damageBody.onDamage.AddListener(() => OnDamage());
    }

    private void OnEnable()
    {
        controller.detectCollisions = true;
        controller.enabled = true;
        animator.ResetTrigger("die");
        animator.ResetTrigger("attack");
        animator.SetBool("move", false);
        animator.Play("Idle");
        state = MonsterState.Idle;
    }

    private bool IsPlayerInRange
    {
        get => direction.magnitude <= attackRangeMagnitude;
    }

    public void DoUpdate()
    {
        if (state == MonsterState.Dead)
        {
            return;
        }

        direction = target.position - transform.position;
        if (state == MonsterState.Attack)
        {
            if (!IsPlayerInRange)
            {
                state = MonsterState.Move;
                MoveToTarget();
            }
        }
        else
        {
            if (IsPlayerInRange)
            {
                Attack();
            }
            else
            {
                MoveToTarget();
            }
        }
        RotateTowardsDirection();
    }

    private void MoveToTarget()
    {
        controller.Move(speed * Time.deltaTime * direction.normalized);
        animator.SetBool("move", true);
        animator.ResetTrigger("attack");
    }

    private void RotateTowardsDirection()
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            rotateSpeed * Time.deltaTime
        );
    }

    private void Attack()
    {
        animator.SetTrigger("attack");
        animator.SetBool("move", false);
        state = MonsterState.Attack;
    }

    private void OnAttack() { }

    private void OnDead()
    {
        state = MonsterState.Dead;
        controller.detectCollisions = false;
        controller.enabled = false;
        animator.SetTrigger("die");
        animator.ResetTrigger("attack");
        animator.SetBool("move", false);
    }

    private void OnDeadAnimationEnd()
    {
        Debug.Log("Monster is dead " + gameObject.name);
        gameObject.SetActive(false);
    }

    private void OnDamage() { }
}
