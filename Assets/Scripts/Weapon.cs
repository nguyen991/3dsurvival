using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float startDelay = 0f;
    public float damage = 0f;
    public float attackSpeed = 1f;
    public LayerMask layer;

    private bool isActive = false;
    private IDisposable attackSubscription;

    private void OnDisable()
    {
        attackSubscription?.Dispose();
    }

    public void SetActive(bool active)
    {
        if (active && !isActive)
        {
            isActive = true;
            AttackRoutine();
        }
        else if (!active && isActive)
        {
            isActive = false;
        }
    }

    private void AttackRoutine()
    {
        attackSubscription?.Dispose();
        attackSubscription = Observable
            .Timer(TimeSpan.FromSeconds(startDelay), TimeSpan.FromSeconds(1f / attackSpeed))
            .Subscribe(_ => OnAttack())
            .AddTo(this);
    }

    public virtual void OnAttack() { }

    protected void OnHit(DamageBody body)
    {
        if (body == null)
        {
            Debug.LogWarning("DamageBody is null");
            return;
        }
        body.TakeDamage(damage);
    }
}
