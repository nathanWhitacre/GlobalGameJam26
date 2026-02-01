using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class TeamManager : MonoBehaviour
{
    public static TeamManager instance;

    [SerializeField] private GameObject friendlyTrooperPrefab;
    [SerializeField] private GameObject enemyTrooperPrefab;

    [SerializeField] private int startingFriendlyTrooperCount;
    [SerializeField] private Vector3 startingFriendlyTrooperPosition;

    [SerializeField] private List<GameObject> friendlyTroopers;
    [SerializeField] private List<GameObject> enemyTroopers;



    private void Awake()
    {
        instance = this;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnTrooperWave(startingFriendlyTrooperPosition, startingFriendlyTrooperCount, Team.FRIENDLY);
    }



    // Update is called once per frame
    void Update()
    {
        
    }



    public void GivePositionOrders(Vector3 position, Team team)
    {
        List<GameObject> targetTroopers = GetTrooperList(team);
        foreach (GameObject trooper in targetTroopers)
        {
            if (trooper.GetComponent<TroopMovement>() != null) trooper.GetComponent<TroopMovement>().GivePositionOrder(position);
        }
    }


    public void SpawnTrooperWave(Vector3 position, int trooperCount, Team team)
    {
        for (int i = 0; i < trooperCount; i++)
        {
            Vector3 offsetPosition = position;
            offsetPosition.z = -i;
            SpawnTrooper(offsetPosition, team);
        }
    }



    public void SpawnTrooper(Vector3 position, Team team)
    {
        GameObject targetTrooperPrefab = GetTrooperPrefab(team);
        GameObject trooper = Instantiate(targetTrooperPrefab, position, Quaternion.identity);
        List<GameObject> targetTroopers = GetTrooperList(team);
        targetTroopers.Add(trooper);
    }



    public List<GameObject> GetTrooperList(Team team)
    {
        return (team == Team.FRIENDLY) ? friendlyTroopers : enemyTroopers;
    }

    public List<GameObject> GetOpposingTrooperList(Team team)
    {
        return (team == Team.ENEMY) ? friendlyTroopers : enemyTroopers;
    }



    public Team GetOpposingTeam(Team team)
    {
        return (team == Team.FRIENDLY) ? Team.ENEMY : Team.FRIENDLY;
    }



    public GameObject GetTrooperPrefab(Team team)
    {
        return (team == Team.FRIENDLY) ? friendlyTrooperPrefab : enemyTrooperPrefab;
    }



    public GameObject GetClosestOpponentToTrooper(GameObject trooper)
    {
        TrooperManager trooperManager = trooper.GetComponent<TrooperManager>();
        List<GameObject> opposingTroopers = GetOpposingTrooperList(trooperManager.GetTeam());

        if (opposingTroopers == null || opposingTroopers.Count <= 0) return null;

        GameObject closestOpponent = opposingTroopers[0];
        float lowestDistance = Vector3.Distance(trooper.transform.position, closestOpponent.transform.position);
        foreach (GameObject opposingTrooper in opposingTroopers)
        {
            float distance = Vector3.Distance(trooper.transform.position, opposingTrooper.transform.position);
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                closestOpponent = opposingTrooper;
            }
        }
        return closestOpponent;
    }


    public enum Team
    {
        FRIENDLY,
        ENEMY
    }
}
