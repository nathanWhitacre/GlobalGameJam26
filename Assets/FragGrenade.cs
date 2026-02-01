using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FragGrenade : MonoBehaviour
{

    [SerializeField] private Vector3 throwInitialPosition;
    [SerializeField] private Vector3 throwVelocity;
    [SerializeField] private Vector3 throwAcceleration;
    [SerializeField] private float radius;
    [SerializeField] private float maxDelayTime;
    [SerializeField] private float minDelayTime;
    //[SerializeField] private float gasSpreadTime;
    [SerializeField] private float blastTime;
    [SerializeField] private float fadeOutTime;
    [SerializeField] private float hitChance;

    [SerializeField] private GameObject bombSprite;
    [SerializeField] private GameObject blastSprite;
    [SerializeField] private float blastSpriteScale;
    [SerializeField] private float blastSpriteAlpha;
    private SpriteRenderer blastSpriteRenderer;
    private SphereCollider blastCollider;
    private List<GameObject> hitEnemies;

    private float throwTimer;
    private float delayTimer;
    private float blastTimer;
    private float fadeOutTimer;

    [SerializeField] private FragState currentState;
    [SerializeField] private TeamManager.Team team;

    [SerializeField] public AudioSource blastSFX;



    private void Awake()
    {
        blastSpriteRenderer = blastSprite.GetComponent<SpriteRenderer>();
        blastCollider = GetComponent<SphereCollider>();
        blastCollider.enabled = false;
        hitEnemies = new List<GameObject>();

        SetRadius(0f, 0f);
        SetCloudAlpha(0f);

        currentState = FragState.WAITING;

        throwTimer = 0f;
        delayTimer = Mathf.Lerp(minDelayTime, maxDelayTime, Random.value);
        blastTimer = blastTime;
        fadeOutTimer = fadeOutTime;
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
        currentState = FragState.THROWING;
    }



    // Update is called once per frame
    void Update()
    {
        if (currentState == FragState.THROWING)
        {
            Throw();
            throwTimer += Time.deltaTime;
            if (transform.position.y <= 0.5f)
            {
                transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                currentState = FragState.DELAYED;
            }
        }

        if (currentState == FragState.DELAYED && delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0f)
            {
                delayTimer = maxDelayTime;
                blastCollider.enabled = true;
                currentState = FragState.BLASTING;
                if (blastSFX != null) blastSFX.Play();
                bombSprite.SetActive(false);
            }
        }

        if (currentState == FragState.BLASTING && blastTimer > 0f)
        {
            Blast();
            blastTimer -= Time.deltaTime;
            if (blastTimer <= 0f)
            {
                blastTimer = blastTime;
                SetRadius(radius, blastSpriteScale);
                currentState = FragState.FADING;
            }
        }

        if (currentState == FragState.FADING && fadeOutTimer > 0f)
        {
            FadeOut();
            fadeOutTimer -= Time.deltaTime;
            if (fadeOutTimer <= 0f)
            {
                fadeOutTimer = fadeOutTime;
                SetCloudAlpha(0f);
                blastCollider.enabled = false;
                currentState = FragState.INERT;
            }
        }
    }



    private void Throw()
    {
        transform.position = throwInitialPosition + (throwVelocity * throwTimer) + (throwAcceleration * Mathf.Pow(throwTimer, 2f));
    }



    private void Blast()
    {
        float blastProgress = (blastTime - blastTimer) / blastTime;
        float currentRadius = Mathf.Lerp(0f, radius, blastProgress);
        float currentSpriteScale = Mathf.Lerp(0f, blastSpriteScale, blastProgress);
        SetRadius(currentRadius, currentSpriteScale);
        float currentAlpha = Mathf.Lerp(blastSpriteAlpha, blastSpriteAlpha, blastProgress);
        SetCloudAlpha(currentAlpha);
    }



    private void FadeOut()
    {
        float fadeProgress = (fadeOutTime - fadeOutTimer) / fadeOutTime;
        float currentAlpha = Mathf.Lerp(0f, blastSpriteAlpha, (1f - fadeProgress));
        SetCloudAlpha(currentAlpha);
        float currentRadius = Mathf.Lerp(0f, radius, (1f - fadeProgress));
        float currentSpriteScale = Mathf.Lerp(blastSpriteScale, (blastSpriteScale * 1.2f), fadeProgress);
        SetRadius(currentRadius, currentSpriteScale);
    }



    private void SetRadius(float radius, float spriteScale)
    {
        blastCollider.radius = radius;
        blastSprite.transform.localScale = new Vector3(spriteScale, spriteScale, spriteScale);
    }



    private void SetCloudAlpha(float alpha)
    {
        blastSpriteRenderer.color = new Color(blastSpriteRenderer.color.r, blastSpriteRenderer.color.g, blastSpriteRenderer.color.b, alpha);
    }



    private void OnTriggerEnter(Collider other)
    {
        //if (!(currentState == GasState.SPREADING || currentState == GasState.LINGERING)) return;
        TrooperManager trooperManager = other.gameObject.GetComponent<TrooperManager>();
        if (trooperManager == null) return;

        //if (trooperManager.trooperHealth.isGassed() || trooperManager.HasMask()) return;
        //trooperManager.trooperHealth.EnterGas();
        if (hitEnemies.Contains(other.gameObject)) return;
        hitEnemies.Add(other.gameObject);
        trooperManager.trooperHealth.Hit(hitChance);
    }



    /*
    private void OnTriggerExit(Collider other)
    {
        TrooperManager trooperManager = other.gameObject.GetComponent<TrooperManager>();
        if (trooperManager == null) return;

        if (!trooperManager.trooperHealth.isGassed()) return;
        trooperManager.trooperHealth.ExitGas();
    }
    */



    public enum FragState
    {
        WAITING,
        THROWING,
        DELAYED,
        BLASTING,
        FADING,
        INERT
    }
}
