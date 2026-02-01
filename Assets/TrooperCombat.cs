using Unity.VisualScripting;
using UnityEngine;
using static TeamManager;

public class TrooperCombat : MonoBehaviour
{

    [SerializeField] private float aggroRadius;
    [SerializeField] private float maxFightRadius;
    [SerializeField] private float minFightRadius;

    [SerializeField] private GameObject targetOpponent;
    private float currentFightRadius;

    private float shootTimer;

    private TrooperManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GetComponent<TrooperManager>();
        targetOpponent = null;
        RandomizeFightRange();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTargetOpponent();
        UpdateFightingState();
        Fight();
    }

    private void UpdateTargetOpponent()
    {
        if (manager.GetCurrentState() == TrooperManager.TrooperState.FLEEING)
        {
            targetOpponent = null;
            return;
        }

        // check opponent staying or escaped radius
        if (targetOpponent != null && Vector3.Distance(transform.position, targetOpponent.transform.position) <= aggroRadius)
        {
            manager.trooperMovement.SetCurrentTargetPosition(targetOpponent.transform.position, false);
            return;
        }
        targetOpponent = null;

        // check opponent in radius
        GameObject closestOpponent = TeamManager.instance.GetClosestOpponentToTrooper(gameObject);
        if (closestOpponent != null && Vector3.Distance(transform.position, closestOpponent.transform.position) <= aggroRadius)
        {
            targetOpponent = closestOpponent;
            manager.trooperMovement.SetCurrentTargetPosition(targetOpponent.transform.position, false);
            RandomizeFightRange();
        }
        /*
        else
        {
            manager.trooperMovement.SetCurrentTargetPositionToOrder(false);
        }
        */

    }



    private void UpdateFightingState()
    {
        // Exit fighting
        if (manager.GetCurrentState() == TrooperManager.TrooperState.FIGHTING &&
            (targetOpponent == null || Vector3.Distance(transform.position, targetOpponent.transform.position) > maxFightRadius))
        {
            manager.trooperMovement.SetCurrentTargetPositionToOrder(true);
            manager.SetCurrentState(TrooperManager.TrooperState.IDLE);
            return;
        }
        
        // Enter fighting
        if (targetOpponent != null &&
            (manager.GetCurrentState() == TrooperManager.TrooperState.IDLE || manager.GetCurrentState() == TrooperManager.TrooperState.MOVING) &&
            Vector3.Distance(transform.position, targetOpponent.transform.position) <= currentFightRadius)
        {
            manager.SetCurrentState(TrooperManager.TrooperState.FIGHTING);
        }
    }



    private void Fight()
    {
        if (manager.GetCurrentState() != TrooperManager.TrooperState.FIGHTING)
        {
            shootTimer = currentFightRadius;
            return;
        }

        if (shootTimer > 0f)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                shootTimer = currentFightRadius;
                TeamManager.instance.GetTrooperList(TeamManager.instance.GetOpposingTeam(manager.GetTeam())).Remove(targetOpponent);
                Destroy(targetOpponent);
            }
        }
    }



    private void RandomizeFightRange()
    {
        float random = Random.value;
        currentFightRadius = Mathf.Lerp(minFightRadius, maxFightRadius, random);
    }



    public GameObject GetTargetOpponent()
    {
        return targetOpponent;
    }



    void OnDrawGizmos()
    {
        if (targetOpponent != null)
        {
            // Set the color of the gizmo (e.g., red)
            Gizmos.color = Color.red;

            // Draw a line from the current object's position to the target object's position
            Gizmos.DrawLine(transform.position, targetOpponent.transform.position);
        }
    }
}
