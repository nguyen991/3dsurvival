using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
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
        positionRange = new Vector2(cameraWidth + 2f, cameraWidth + 5f);

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
        Observable
            .Interval(System.TimeSpan.FromSeconds(1), UnityTimeProvider.PostLateUpdate)
            .Subscribe(_ => SpawnAndCleanupMonsters())
            .AddTo(this);
    }

    private void SpawnAndCleanupMonsters()
    {
        // remove monsters that are destroyed
        spawnedMonsters.RemoveAll(m => !m.gameObject.activeInHierarchy);

        // spawn a new monster
        if (spawnedMonsters.Count < maxMonsterCount)
        {
            spawnedMonsters.Add(SpawnMonster());
        }
    }

    public Monster SpawnMonster()
    {
        // get a monster from the pool
        var monsterId = Random.Range(0, monsters.Count);

        // calculate spawn position
        Vector3 position = Random.insideUnitCircle;
        while (position.x == 0f && position.y == 0f)
        {
            position = Random.insideUnitCircle;
        }
        position.Set(position.x, 0f, position.y);
        position =
            position.normalized * Random.Range(positionRange.x, positionRange.y)
            + character.transform.position;

        // get monster
        GameObject monsterGO = monsterPools[monsterId].Find(m => !m.activeInHierarchy);
        if (monsterGO == null)
        {
            monsterGO = Instantiate(monsters[monsterId]);
            monsterPools[monsterId].Add(monsterGO);
        }
        var monster = monsterGO.GetComponent<Monster>();

        // active monster
        monster.SetPositionAndRotation(
            position,
            Quaternion.LookRotation(character.transform.position - position)
        );

        // set the target of the monster to the character
        monster.target = character.transform;

        // active monster
        monsterGO.SetActive(true);

        return monster;
    }

    private void Update()
    {
        foreach (var monster in spawnedMonsters)
        {
            monster.DoUpdate();
        }
    }
}
