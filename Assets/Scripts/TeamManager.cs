using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TeamManager : MonoBehaviour
{
    public static TeamManager instance;

    [SerializeField] private GameObject friendlyTrooperPrefab;
    [SerializeField] private GameObject enemyTrooperPrefab;

    [SerializeField] private List<GameObject> friendlyTroopers;
    [SerializeField] private List<GameObject> enemyTroopers;



    private void Awake()
    {
        instance = this;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SpawnTrooperWave(startingFriendlyTrooperPosition, startingFriendlyTrooperCount, Team.FRIENDLY);
    }



    // Update is called once per frame
    void Update()
    {
        if (GetTrooperList(Team.FRIENDLY).Count <= 0)
        {
            SceneManager.LoadScene(2);
        }
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



    public void SpawnTrooperWave(Vector3 position, int trooperCount, Team team, float maskChance)
    {
        for (int i = 0; i < trooperCount; i++)
        {
            Vector3 offsetPosition = position;
            offsetPosition.z = -i;
            SpawnTrooper(offsetPosition, team, maskChance);
        }
    }



    public GameObject SpawnTrooper(Vector3 position, Team team)
    {
        GameObject targetTrooperPrefab = GetTrooperPrefab(team);
        GameObject trooper = Instantiate(targetTrooperPrefab, position, Quaternion.identity);
        List<GameObject> targetTroopers = GetTrooperList(team);
        targetTroopers.Add(trooper);
        return trooper;
    }



    public GameObject SpawnTrooper(Vector3 position, Team team, bool isCaptive)
    {
        GameObject targetTrooperPrefab = GetTrooperPrefab(team);
        GameObject trooper = Instantiate(targetTrooperPrefab, position, Quaternion.identity);
        if (isCaptive) trooper.GetComponent<TrooperManager>().SetCurrentState(TrooperManager.TrooperState.CAPTIVE);
        //List<GameObject> targetTroopers = GetTrooperList(team);
        //targetTroopers.Add(trooper);
        return trooper;
    }



    public GameObject SpawnTrooper(Vector3 position, Team team, float maskChance)
    {
        GameObject targetTrooperPrefab = GetTrooperPrefab(team);
        GameObject trooper = Instantiate(targetTrooperPrefab, position, Quaternion.identity);
        if (Random.value <= maskChance)
        {
            trooper.GetComponent<TrooperManager>().EquipMask(true);
        }
        List<GameObject> targetTroopers = GetTrooperList(team);
        targetTroopers.Add(trooper);
        return trooper;
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



    public GameObject GetRightMostTrooper(Team team)
    {
        List<GameObject> troopers = GetTrooperList(team);
        if (troopers == null || troopers.Count <= 0) return null;

        GameObject rightMostTrooper = troopers[0];
        float highestX = rightMostTrooper.transform.position.x;
        foreach (GameObject trooper in troopers)
        {
            float trooperX = trooper.transform.position.x;
            if (trooperX > highestX)
            {
                highestX = trooperX;
                rightMostTrooper = trooper;
            }
        }
        return rightMostTrooper;
    }


    public enum Team
    {
        FRIENDLY,
        ENEMY
    }
}
