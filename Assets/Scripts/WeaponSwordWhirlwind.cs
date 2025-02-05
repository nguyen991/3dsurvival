using PrimeTween;
using R3;
using R3.Triggers;
using UnityEngine;

public class WeaponSwordWhirlwind : Weapon
{
    public float rotateSpeed = 20f;

    private bool initialized = false;
    private Transform parent = null;

    public override void OnAttack()
    {
        if (initialized)
        {
            return;
        }
        initialized = true;

        // detach weapon from player
        parent = transform.parent;
        transform.SetParent(null);

        var bodies = GetComponentsInChildren<Rigidbody>();
        foreach (var body in bodies)
        {
            var collider = body.GetComponent<Collider>();
            collider.includeLayers = layer;

            body.includeLayers = layer;
            body.OnTriggerEnterAsObservable().Subscribe(OnHit).AddTo(this);

            // play particle
            var particle = body.GetComponent<ParticleSystem>();
            particle.Play();
        }

        // rotate weapon
        Tween.LocalRotationAtSpeed(
            transform,
            Quaternion.Euler(0, 180, 0),
            rotateSpeed,
            cycles: -1,
            cycleMode: CycleMode.Incremental,
            ease: Ease.Linear
        );
    }

    private void Update()
    {
        if (!initialized || parent == null)
        {
            return;
        }
        transform.position = parent.position;
    }

    private void OnHit(Collider other)
    {
        if (!other.TryGetComponent<DamageBody>(out var body))
        {
            Debug.LogWarning("DamageBody is null");
            return;
        }
        body.TakeDamage(damage);
    }
}
