
using UnityEngine;

public class GasGrenade : MonoBehaviour
{

    [SerializeField] private Vector3 throwInitialPosition;
    [SerializeField] private Vector3 throwVelocity;
    [SerializeField] private Vector3 throwAcceleration;
    [SerializeField] private float radius;
    [SerializeField] private float gasDelayTime;
    [SerializeField] private float gasSpreadTime;
    [SerializeField] private float gasLingerTime;
    [SerializeField] private float gasFadeOutTime;

    [SerializeField] private GameObject bombSprite;
    [SerializeField] private GameObject gasSprite;
    [SerializeField] private float lingeringGasSpriteScale;
    [SerializeField] private float lingeringGasCloudAlpha;
    private SpriteRenderer gasSpriteRenderer;
    private SphereCollider gasCollider;

    private float throwTimer;
    private float delayTimer;
    private float spreadTimer;
    private float lingerTimer;
    private float fadeOutTimer;

    [SerializeField] private GasState currentState;
    [SerializeField] private TeamManager.Team team;

    [SerializeField] public AudioSource gasSFX;



    private void Awake()
    {
        gasSpriteRenderer = gasSprite.GetComponent<SpriteRenderer>();
        gasCollider = GetComponent<SphereCollider>();
        gasCollider.enabled = false;

        SetRadius(0f, 0f);
        SetCloudAlpha(0f);

        currentState = GasState.WAITING;

        throwTimer = 0f;
        delayTimer = gasDelayTime;
        spreadTimer = gasSpreadTime;
        lingerTimer = gasLingerTime;
        fadeOutTimer = gasFadeOutTime;
    }



    public void StartThrow(TeamManager.Team team)
    {
        this.team = team;
        if (team == TeamManager.Team.ENEMY)
        {
            throwInitialPosition.x *= -1f;
            throwVelocity.x *= -1f;
            throwAcceleration.x *= -1f;
        }
        throwInitialPosition += transform.position;
        currentState = GasState.THROWING;
    }



    // Update is called once per frame
    void Update()
    {
        if (currentState == GasState.THROWING)
        {
            Throw();
            throwTimer += Time.deltaTime;
            if (transform.position.y <= 0.5f)
            {
                transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                currentState = GasState.DELAYED;
                if (gasSFX != null) gasSFX.Play();
            }
        }

        if (currentState == GasState.DELAYED && delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0f)
            {
                delayTimer = gasDelayTime;
                gasCollider.enabled = true;
                currentState = GasState.SPREADING;
            }
        }

        if (currentState == GasState.SPREADING && spreadTimer > 0f)
        {
            Spread();
            spreadTimer -= Time.deltaTime;
            if (spreadTimer <= 0f)
            {
                spreadTimer = gasSpreadTime;
                SetRadius(radius, lingeringGasSpriteScale);
                currentState = GasState.LINGERING;
            }
        }

        if (currentState == GasState.LINGERING && lingerTimer > 0f)
        {
            Linger();
            lingerTimer -= Time.deltaTime;
            if (lingerTimer <= 0f)
            {
                lingerTimer = gasLingerTime;
                currentState = GasState.FADING;
            }
        }

        if (currentState == GasState.FADING && fadeOutTimer > 0f)
        {
            FadeOut();
            fadeOutTimer -= Time.deltaTime;
            if (fadeOutTimer <= 0f)
            {
                fadeOutTimer = gasFadeOutTime;
                SetCloudAlpha(0f);
                gasCollider.enabled = false;
                currentState = GasState.INERT;
            }
        }
    }



    private void Throw()
    {
        transform.position = throwInitialPosition + (throwVelocity * throwTimer) + (throwAcceleration * Mathf.Pow(throwTimer, 2f));
    }



    private void Spread()
    {
        float spreadProgress = (gasSpreadTime - spreadTimer) / gasSpreadTime;
        float currentRadius = Mathf.Lerp(0f, radius, spreadProgress);
        float currentSpriteScale = Mathf.Lerp(0f, lingeringGasSpriteScale, spreadProgress);
        SetRadius(currentRadius, currentSpriteScale);
        float currentAlpha = Mathf.Lerp(0f, lingeringGasCloudAlpha, spreadProgress);
        SetCloudAlpha(currentAlpha);
    }



    private void Linger()
    {
        float lingerProgress = (gasLingerTime - lingerTimer) / gasLingerTime;
        float currentRadius = Mathf.Lerp(radius, (radius * 1.2f), lingerProgress);
        float currentSpriteScale = Mathf.Lerp(lingeringGasSpriteScale, (lingeringGasSpriteScale * 1.2f), lingerProgress);
        SetRadius(currentRadius, currentSpriteScale);
        float currentAlpha = Mathf.Lerp((lingeringGasCloudAlpha * 0.75f), lingeringGasCloudAlpha, (1f - lingerProgress));
        SetCloudAlpha(currentAlpha);
    }



    private void FadeOut()
    {
        float fadeProgress = (gasFadeOutTime - fadeOutTimer) / gasFadeOutTime;
        float currentAlpha = Mathf.Lerp(0f, (lingeringGasCloudAlpha * 0.75f), (1f - fadeProgress));
        SetCloudAlpha(currentAlpha);
        float currentRadius = Mathf.Lerp(0f, (radius * 1.2f), (1f - fadeProgress));
        float currentSpriteScale = Mathf.Lerp((lingeringGasSpriteScale * 1.2f), (lingeringGasSpriteScale * 1.3f), fadeProgress);
        SetRadius(currentRadius, currentSpriteScale);
    }



    private void SetRadius(float radius, float spriteScale)
    {
        gasCollider.radius = radius;
        gasSprite.transform.localScale = new Vector3(spriteScale, spriteScale, spriteScale);
    }



    private void SetCloudAlpha(float alpha)
    {
        gasSpriteRenderer.color = new Color(gasSpriteRenderer.color.r, gasSpriteRenderer.color.g, gasSpriteRenderer.color.b, alpha);
    }



    private void OnTriggerStay(Collider other)
    {
        //if (!(currentState == GasState.SPREADING || currentState == GasState.LINGERING)) return;
        TrooperManager trooperManager = other.gameObject.GetComponent<TrooperManager>();
        if (trooperManager == null) return;

        if (trooperManager.trooperHealth.isGassed() || trooperManager.HasMask()) return;
        trooperManager.trooperHealth.EnterGas();
    }



    private void OnTriggerExit(Collider other)
    {
        TrooperManager trooperManager = other.gameObject.GetComponent<TrooperManager>();
        if (trooperManager == null) return;

        if (!trooperManager.trooperHealth.isGassed()) return;
        trooperManager.trooperHealth.ExitGas();
    }



    public enum GasState
    {
        WAITING,
        THROWING,
        DELAYED,
        SPREADING,
        LINGERING,
        FADING,
        INERT
    }
}
