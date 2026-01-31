using Unity.Jobs;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    private bool alive; // If a unit is alive or dead
    [SerializeField] private float gasResistMax; // Measured in seconds
    [SerializeField] private float gasResistVariation; // true max resistance is (gasResistMax +- gasResistVariation)
    private float gasResist;    // Remaining value of gas resistance meter in seconds
    private float gasRecoveryStart;
    private bool inGas;
    private bool inGasRecovery;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        if (inGas)
        {
            // Decrement resistance
            gasResist -= Time.deltaTime;
            if (gasResist <= 0)
            {
                this.Kill();
            }
        }
        else if (inGasRecovery)
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
    /// Called when a unit dies
    /// </summary>
    public void Kill()
    {
        this.alive = false;
        // Do more here
    }

    /// <summary>
    /// Called when a unit gets hit by something
    /// </summary>
    /// <param name="killChance">Chance of hit from 0.0 to 1.0</param>
    public void Hit(float killChance)
    {
        if (Random.value < killChance)
        {
            this.Kill();
        }
    }

    /// <summary>
    /// Returns whether a unit is in gas cloud
    /// </summary>
    /// <returns>Bool of if unit is in gas cloud</returns>
    public bool isGassed()
    {
        return inGas;
    }

    /// <summary>
    /// Called when unit enters gas cloud
    /// </summary>
    public void EnterGas()
    {
        inGas = true;
        inGasRecovery = false;
    }

    /// <summary>
    /// Called when unit exits gas cloud
    /// </summary>
    public void ExitGas()
    {
        inGas = false;
        inGasRecovery = true;
        gasRecoveryStart = Time.time;
    }
}
