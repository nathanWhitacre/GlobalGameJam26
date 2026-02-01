using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{

    [SerializeField] Vector3 enemySpawnPoint1;
    [SerializeField] Vector3 enemySpawnPoint2;
    [SerializeField] Vector3 enemySpawnPoint3;
    [SerializeField] float farSpawnOffset;

    [SerializeField] int prisonerCount;
    [SerializeField] Vector3 prisonerSpawnPoint;
    [SerializeField] float prisonerSpawnOffset;
    private List<GameObject> prisonerList;

    [SerializeField] float farSpawnDeactivationDistance;
    [SerializeField] float deactivationDistance;
    [SerializeField] float spawnNextDistance;

    [SerializeField] public bool hasPassedFarSpawn;
    [SerializeField] public bool isActive;
    [SerializeField] public bool hasSpawnedNextBase;

    private BaseManager manager;



    private void Awake()
    {
        enemySpawnPoint1 += transform.position;
        enemySpawnPoint2 += transform.position;
        enemySpawnPoint3 += transform.position;

        prisonerSpawnPoint += transform.position;
        prisonerList = new List<GameObject>();

        farSpawnDeactivationDistance += transform.position.x;
        deactivationDistance += transform.position.x;
        spawnNextDistance += transform.position.x;

        hasPassedFarSpawn = false;
        isActive = false;
        hasSpawnedNextBase = false;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*
        enemySpawnPoint1 += transform.position;
        enemySpawnPoint2 += transform.position;
        enemySpawnPoint3 += transform.position;

        prisonerSpawnPoint += transform.position;

        deactivationDistance += transform.position.x;
        spawnNextDistance += transform.position.x;
        */
    }



    public void StartBase(BaseManager manager)
    {
        this.manager = manager;
        isActive = true;
        prisonerList.Clear();
        for (int i = 0; i < prisonerCount; i++)
        {
            Vector3 positionOffset = Vector3.zero;
            positionOffset.x = -prisonerSpawnOffset + (Random.value * 2f * prisonerSpawnOffset);
            positionOffset.z = -prisonerSpawnOffset + (Random.value * 2f * prisonerSpawnOffset);
            GameObject prisoner = TeamManager.instance.SpawnTrooper(prisonerSpawnPoint + positionOffset, TeamManager.Team.FRIENDLY, true);
            prisonerList.Add(prisoner);
        }
    }



    // Update is called once per frame
    void Update()
    {
        CheckFarSpawnDeactivationDistance();
        CheckDeactivationDistance();
        CheckNewBaseDistance();
    }



    private void CheckFarSpawnDeactivationDistance()
    {
        GameObject rightMostTrooper = TeamManager.instance.GetRightMostTrooper(TeamManager.Team.FRIENDLY);
        if (isActive && rightMostTrooper != null && rightMostTrooper.transform.position.x >= farSpawnDeactivationDistance)
        {
            DeactivateFarSpawn();
        }
    }



    private void CheckDeactivationDistance()
    {
        GameObject rightMostTrooper = TeamManager.instance.GetRightMostTrooper(TeamManager.Team.FRIENDLY);
        if (isActive && rightMostTrooper != null && rightMostTrooper.transform.position.x >= deactivationDistance)
        {
            DeactivateBase();
        }
    }



    private void CheckNewBaseDistance()
    {
        GameObject rightMostTrooper = TeamManager.instance.GetRightMostTrooper(TeamManager.Team.FRIENDLY);
        if (!hasSpawnedNextBase && rightMostTrooper != null && rightMostTrooper.transform.position.x >= spawnNextDistance)
        {
            SpawnNewBase();
        }
    }



    public void DeactivateFarSpawn()
    {
        hasPassedFarSpawn = true;
    }



    public void DeactivateBase()
    {
        isActive = false;
        EnemyTrooperSpawner.instance.IncrementDifficulty();
    }



    public void SpawnNewBase()
    {
        manager.EstablishNewBase();
        hasSpawnedNextBase = true;
        //FriendlyTrooperSpawner.instance.SpawnReinforcementWave(true);
        LiberatePrisoners();
    }



    public void LiberatePrisoners()
    {
        foreach (GameObject prisoner in prisonerList)
        {
            prisoner.GetComponent<TrooperManager>().SetCurrentState(TrooperManager.TrooperState.IDLE, false, true);
            List<GameObject> targetTroopers = TeamManager.instance.GetTrooperList(TeamManager.Team.FRIENDLY);
            if (prisoner.GetComponent<TrooperManager>().GetCurrentState() != TrooperManager.TrooperState.DEAD) targetTroopers.Add(prisoner);
            prisoner.GetComponent<TroopMovement>().SetCurrentTargetPositionToOrder(true);
        }
        prisonerList.Clear();
    }



    public Vector3 GetWaveSpawnPosition()
    {
        int baseId = Mathf.Clamp(Mathf.CeilToInt(Random.value * 3f), 1, 3);
        Vector3 spawnPoint = enemySpawnPoint2;
        switch (baseId)
        {
            case 1:
                spawnPoint = enemySpawnPoint1;
                break;
            case 2:
                spawnPoint = enemySpawnPoint2;
                break;
            case 3:
                spawnPoint = enemySpawnPoint3;
                break;
            default:
                spawnPoint = enemySpawnPoint1;
                break;
        }
        if (!hasPassedFarSpawn)
        {
            spawnPoint.x += farSpawnOffset;
        }
        return spawnPoint;
    }
}
