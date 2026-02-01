using System.Collections;
using UnityEngine;

public class EnemyTrooperSpawner : MonoBehaviour
{

    public static EnemyTrooperSpawner instance;

    [SerializeField] private int minWavesCount;
    [SerializeField] private int maxWavesCount;
    [SerializeField] private int minWaveTrooperCount;
    [SerializeField] private int maxWaveTrooperCount;
    [SerializeField] private Vector3 wavePosition;
    [SerializeField] private float minWaveSpawnDelay;
    [SerializeField] private float maxWaveSpawnDelay;
    [SerializeField] private float maskedEnemySpawnChance;

    private float waveSpawnTimer;

    [SerializeField] public int difficultyLevel = 0;



    private void Awake()
    {
        instance = this;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waveSpawnTimer = Mathf.Lerp(minWaveSpawnDelay, maxWaveSpawnDelay, Random.value);
    }



    // Update is called once per frame
    void Update()
    {
        if (waveSpawnTimer > 0f)
        {
            waveSpawnTimer -= Time.deltaTime;
            if (waveSpawnTimer <= 0f)
            {
                waveSpawnTimer = Mathf.Lerp(minWaveSpawnDelay, maxWaveSpawnDelay, Random.value);
                if (BaseManager.instance.currentBase == null || !BaseManager.instance.currentBase.isActive) return;
                SpawnTrooperWaves();
            }
        }
    }



    public void SpawnTrooperWaves()
    {
        StartCoroutine(SpawnWavesCoroutine());
    }



    private IEnumerator SpawnWavesCoroutine()
    {
        int wavesCount = Mathf.RoundToInt(Mathf.Lerp(minWavesCount, maxWavesCount, Random.value));
        for (int i = 0; i < wavesCount; i++)
        {
            TeamManager.instance.SpawnTrooperWave(BaseManager.instance.currentBase.GetWaveSpawnPosition(),
                                                      Mathf.RoundToInt(Mathf.Lerp(minWaveTrooperCount, maxWaveTrooperCount, Random.value)),
                                                      TeamManager.Team.ENEMY, maskedEnemySpawnChance);
            yield return new WaitForSeconds(0.5f);
        }
    }



    public int IncrementDifficulty()
    {
        difficultyLevel++;
        if (difficultyLevel % 3 == 0)
        {
            if (Random.value <= 0.5f && minWavesCount < maxWavesCount)
            {
                minWavesCount++;
            }
            else
            {
                maxWavesCount++;
            }
        }

        if (Random.value <= 0.5f && minWaveTrooperCount < maxWaveTrooperCount)
        {
            minWaveTrooperCount++;
        }
        else
        {
            maxWaveTrooperCount++;
        }

        maskedEnemySpawnChance += 0.05f;

        ItemManager.instance.IncrementDifficulty();

        return difficultyLevel;
    }
}
