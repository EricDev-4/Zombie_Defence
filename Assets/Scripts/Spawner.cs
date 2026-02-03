
using System.Linq;
using UnityEngine;


public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoints;

    [SerializeField] private float[] _spawnLevel = { 3f, 2f, 1f };

    private void Awake()
    {
        spawnPoints = GetComponentsInChildren<Transform>().Where(t => t != transform).ToArray();
    }

    void Update()
    {
        if (GameManager.instance.timer > _spawnLevel[GameManager.instance.level])
        {
            GameManager.instance.timer = 0;
            Spawn();
        }

        if (GameManager.instance.levelTimer >= 30f)
        {
            GameManager.instance.levelTimer = 0;
            if(GameManager.instance.level < _spawnLevel.Length -1 )
                GameManager.instance.level++;
        }
    }

    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        //ToDO : 적 많아지면 random 또는 확률로 다양한 적 생성
    }
}
