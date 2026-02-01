using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class TrooperManager : MonoBehaviour
{

    [SerializeField] private TeamManager.Team team;
    [SerializeField] private bool hasMask;

    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] private Animator spriteAnimator;
    [HideInInspector] public HealthScript trooperHealth;
    [HideInInspector] public TroopMovement trooperMovement;
    [HideInInspector] public TrooperCombat trooperCombat;

    [SerializeField] private AnimatorController defaultController;
    [SerializeField] private AnimatorController maskController;

    [SerializeField] private TrooperState currentState;

    [SerializeField] public AudioSource rifleSFX;
    [SerializeField] public AudioSource footstepSFX;
    [SerializeField] public AudioSource deathSFX;



    private void Awake()
    {
        trooperHealth = GetComponent<HealthScript>();
        trooperMovement = GetComponent<TroopMovement>();
        trooperCombat = GetComponent<TrooperCombat>();
        EquipMask(false);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 positionOrder = (team == TeamManager.Team.FRIENDLY) ? (InputManager.instance.GetLatestOrdersFromHigh()) :
                                                                      (transform.position + new Vector3(-100f, 0f, 0f));
        trooperMovement.GivePositionOrder(positionOrder);
    }



    // Update is called once per frame
    void Update()
    {
        UpdateSprite();
        UpdateAnimator();
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

        if (currentState == TrooperState.FIGHTING && trooperCombat.GetTargetOpponent() != null)
        {
            spriteRenderer.flipX = (GetDirectionToTarget(trooperCombat.GetTargetOpponent().transform.position).x < 0f) ? true : false;
        }

        if (currentState == TrooperState.IDLE)
        {
            spriteRenderer.flipX = (team == TeamManager.Team.FRIENDLY) ? false : true;
        }
    }



    private void UpdateAnimator()
    {
        if (spriteAnimator == null) return;

        spriteAnimator.SetBool("IDLE", false);
        spriteAnimator.SetBool("MOVING", false);
        spriteAnimator.SetBool("FIGHTING", false);
        spriteAnimator.SetBool("CAPTIVE", false);
        spriteAnimator.SetBool("DEAD", false);
        spriteAnimator.SetBool("FLEEING", false);
        switch (currentState)
        {
            case TrooperState.IDLE:
                spriteAnimator.SetBool("IDLE", true);
                break;

            case TrooperState.MOVING:
                spriteAnimator.SetBool("MOVING", true);
                break;

            case TrooperState.FIGHTING:
                spriteAnimator.SetBool("FIGHTING", true);
                break;

            case TrooperState.CAPTIVE:
                spriteAnimator.SetBool("CAPTIVE", true);
                break;

            case TrooperState.DEAD:
                spriteAnimator.SetBool("DEAD", true);
                break;

            case TrooperState.FLEEING:
                spriteAnimator.SetBool("FLEEING", true);
                break;

            default:
                spriteAnimator.SetBool("IDLE", true);
                break;
        }
    }



    public Animator GetAnimator()
    {
        return spriteAnimator;
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
        if (currentState == TrooperState.DEAD) return currentState;
        if (currentState == TrooperState.CAPTIVE && state != TrooperState.DEAD) return currentState;
        currentState = state;
        return currentState;
    }



    public TrooperState SetCurrentState(TrooperState state, bool ignoreDead, bool ignoreCaptive)
    {
        if (currentState == TrooperState.DEAD && !ignoreDead) return currentState;
        if (currentState == TrooperState.CAPTIVE && !ignoreCaptive) return currentState;
        currentState = state;
        return currentState;
    }



    public Vector3 GetDirectionToTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        return Vector3.Normalize(dir);
    }



    public bool HasMask()
    {
        return hasMask;
    }



    public void EquipMask(bool equip)
    {
        if (team == TeamManager.Team.FRIENDLY)
        {
            if (equip)
            {
                ItemManager.instance.currentMasks++;
            }
            else
            {
                //ItemManager.instance.currentMasks--;
            }
        }
        hasMask = equip;
        if (spriteAnimator != null) spriteAnimator.runtimeAnimatorController = (hasMask) ? maskController : defaultController;
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
