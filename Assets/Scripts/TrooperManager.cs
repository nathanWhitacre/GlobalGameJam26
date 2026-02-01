using UnityEngine;

public class TrooperManager : MonoBehaviour
{

    [SerializeField] private TeamManager.Team team;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [HideInInspector] public HealthScript trooperHealth;
    [HideInInspector] public TroopMovement trooperMovement;
    [HideInInspector] public TrooperCombat trooperCombat;

    [SerializeField] private TrooperState currentState;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trooperHealth = GetComponent<HealthScript>();
        trooperMovement = GetComponent<TroopMovement>();
        trooperCombat = GetComponent<TrooperCombat>();
        Vector3 positionOrder = (team == TeamManager.Team.FRIENDLY) ? (InputManager.instance.GetLatestOrdersFromHigh()) :
                                                                      (transform.position + new Vector3(-100f, 0f, 0f));
        trooperMovement.GivePositionOrder(positionOrder);
    }



    // Update is called once per frame
    void Update()
    {
        UpdateSprite();
    }



    private void UpdateSprite()
    {
        if (trooperMovement.GetVelocity().x < 0f)
        {
            spriteRenderer.flipX = true;
        }
        else if (trooperMovement.GetVelocity().x > 0f)
        {
            spriteRenderer.flipX = false;
        }

        if (currentState == TrooperState.IDLE)
        {
            spriteRenderer.flipX = (team == TeamManager.Team.FRIENDLY) ? false : true;
        }
    }



    public TeamManager.Team GetTeam()
    {
        return team;
    }



    public TrooperState GetCurrentState()
    {
        return currentState;
    }



    public TrooperState SetCurrentState(TrooperState state)
    {
        currentState = state;
        return currentState;
    }



    public Vector3 GetDirectionToTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        return Vector3.Normalize(dir);
    }



    public enum TrooperState
    {
        IDLE,
        MOVING,
        FIGHTING,
        CAPTIVE,
        DEAD,
        FLEEING
    }
}
