using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnenmiesController : MonoBehaviour
{
    [SerializeField] private List<Sprite> AllEnemies;
    [SerializeField] private List<SpawnPoint> SpawnPoints;
    [SerializeField] private GameObject EnemyPrefab;



    private int _maxEnemies = 3;
    [SerializeField] private List<SoulEnemy> _currentEnemies;
    [SerializeField] private Startup_Button_Selector Main_Selector;


    public GameObject A()
    {
        foreach(var X in _currentEnemies)
        {
            if (X.Return_ActionsPanel_State() == true)
                return X.Return_Active_Button();
        }

        return _currentEnemies[1].Return_Active_Button();
    }


    private void Awake()
    {
        ConfigureEnemiesController();
    }

    private void Start()
    {
        SpawnEnemies();
        Main_Selector.Force_Select(A());
    }

    private void OnEnable()
    {
        AttachListeners();
    }

    private void OnDisable()
    {
        DettachListeners();
    }

    private void AttachListeners()
    {
        GameEvents.EnemyKilled += EnemyKilled;
    }

    private void DettachListeners()
    {
        GameEvents.EnemyKilled -= EnemyKilled;
    }

    private void EnemyKilled(IEnemy enemy)
    {
        FreeSpawnPoint(enemy.GetEnemyPosition(),enemy);
        DestroyKilledEnemy(enemy.GetEnemyObject());
        StartCoroutine(SpawnEnemyViaCor());
    }

    private void SpawnEnemies()
    {
        while (_currentEnemies.Count < _maxEnemies)
        {
            SpawnEnemy();
        }
    }

    private IEnumerator SpawnEnemyViaCor()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (_currentEnemies.Count >= _maxEnemies)
        {
            Debug.LogError("Max Enemies reached! Kil some to spawn new");
            return;
        }

        int freeSpawnPointIndex = -1;
        for (int i = 0; i < SpawnPoints.Count; i++)
        {
            if (SpawnPoints[i].IsOccupied) continue;
            
            freeSpawnPointIndex = i;
            break;
        }

        if (freeSpawnPointIndex == -1) return;
        
        SpawnPoints[freeSpawnPointIndex].IsOccupied = true;
        SoulEnemy enemy = Instantiate(EnemyPrefab, SpawnPoints[freeSpawnPointIndex].Position.position, Quaternion.identity, transform).GetComponent<SoulEnemy>();
        int spriteIndex = Random.Range(0, AllEnemies.Count);
        enemy.SetupEnemy(AllEnemies[spriteIndex], SpawnPoints[freeSpawnPointIndex]);
        _currentEnemies.Add(enemy);
    }

    private void DestroyKilledEnemy(GameObject enemy)
    {
        Destroy(enemy);
    }

    private void FreeSpawnPoint(SpawnPoint spawnPoint, IEnemy enemy)
    {
        for (int i = 0; i < SpawnPoints.Count; i++)
        {
            if (spawnPoint != SpawnPoints[i]) continue;
            
            SpawnPoints[i].IsOccupied = false;
            _currentEnemies.Remove(enemy.GetEnemyObject().GetComponent<SoulEnemy>());
            break;
        }
    }

    private void ConfigureEnemiesController()
    {
        _maxEnemies = SpawnPoints != null ? SpawnPoints.Count : 3;
    }

}

[System.Serializable]
public class SpawnPoint
{
    public Transform Position;
    public bool IsOccupied;
}