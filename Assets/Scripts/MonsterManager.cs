using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField]
    private GameObject character;

    [SerializeField]
    private List<GameObject> monsters;

    [SerializeField]
    private int maxMonsterCount = 10;

    private Vector2 positionRange;
    private List<Monster> spawnedMonsters;
    private Dictionary<int, List<GameObject>> monsterPools;

    private void Awake()
    {
        // get camera width
        var camera = Camera.main;
        var cameraWidth = camera.orthographicSize * camera.aspect;
        positionRange = new Vector2(cameraWidth + 1f, cameraWidth + 5f);

        // initialize the list
        spawnedMonsters = new List<Monster>();

        // initialize the pool
        monsterPools = new Dictionary<int, List<GameObject>>();
        for (var i = 0; i < monsters.Count; i++)
        {
            monsterPools.Add(i, new List<GameObject>());
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnMonsterRoutine());
    }

    private IEnumerator SpawnMonsterRoutine()
    {
        while (enabled)
        {
            // remove monsters that are destroyed
            spawnedMonsters.RemoveAll(m => !m.gameObject.activeInHierarchy);

            // spawn a new monster
            if (spawnedMonsters.Count < maxMonsterCount)
            {
                spawnedMonsters.Add(SpawnMonster());
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public Monster SpawnMonster()
    {
        // get a monster from the pool
        var monsterId = Random.Range(0, monsters.Count);
        GameObject monster = monsterPools[monsterId].Find(m => !m.activeInHierarchy);
        if (monster == null)
        {
            monster = Instantiate(monsters[monsterId]);
            monsterPools[monsterId].Add(monster);
        }

        // set position
        var position =
            Random.insideUnitCircle.normalized * Random.Range(positionRange.x, positionRange.y);
        monster.transform.position =
            new Vector3(position.x, 0, position.y) + character.transform.position;

        // look at the character
        monster.transform.LookAt(character.transform);

        // set the target of the monster to the character
        monster.GetComponent<Monster>().target = character.transform;

        // active monster
        monster.SetActive(true);

        return monster.GetComponent<Monster>();
    }

    private void Update()
    {
        foreach (var monster in spawnedMonsters)
        {
            monster.DoUpdate();
        }
    }
}
