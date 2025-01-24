using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 0f;
    public float attackSpeed = 1f;
    public LayerMask layer;

    private bool isActive = false;

    public void SetActive(bool active)
    {
        if (active && !isActive)
        {
            isActive = true;
            StartCoroutine(AttackRoutine());
        }
        else if (!active && isActive)
        {
            isActive = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(attackSpeed);
            OnAttack();
        }
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
