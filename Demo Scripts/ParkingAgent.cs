using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ParkingAgent : MonoBehaviour
{
    public NavMeshAgent agent;
    public Vector3 closestEmptySpot;

    public int queueIndex;

    public Player player;

    public bool stopped;
    public Vector3 initialPosition;
    public Quaternion initialRotation;

    public bool canGo;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (canGo)
        {
            UpdateDestination();
        }
        else
        {
            agent.destination = player.initialPositions[queueIndex - player.currentQueueIndex];
        }
    }

    private void UpdateDestination()
    {
        if (Vector3.Distance(transform.position, closestEmptySpot) > 1f)
        {
            closestEmptySpot = GetClosestPositionFromOrigin(transform.position, player.emptySpots);

            for (int i = 0; i < player.cars.Length; i++)
            {
                if (player.cars[i].transform.position.Equals(closestEmptySpot))
                {
                    player.closestEmptySpotIndex = i;
                }
            }

            agent.destination = closestEmptySpot;
        }
        else
        {
            if (!stopped)
            {
                player.emptySpots.Remove(closestEmptySpot);

                stopped = true;
            }
        }
    }

    Vector3 GetClosestPositionFromOrigin(Vector3 origin, List<Vector3> positions)
    {
        Vector3 tMin = Vector3.zero;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = origin;

        foreach (Vector3 t in positions)
        {
            float dist = Vector3.Distance(t, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    public void Reset()
    {
        gameObject.SetActive(false);

        canGo = false;
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        gameObject.SetActive(true);
    }

}
