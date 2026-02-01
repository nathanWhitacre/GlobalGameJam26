using System.Collections;
using Unity.Jobs;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    private bool alive; // If a trooper is alive or dead
    [SerializeField] private float gasResistMax; // Measured in seconds
    [SerializeField] private float gasResistVariation; // true max resistance is (gasResistMax +- gasResistVariation)
    private float gasResist;    // Remaining value of gas resistance meter in seconds
    private float gasRecoveryStart;
    [SerializeField] private bool inGas;
    [SerializeField] private bool inGasRecovery;
    [SerializeField] private float deathTimeVariability;   // Necessary to add slight random offset in death time

    private TrooperManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GetComponent<TrooperManager>();
        alive = true;
        gasResistMax = gasResistMax + (Random.Range(-1, 1) * gasResistVariation);
        gasResist = gasResistMax;
        gasRecoveryStart = Time.time;
        inGas = false;
        inGasRecovery = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inGas && alive)
        {
            // Decrement resistance
            gasResist -= Time.deltaTime;
            if (gasResist <= 0)
            {
                this.Kill();
            }
        }
        else if (inGasRecovery && alive)
        {
            if (Time.time > gasRecoveryStart + gasResistMax)
            {
                // Reset gas resistance, exit recovery
                gasResist = gasResistMax;
                inGasRecovery = false;
            }
        }
    }

    /// <summary>
    /// Called when a trooper dies from any cause
    /// </summary>
    public void Kill()
    {
        this.alive = false;
        float deathDelay = Random.value * deathTimeVariability;
        StartCoroutine(DieWithDelay(deathDelay));
    }

    /// <summary>
    /// Performs actions to handle trooper death
    /// </summary>
    private void DeathHandling()
    {
        // Do something here
        TeamManager.instance.GetTrooperList(manager.GetTeam()).Remove(gameObject);
        manager.SetCurrentState(TrooperManager.TrooperState.DEAD);
        GetComponent<Collider>().enabled = false;
        manager.spriteRenderer.sortingOrder = 0;
        if (manager.deathSFX != null) manager.deathSFX.Play();
        if (manager.HasMask() && manager.GetTeam() == TeamManager.Team.FRIENDLY)
        {
            ItemManager.instance.currentMasks--;
        }
        //Destroy(gameObject);
    }

    /// <summary>
    /// Called when a trooper gets hit by something
    /// </summary>
    /// <param name="killChance">Chance of hit from 0.0 to 1.0</param>
    public bool Hit(float killChance)
    {
        if (Random.value < killChance)
        {
            this.Kill();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns whether a trooper is in gas cloud
    /// </summary>
    /// <returns>Bool of whether trooper is in gas cloud</returns>
    public bool isGassed()
    {
        return inGas;
    }

    /// <summary>
    /// Called when trooper enters gas cloud
    /// </summary>
    public void EnterGas()
    {
        inGas = true;
        inGasRecovery = false;
        manager.SetCurrentState(TrooperManager.TrooperState.FLEEING);
        manager.trooperMovement.SetCurrentTargetPositionToOrder(true);
    }

    /// <summary>
    /// Called when trooper exits gas cloud
    /// </summary>
    public void ExitGas()
    {
        inGas = false;
        inGasRecovery = true;
        gasRecoveryStart = Time.time;
    }

    // Calls DeathHandling() with a delay in seconds
    IEnumerator DieWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.DeathHandling();
        yield return null;
    }
}