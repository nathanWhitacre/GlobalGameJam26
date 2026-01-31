using Unity.Jobs;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    [SerializeField] private bool alive;
    [SerializeField] private float gasResistMax; // measured in seconds
    [SerializeField] private float gasResistVariation; // true max resistance is (gasResistMax +- gasResistVariation)
    private float gasResist;
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

    // Called when a unit dies
    public void Kill()
    {
        this.alive = false;
        // Do more here
    }

    // Called when a unit gets hit by something
    public void Hit(float killChance)
    {
        if (Random.value < killChance)
        {
            this.Kill();
        }
    }

    public bool isGassed()
    {
        return inGas;
    }

    public void EnterGas()
    {
        inGas = true;
        inGasRecovery = false;
    }

    public void ExitGas()
    {
        inGas = false;
        inGasRecovery = true;
        gasRecoveryStart = Time.time;
    }
}
