using UnityEngine;

public class BaseManager : MonoBehaviour
{

    public static BaseManager instance;

    [SerializeField] private GameObject enemyBasePrefab;
    [SerializeField] private float baseSeparationDistance;

    public EnemyBase currentBase;



    private void Awake()
    {
        instance = this;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EstablishNewBase();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public EnemyBase EstablishNewBase()
    {
        Vector3 newBasePosition = new Vector3(baseSeparationDistance, 0f, 0f);
        if (currentBase != null)
        {
            newBasePosition += currentBase.transform.position;
        }
        GameObject newBase = Instantiate(enemyBasePrefab, newBasePosition, Quaternion.identity);
        currentBase = newBase.GetComponent<EnemyBase>();
        currentBase.StartBase(this);
        return currentBase;
    }
}
