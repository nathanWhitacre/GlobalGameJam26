using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using static TeamManager;

public class TrooperCombat : MonoBehaviour
{

    [SerializeField] private float aggroRadius;
    [SerializeField] private float maxFightRadius;
    [SerializeField] private float minFightRadius;

    [SerializeField] private GameObject rifleHitVFX;
    [SerializeField] private float rifleHitChance;
    [SerializeField] private float minRifleWindup;
    [SerializeField] private float maxRifleWindup;

    [SerializeField] private GameObject targetOpponent;
    private float currentFightRadius;
    private float currentRifleWindup;

    private float shootTimer;

    private TrooperManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GetComponent<TrooperManager>();
        targetOpponent = null;
        RandomizeFightRange();
        RandomizeRifleWindupTime();
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

        if (targetOpponent == null || targetOpponent.GetComponent<TrooperManager>().GetCurrentState() == TrooperManager.TrooperState.DEAD ||
                                      targetOpponent.GetComponent<TrooperManager>().GetCurrentState() == TrooperManager.TrooperState.CAPTIVE)
        {
            targetOpponent = null;
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
            (targetOpponent == null ||
            targetOpponent.GetComponent<TrooperManager>().GetCurrentState() == TrooperManager.TrooperState.DEAD ||
            targetOpponent.GetComponent<TrooperManager>().GetCurrentState() == TrooperManager.TrooperState.CAPTIVE ||
            Vector3.Distance(transform.position, targetOpponent.transform.position) > maxFightRadius))
        {
            manager.trooperMovement.SetCurrentTargetPositionToOrder(true);
            manager.SetCurrentState(TrooperManager.TrooperState.IDLE);
            return;
        }
        
        // Enter fighting
        if (targetOpponent != null &&
            targetOpponent.GetComponent<TrooperManager>().GetCurrentState() != TrooperManager.TrooperState.DEAD &&
            targetOpponent.GetComponent<TrooperManager>().GetCurrentState() != TrooperManager.TrooperState.CAPTIVE &&
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
            shootTimer = currentRifleWindup;
            return;
        }

        if (shootTimer > 0f)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                RandomizeRifleWindupTime();
                shootTimer = currentRifleWindup;
                FireRifle();
            }
        }
    }



    private void FireRifle()
    {
        if (targetOpponent == null) return;
        if (manager.GetAnimator() != null) manager.GetAnimator().SetTrigger("Shoot");
        if (manager.rifleSFX != null) manager.rifleSFX.Play();
        bool hit = targetOpponent.GetComponent<TrooperManager>().trooperHealth.Hit(rifleHitChance);
        Vector3 hitPosition = targetOpponent.transform.position;
        if (!hit)
        {
            hitPosition += manager.GetDirectionToTarget(targetOpponent.transform.position) * (2f + Random.value * 8f);
            hitPosition += new Vector3((-4f + Random.value * 4f), 0f, (-4f + Random.value * 4f));
        }
        else
        {
            if (targetOpponent.GetComponent<TrooperManager>().HasMask() && !manager.HasMask())
            {
                manager.EquipMask(true);
            }
            if (Random.value <= 0.075f)
            {
                ItemManager.instance.currentFrags++;
                ItemManager.instance.currentFrags = Mathf.Min(ItemManager.instance.currentFrags, ItemManager.instance.maxFrags);
            }
            if (Random.value <= 0.1f)
            {
                ItemManager.instance.currentGasBombs++;
                ItemManager.instance.currentGasBombs = Mathf.Min(ItemManager.instance.currentGasBombs, ItemManager.instance.maxGasBombs);
            }
        }
        GameObject hitVFX = Instantiate(rifleHitVFX, hitPosition, Quaternion.identity);
        Destroy(hitVFX, 1f);
    }



    private void RandomizeFightRange()
    {
        float random = Random.value;
        currentFightRadius = Mathf.Lerp(minFightRadius, maxFightRadius, random);
    }

    private void RandomizeRifleWindupTime()
    {
        float random = Random.value;
        currentRifleWindup = Mathf.Lerp(minRifleWindup, maxRifleWindup, random);
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
