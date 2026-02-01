using System.Collections;
using UnityEngine;

public class FriendlyTrooperSpawner : MonoBehaviour
{

    public static FriendlyTrooperSpawner instance;

    [SerializeField] private int waveTrooperCount;
    [SerializeField] private int reinforcementWaveCount;
    [SerializeField] private Vector3 wavePosition;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TeamManager.instance.SpawnTrooperWave(wavePosition, waveTrooperCount, TeamManager.Team.FRIENDLY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void SpawnReinforcementWave()
    {
        StartCoroutine(SpawnReinforcementsCoroutine());
    }



    private IEnumerator SpawnReinforcementsCoroutine()
    {
        for (int i = 0; i < reinforcementWaveCount; i++)
        {
            TeamManager.instance.SpawnTrooperWave(wavePosition, waveTrooperCount, TeamManager.Team.FRIENDLY);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
