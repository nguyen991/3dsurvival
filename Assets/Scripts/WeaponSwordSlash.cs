using UnityEngine;

public class WeaponSwordSlash : Weapon
{
    [SerializeField]
    private ParticleSystem particleVFX;

    [SerializeField]
    private float radius = 1f;

    [SerializeField]
    private float distance = 1f;

    [SerializeField]
    private Vector2 angleRange = new Vector2(-180, 180);

    private readonly RaycastHit[] raycastHits = new RaycastHit[20];

    public override void OnAttack()
    {
        particleVFX.Play();

        // raycast sphere to detect monsters
        var hits = Physics.SphereCastNonAlloc(
            transform.parent.position,
            radius,
            transform.parent.forward,
            raycastHits,
            distance,
            layer.value
        );

        // hit monsters
        for (var i = 0; i < hits && i < raycastHits.Length; i++)
        {
            // check if the hit object is in angle range
            var direction = raycastHits[i].collider.transform.position - transform.parent.position;
            var angle = Vector3.Angle(transform.parent.forward, direction);
            if (angle < angleRange.x || angle > angleRange.y)
            {
                continue;
            }

            // hit the monster
            var monster = raycastHits[i].collider.GetComponent<DamageBody>();
            OnHit(monster);
        }
    }
}
