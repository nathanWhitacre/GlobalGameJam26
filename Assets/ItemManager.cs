using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    [SerializeField] private GameObject gasBombPrefab;
    [SerializeField] private float enemyGasBombSpawnDelay;
    [SerializeField] private Vector3 enemyGasBombSpawnBottomLeft;
    [SerializeField] private Vector3 enemyGasBombSpawnTopRight;
    private float enemyGasBombSpawnTimer;



    private void Awake()
    {
        instance = this;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyGasBombSpawnTimer = enemyGasBombSpawnDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyGasBombSpawnTimer > 0f)
        {
            enemyGasBombSpawnTimer -= Time.deltaTime;
            if (enemyGasBombSpawnTimer <= 0f)
            {
                enemyGasBombSpawnTimer = enemyGasBombSpawnDelay;
                SpawnGasBomb(GetEnemyGasBombSpawnPoint(), TeamManager.Team.ENEMY);
            }
        }
    }



    private Vector3 GetEnemyGasBombSpawnPoint()
    {
        float xPoint = Mathf.Lerp(enemyGasBombSpawnBottomLeft.x, enemyGasBombSpawnTopRight.x, Random.value);
        float zPoint = Mathf.Lerp(enemyGasBombSpawnBottomLeft.z, enemyGasBombSpawnTopRight.z, Random.value);
        Vector3 spawnPoint = new Vector3(xPoint, 0.5f, zPoint);
        return spawnPoint;
    }



    public void SpawnGasBomb(Vector3 position, TeamManager.Team team)
    {
        GameObject gasBomb = Instantiate(gasBombPrefab, position, Quaternion.identity);
        gasBomb.GetComponent<GasGrenade>().StartThrow(team);
    }
}
