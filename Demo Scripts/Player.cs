using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public bl_Joystick joystick;

    public GameObject agentPrefab;

    public float delay = 5;
    public float countdown = 0;
    public int seconds = 0;

    public Vector3[] initialPositions;
    public ParkingAgent[] agents;
    public ParkingAgent[] queue;
    public ParkingAgent currentQueueAgent;
    public int currentQueueIndex;

    public GameObject[] cars;
    public List<Vector3> emptySpots;

    public Vector3 closestEmptySpot;
    public int closestEmptySpotIndex;

    public Text remainingDistance, status, queueIndex;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        agents = FindObjectsOfType<ParkingAgent>();
        cars = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach(ParkingAgent a in agents)
        {
            a.player = this;

            initialPositions[a.queueIndex] = a.initialPosition;

            queue[a.queueIndex] = a;
        }

        currentQueueAgent = queue[0];

        Refresh();

        StartCoroutine(UpdateStats());
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        UpdateQueue();
    }

    private void Movement()
    {
        float moveX = 0;
        float moveZ = 0;

        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            moveX = joystick.Horizontal / 5;
            moveZ = joystick.Vertical / 5;
        }

        Vector3 newPos = new Vector3(transform.position.x + moveX, transform.position.y, transform.position.z + moveZ);

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 20);

        transform.Translate(transform.forward * Input.GetAxis("Mouse ScrollWheel") * 20, Space.World);

    }

    public void Refresh()
    {
        currentQueueIndex = 0;
        countdown = 0;
        seconds = 0;

        for (int i=0; i<queue.Length; i++)
        {
            queue[i] = null;
        }

        foreach (ParkingAgent a in agents)
        {
            a.Reset();

            a.player = this;

            queue[a.queueIndex] = a;
        }

        currentQueueAgent = queue[0];

        emptySpots.Clear();

        foreach(GameObject car in cars)
        {
            if (!car.activeSelf)
            {
                car.SetActive(true);
            }
        }

        int[] randArray = new int[cars.Length/2];

        for(int i =0; i<randArray.Length; i++)
        {
            randArray[i] = Random.Range(0, cars.Length);
        }

        foreach(int r in randArray)
        {
            if (cars[r].activeSelf)
            {
                cars[r].SetActive(false);
            }
        }

        foreach(GameObject car in cars)
        {
            if (!car.activeSelf)
            {
                emptySpots.Add(car.transform.position);
            }
        }

        currentQueueAgent.canGo = true;
    }

    private void UpdateQueue()
    {
        if(seconds >= delay)
        {
            if(currentQueueIndex < queue.Length && queue[currentQueueIndex+1])
            {
                queue[currentQueueIndex] = null;

                currentQueueIndex++;

                currentQueueAgent = queue[currentQueueIndex];

                if (currentQueueAgent)
                {
                    currentQueueAgent.canGo = true;
                }

                countdown = 0;
                seconds = 0;
            }
        }

        countdown += Time.deltaTime;
        seconds = (int)(countdown % 60);
    }

    IEnumerator UpdateStats()
    {
        while (true)
        {
            if(Vector3.Distance(currentQueueAgent.transform.position, closestEmptySpot) > 1f)
            {
                if (currentQueueAgent)
                {
                    remainingDistance.text = Vector3.Distance(currentQueueAgent.transform.position, closestEmptySpot).ToString();
                    
                    if(Vector3.Distance(currentQueueAgent.transform.position, closestEmptySpot) < 2f)
                    {
                        status.text = "Reached";
                    }
                    else
                    {
                        status.text = "Travelling";
                    }

                    queueIndex.text = currentQueueIndex.ToString();
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
