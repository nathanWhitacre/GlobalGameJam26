using UnityEngine;

public class EnemyTrooperSpawner : MonoBehaviour
{

    public static EnemyTrooperSpawner instance;

    [SerializeField] private int waveTrooperCount;
    [SerializeField] private Vector3 wavePosition;
    [SerializeField] private float waveSpawnDelay;

    private float waveSpawnTimer;



    private void Awake()
    {
        instance = this;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waveSpawnTimer = waveSpawnDelay;
    }



    // Update is called once per frame
    void Update()
    {
        if (waveSpawnTimer > 0f)
        {
            waveSpawnTimer -= Time.deltaTime;
            if (waveSpawnTimer <= 0f)
            {
                waveSpawnTimer = waveSpawnDelay;
                TeamManager.instance.SpawnTrooperWave(wavePosition, waveTrooperCount, TeamManager.Team.ENEMY);
            }
        }
    }
}
