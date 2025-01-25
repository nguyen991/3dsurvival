using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletEmitter : Weapon
{
    [System.Serializable]
    public class BulletInfo
    {
        public GameObject prefab;
        public float bulletSpeed = 10f;
        public float bulletLifeTime = 2f;
        public bool dieOnHit = true;

        [HideInInspector]
        public LayerMask hitLayer;

        [HideInInspector]
        public float damage;
    }

    public bool worldPosition = false;
    public BulletInfo bulletInfo;
    public float shotRate = 0.1f;
    public int shotCount = 1;

    private bool isShooting = false;
    private int count = 0;
    private float dt = 0f;

    private List<BulletEmitter> subEmitters;

    private void Awake()
    {
        // init sub emitters
        subEmitters = new List<BulletEmitter>();
        foreach (Transform child in transform)
        {
            var emitter = child.GetComponent<BulletEmitter>();
            if (emitter)
            {
                subEmitters.Add(emitter);
            }
        }
    }

    public override void OnAttack()
    {
        if (isShooting)
        {
            return;
        }
        isShooting = true;
        count = 0;
        dt = 0f;

        // fire sub emitters
        foreach (var emitter in subEmitters)
        {
            emitter.OnAttack();
        }
    }

    private void Update()
    {
        if (!isShooting)
        {
            return;
        }

        dt += Time.deltaTime;
        if (count < shotCount && dt >= shotRate)
        {
            Fire();
            count++;
            dt = 0f;
        }
        if (count >= shotCount && dt >= attackSpeed)
        {
            count = 0;
            dt = 0f;
        }
    }

    public void Fire()
    {
        var bullet = BulletManager.Instance.SpawnBullet(
            bulletInfo.prefab,
            transform.position,
            transform.rotation
        );

        bulletInfo.hitLayer = layer;
        bulletInfo.damage = damage;

        bullet.SetBulletInfo(bulletInfo);
        bullet.onHit.AddListener((body) => OnHit(body));
    }
}
