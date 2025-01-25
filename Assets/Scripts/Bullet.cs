using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Bullet : MonoBehaviour
{
    public GameObject hitVFX;

    [HideInInspector]
    public UnityEvent<DamageBody> onHit;

    [HideInInspector]
    public UnityEvent onDie;

    private BulletEmitter.BulletInfo bulletInfo;
    private new Rigidbody rigidbody;
    private new Collider collider;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        bulletInfo = null;
    }

    private void OnDisable()
    {
        onHit.RemoveAllListeners();
    }

    public void SetBulletInfo(BulletEmitter.BulletInfo info)
    {
        bulletInfo = info;
        collider.includeLayers = info.hitLayer;
        rigidbody.includeLayers = info.hitLayer;
        Invoke(nameof(OnDie), info.bulletLifeTime);
    }

    private void Update()
    {
        if (bulletInfo == null)
        {
            return;
        }
        rigidbody.MovePosition(
            transform.position + transform.forward * bulletInfo.bulletSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        // trigger hit event
        onHit.Invoke(other.GetComponent<DamageBody>());

        // swap hit vfx
        if (hitVFX)
        {
            BulletManager.Instance.SpawnHitVFX(hitVFX, transform.position, transform.rotation);
        }

        // kill bullet
        if (bulletInfo.dieOnHit)
        {
            OnDie();
        }
    }

    private void OnDie()
    {
        CancelInvoke(nameof(OnDie));
        onDie.Invoke();
    }
}
