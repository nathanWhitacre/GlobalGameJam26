using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{

    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float accelerationRate;
    [SerializeField] private float decelerationRate;
    [SerializeField] private float targetPositionRadius;

    private Vector3 currentTargetPosition;
    private Vector3 velocity;
    private Vector3 targetVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTargetVelocity();
        Move();
    }



    private void UpdateTargetVelocity()
    {
        List<GameObject> friendlyList = TeamManager.instance.GetTrooperList(TeamManager.Team.FRIENDLY);
        if (friendlyList == null || friendlyList.Count <= 0) return;
        Vector3 targetPosition = TeamManager.instance.GetRightMostTrooper(TeamManager.Team.FRIENDLY).transform.position;
        targetPosition.z = 0f;
        currentTargetPosition = targetPosition + targetOffset;

        targetVelocity = moveSpeed * GetDirectionToTarget(currentTargetPosition);
    }

    

    private void Move()
    {
        if (HasReachedTargetPosition())
        {
            /*
            velocity -= Vector3.Normalize(targetVelocity) * decelerationRate;
            if (velocity.magnitude <= 10f)
            {
                velocity = Vector3.zero;
            }
            transform.position += (velocity * Time.deltaTime);
            return;
            */
            velocity = Vector3.zero;
            velocity *= decelerationRate;
            if (velocity.magnitude <= 1f)
            {
                velocity = Vector3.zero;
            }
            transform.position += velocity * Time.deltaTime;
        }

        velocity += Vector3.Normalize(targetVelocity) * accelerationRate;
        if (velocity.magnitude > moveSpeed)
        {
            velocity = velocity.normalized * moveSpeed;
        }
        transform.position += (velocity * Time.deltaTime);
    }



    public Vector3 GetDirectionToTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        return Vector3.Normalize(dir);
    }



    private bool HasReachedTargetPosition()
    {
        return (Vector3.Distance(transform.position, currentTargetPosition) <= targetPositionRadius);
    }
}
