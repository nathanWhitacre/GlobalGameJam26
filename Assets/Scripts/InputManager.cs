using UnityEngine;

public class InputManager : MonoBehaviour
{

    public static InputManager instance;

    private Vector3 latestOrdersFromHigh;



    private void Awake()
    {
        instance = this;
        latestOrdersFromHigh = new Vector3(0f, 0.5f, 0f);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            HandleFrag();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            HandleGasBomb();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            HandleReinforcement();
        }

        transform.position = GetLatestOrdersFromHigh();
    }



    private void HandleLeftClick()
    {

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            latestOrdersFromHigh = hit.point;
        } else
        {
            latestOrdersFromHigh = Vector3.zero;
        }
        TeamManager.instance.GivePositionOrders(latestOrdersFromHigh, TeamManager.Team.FRIENDLY);
    }



    private void HandleGasBomb()
    {
        Vector3 gasBombPosition = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            gasBombPosition = hit.point;
        }
        ItemManager.instance.SpawnGasBomb(gasBombPosition, TeamManager.Team.FRIENDLY);

        //TEMP MASK TEST
        foreach (GameObject trooper in TeamManager.instance.GetTrooperList(TeamManager.Team.FRIENDLY))
        {
            trooper.GetComponent<TrooperManager>().EquipMask(true);
        }
    }



    private void HandleFrag()
    {
        Vector3 fragPosition = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            fragPosition = hit.point;
        }
        ItemManager.instance.SpawnFrag(fragPosition, TeamManager.Team.FRIENDLY);
    }



    private void HandleReinforcement()
    {
        FriendlyTrooperSpawner.instance.SpawnReinforcementWave();
    }



    public Vector3 GetLatestOrdersFromHigh()
    {
        return latestOrdersFromHigh;
    }
}
