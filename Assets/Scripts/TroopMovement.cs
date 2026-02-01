using UnityEngine;

public class TroopMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float accelerationRate;

    [SerializeField] private Vector3 defaultTargetPosition;
    [SerializeField] private float maxPositionOrderOffset;
    [SerializeField] private float targetPositionRadius;

    [SerializeField] private Vector3 currentPositionOrder;
    private Vector3 positionOrderOffset;
    [SerializeField] private Vector3 currentTargetPosition;

    [SerializeField] private float minRetreatAngle;

    private Vector3 velocity;
    private Vector3 targetVelocity;

    //private bool isMoving = false;

    private TrooperManager manager;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GetComponent<TrooperManager>();
    }



    // Update is called once per frame
    void Update()
    {
        UpdateTargetVelocity();
        Move();
    }



    private void UpdateTargetVelocity()
    {
        /*
        if (!isMoving && !HasReachedTargetPosition())
        {
            StartMoving();
        }

        if (isMoving) targetVelocity = moveSpeed * GetDirectionToTarget(currentTargetPosition);

        if (isMoving && HasReachedTargetPosition())
        {
            StopMoving();
        }
        */

        if (manager.GetCurrentState() == TrooperManager.TrooperState.IDLE && !HasReachedTargetPosition())
        {
            manager.SetCurrentState(TrooperManager.TrooperState.MOVING);
        }

        if (manager.GetCurrentState() == TrooperManager.TrooperState.MOVING ||
            manager.GetCurrentState() == TrooperManager.TrooperState.FLEEING)
        {
            targetVelocity = moveSpeed * GetDirectionToTarget(currentTargetPosition);
        }

        if ((manager.GetCurrentState() == TrooperManager.TrooperState.MOVING || manager.GetCurrentState() == TrooperManager.TrooperState.FLEEING)
            && HasReachedTargetPosition())
        {
            manager.SetCurrentState(TrooperManager.TrooperState.IDLE);
        }
    }



    private void Move()
    {
        /*
        if (!isMoving)
        {
            velocity = Vector3.zero;
            return;
        }
        */

        if (!(manager.GetCurrentState() == TrooperManager.TrooperState.MOVING ||
              manager.GetCurrentState() == TrooperManager.TrooperState.FLEEING))
        {
            velocity = Vector3.zero;
            return;
        }

        velocity += Vector3.Normalize(targetVelocity) * accelerationRate;
        if (velocity.magnitude > moveSpeed)
        {
            velocity = velocity.normalized * moveSpeed;
        }
        transform.position += (velocity * Time.deltaTime);
    }



    /*
    private void StopMoving()
    {
        isMoving = false;
    }



    private void StartMoving()
    {
        isMoving = true;
    }
    */



    private bool HasReachedTargetPosition()
    {
        return (Vector3.Distance(transform.position, currentTargetPosition) <= targetPositionRadius);
    }



    private Vector3 GetDirectionToTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        return Vector3.Normalize(dir);
    }



    public void GivePositionOrder(Vector3 position)
    {
        if (manager != null && manager.GetCurrentState() == TrooperManager.TrooperState.FIGHTING)
        {
            float angle = Vector3.Angle(GetDirectionToTarget(position), GetDirectionToTarget(manager.trooperCombat.GetTargetOpponent().transform.position));
            if (angle >= minRetreatAngle)
            {
                manager.SetCurrentState(TrooperManager.TrooperState.FLEEING);
            }
        }
        else if (manager != null && manager.GetCurrentState() == TrooperManager.TrooperState.FLEEING)
        {
            float angle = Vector3.Angle(GetDirectionToTarget(position), GetDirectionToTarget(currentPositionOrder));
            if (angle >= minRetreatAngle)
            {
                manager.SetCurrentState(TrooperManager.TrooperState.MOVING);
            }
        }


        currentPositionOrder = position;
        SetCurrentTargetPositionToOrder(true);
    }



    public void SetCurrentTargetPositionToOrder(bool setNewOffset)
    {
        if (setNewOffset) GeneratePositionOrderOffset();
        currentTargetPosition = currentPositionOrder + positionOrderOffset;
    }



    public void SetCurrentTargetPosition(Vector3 targetPosition, bool useOffset)
    {
        Vector3 offset = (useOffset) ? positionOrderOffset : Vector3.zero;
        currentTargetPosition = targetPosition + offset;
    }



    private void GeneratePositionOrderOffset()
    {
        positionOrderOffset.x = -maxPositionOrderOffset + (Random.value * 2f * maxPositionOrderOffset);
        positionOrderOffset.z = -maxPositionOrderOffset + (Random.value * 2f * maxPositionOrderOffset);
    }



    public Vector3 GetVelocity()
    {
        return velocity;
    }
}
