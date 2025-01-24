using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Bullet : MonoBehaviour
{
    public GameObject hitVFX;
    public UnityEvent<DamageBody> onHit;
    public UnityEvent onDie;

    private BulletEmitter.BulletInfo bulletInfo;
    private new Rigidbody rigidbody;
    private new Collider collider;

    private GameObject hitVFXInstance;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        hitVFXInstance = Instantiate(hitVFX);
        hitVFXInstance.transform.SetParent(transform);
        hitVFXInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        hitVFXInstance.SetActive(false);
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
        onHit.Invoke(other.GetComponent<DamageBody>());
        hitVFXInstance.SetActive(true);
        hitVFXInstance.GetComponent<ParticleSystem>().Play();
        if (bulletInfo.dieOnHit)
        {
            OnDie();
        }
    }

    private void OnDie()
    {
        CancelInvoke(nameof(OnDie));
        onDie.Invoke();
        Destroy(gameObject);
    }
}
