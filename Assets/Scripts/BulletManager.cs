using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private readonly Dictionary<int, List<Bullet>> bulletPools = new();
    private readonly Dictionary<int, List<GameObject>> hitPools = new();

    public static BulletManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Bullet SpawnBullet(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var bulletId = prefab.GetInstanceID();
        if (!bulletPools.ContainsKey(bulletId))
        {
            bulletPools.Add(bulletId, new List<Bullet>());
        }

        var bullet = bulletPools[bulletId].FirstOrDefault(b => !b.gameObject.activeInHierarchy);
        if (bullet == null)
        {
            bullet = Instantiate(prefab).GetComponent<Bullet>();
            bulletPools[bulletId].Add(bullet);

            // add listener to bullet.onDie event
            bullet.onDie.AddListener(() =>
            {
                bullet.gameObject.SetActive(false);
            });
        }

        // active bullet
        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.gameObject.SetActive(true);

        // reset bullet particle
        var particle = bullet.GetComponent<ParticleSystem>();
        if (particle)
        {
            particle.Stop();
            particle.Play();
        }

        return bullet;
    }

    public GameObject SpawnHitVFX(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var hitId = prefab.GetInstanceID();
        if (!hitPools.ContainsKey(hitId))
        {
            hitPools.Add(hitId, new List<GameObject>());
        }

        GameObject vfx = hitPools[hitId].FirstOrDefault(vfx => !vfx.activeInHierarchy);
        if (vfx == null)
        {
            vfx = Instantiate(prefab);
            hitPools[hitId].Add(vfx);
        }

        // play vfx
        vfx.transform.SetPositionAndRotation(position, rotation);
        vfx.SetActive(true);
        var particle = vfx.GetComponent<ParticleSystem>();
        particle.Stop();
        particle.Play();

        return vfx;
    }
}
