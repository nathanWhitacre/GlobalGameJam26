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



    public Vector3 GetLatestOrdersFromHigh()
    {
        return latestOrdersFromHigh;
    }
}
