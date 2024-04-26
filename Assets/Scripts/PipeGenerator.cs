using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeGenerator : MonoBehaviour
{
    [SerializeField]
    float spawnAreaSize;
    [SerializeField]
    float spawnIntervall;
    [SerializeField]
    float startDelay;
    [SerializeField]
    int turnFrequency;

    [SerializeField]
    GameObject pipePrefab;
    [SerializeField]
    GameObject jointPrefab;

    Dictionary<Vector3Int, GameObject> pipes;

    void Start()
    {
        // pipes keeps track of all pipe locations to avoid collisions
        pipes = new Dictionary<Vector3Int, GameObject>();

        // Generator starts randomly within area
        RandomizeRotation();
        RandomizePosition();

        //generating pipes in a loop
        InvokeRepeating("GeneratePipes", startDelay, spawnIntervall);
    }

    private void RandomizePosition()
    {
        float randomX = Random.Range(0, spawnAreaSize);
        float randomY = Random.Range(0, spawnAreaSize);
        float randomZ = Random.Range(0, spawnAreaSize);
        transform.position = new Vector3(Mathf.Round(randomX), Mathf.Round(randomY), Mathf.Round(randomZ));
    }

    private void RandomizeRotation()
    {
        int randomRotation = Random.Range(0, 2);
        Quaternion newRotation = Quaternion.Euler(Random.Range(0, 4) * 90f, Random.Range(0, 4) *90f, Random.Range(0, 4) * 90f);

        if (!(transform.rotation == newRotation))
            SpawnJoint();
        transform.rotation = newRotation;
    }
    bool IsTargetPositionWithinBounds(Vector3 targetPosition)
    {
        bool withinBoundsX = targetPosition.x >= 0 && targetPosition.x <= spawnAreaSize;
        bool withinBoundsY = targetPosition.y >= 0 && targetPosition.y <= spawnAreaSize;
        bool withinBoundsZ = targetPosition.z >= 0 && targetPosition.z <= spawnAreaSize;
        return withinBoundsX && withinBoundsY && withinBoundsZ;
    }

    private void GeneratePipes()
    {
        Vector3 targetPosition = transform.position + transform.up * 2;

        // if target position is within Spawn Area
        if (IsTargetPositionWithinBounds(targetPosition))
        {
            // if target position is devoid of pipes to avoid overlaps
            if (!pipes.ContainsKey(Vector3Int.RoundToInt(targetPosition)))
            {
                transform.position = targetPosition;
                // change rotation sometimes
                int randomRotation = Random.Range(turnFrequency, 11);
                if (randomRotation == 10)
                {
                    RandomizeRotation();
                }
                SpawnPipe();
                targetPosition = transform.position + (transform.up * 2);
            }
            // when target position is occupied by a pipe
            else
            {
                RandomizeRotation();
                targetPosition = transform.position + transform.up * 2;
            }
        }
        // when target position is not inside spawn area, the generator will be moved
        else
        {
            RandomizePosition();
            targetPosition = transform.position + transform.up * 2;
        }
    }

    private GameObject SpawnPipe()
    {
        // Create a Pipe based on generator position and rotation
        GameObject pipeInstance = Instantiate(pipePrefab, transform.position, transform.rotation);

        // Add the Pipe into the pipe dictionary
        Vector3 floatPosition = pipeInstance.transform.position;
        Vector3Int intPosition = Vector3Int.RoundToInt(floatPosition);
        pipes.Add(intPosition, pipeInstance);
        return pipeInstance;
    }

    private GameObject SpawnJoint()
    {
        // Create a Joint based on generator position and rotation
        GameObject jointInstance = Instantiate(jointPrefab, transform.position, transform.rotation);
        return jointInstance;
    }
}