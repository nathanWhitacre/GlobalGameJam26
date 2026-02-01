using NUnit.Framework;
using System.Collections;
using UnityEngine;
using static TeamManager;

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
        if (ItemManager.instance.currentReinforcements <= 0) return;
        ItemManager.instance.currentReinforcements--;
        StartCoroutine(SpawnReinforcementsCoroutine());
    }



    public void SpawnReinforcementWave(bool ignoreReinforcementCost)
    {
        if (!ignoreReinforcementCost && ItemManager.instance.currentReinforcements <= 0) return;
        if (!ignoreReinforcementCost) ItemManager.instance.currentReinforcements--;
        StartCoroutine(SpawnReinforcementsCoroutine());
    }



    private IEnumerator SpawnReinforcementsCoroutine()
    {
        GameObject rightMostTrooper = TeamManager.instance.GetRightMostTrooper(TeamManager.Team.FRIENDLY);
        if (rightMostTrooper == null) yield break;
        for (int i = 0; i < reinforcementWaveCount; i++)
        {
            TeamManager.instance.SpawnTrooperWave(rightMostTrooper.transform.position + wavePosition, waveTrooperCount, TeamManager.Team.FRIENDLY);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
