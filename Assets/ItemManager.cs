using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    [SerializeField] private GameObject gasBombPrefab;
    [SerializeField] private float enemyGasBombSpawnMinDelay;
    [SerializeField] private float enemyGasBombSpawnMaxDelay;
    [SerializeField] private float enemyGasBombSpawnChance;
    [SerializeField] private Vector3 enemyGasBombSpawnBottomLeft;
    [SerializeField] private Vector3 enemyGasBombSpawnTopRight;
    private float enemyGasBombSpawnTimer;



    [SerializeField] private GameObject fragPrefab;
    [SerializeField] private float enemyFragSpawnMinDelay;
    [SerializeField] private float enemyFragSpawnMaxDelay;
    [SerializeField] private float enemyFragSpawnChance;
    [SerializeField] private Vector3 enemyFragSpawnBottomLeft;
    [SerializeField] private Vector3 enemyFragSpawnTopRight;
    private float enemyFragSpawnTimer;



    private void Awake()
    {
        instance = this;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyGasBombSpawnTimer = Mathf.Lerp(enemyGasBombSpawnMinDelay, enemyGasBombSpawnMaxDelay, Random.value);
        enemyFragSpawnTimer = Mathf.Lerp(enemyFragSpawnMinDelay, enemyFragSpawnMaxDelay, Random.value);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyGasBombSpawnTimer > 0f)
        {
            enemyGasBombSpawnTimer -= Time.deltaTime;
            if (enemyGasBombSpawnTimer <= 0f)
            {
                enemyGasBombSpawnTimer = Mathf.Lerp(enemyGasBombSpawnMinDelay, enemyGasBombSpawnMaxDelay, Random.value);
                if (Random.value <= enemyGasBombSpawnChance)
                {
                    SpawnGasBomb(GetEnemyGasBombSpawnPoint(), TeamManager.Team.ENEMY);
                }
            }
        }

        if (enemyFragSpawnTimer > 0f)
        {
            enemyFragSpawnTimer -= Time.deltaTime;
            if (enemyFragSpawnTimer <= 0f)
            {
                enemyFragSpawnTimer = Mathf.Lerp(enemyFragSpawnMinDelay, enemyFragSpawnMaxDelay, Random.value);
                if (Random.value <= enemyFragSpawnChance)
                {
                    SpawnFrag(GetEnemyFragSpawnPoint(), TeamManager.Team.ENEMY);
                }
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



    private Vector3 GetEnemyFragSpawnPoint()
    {
        float xPoint = Mathf.Lerp(enemyFragSpawnBottomLeft.x, enemyFragSpawnTopRight.x, Random.value);
        float zPoint = Mathf.Lerp(enemyFragSpawnBottomLeft.z, enemyFragSpawnTopRight.z, Random.value);
        Vector3 spawnPoint = new Vector3(xPoint, 0.5f, zPoint);
        return spawnPoint;
    }



    public void SpawnGasBomb(Vector3 position, TeamManager.Team team)
    {
        GameObject gasBomb = Instantiate(gasBombPrefab, position, Quaternion.identity);
        gasBomb.GetComponent<GasGrenade>().StartThrow(team);
    }



    public void SpawnFrag(Vector3 position, TeamManager.Team team)
    {
        GameObject frag = Instantiate(fragPrefab, position, Quaternion.identity);
        frag.GetComponent<FragGrenade>().StartThrow(team);
    }
}
