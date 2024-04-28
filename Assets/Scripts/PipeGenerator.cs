using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// #############################################################################################################
// ### This script contains the logic to generate pipes as seen on the windows 98 3D Pipe Screensaver.       ###
// ### Attach this script to an empty gameobject and the gameobject will wander around a cubic area          ###
// ### to fill the space with colorful pipes!                                                                ###
// ### Spawn area and other spawn behaviour can be adjusted within the editor.                               ###
// ### If you plan to use differernt pipe assets, keep the pipe size below 0.5 units, rotate horizontally    ###
// ### and set the pipe's pivot point to -0.5 to avoid orientation or clipping issues.                       ###
// ### _____________________________________________________________________________________________________ ###
// ### How it works TL;DR:                                                                                   ###
// ### - Generator tries to find a valid location, moves to location and spawns a pipe behind the generator  ###
// ### - If a change in direction is detected, a pipe joint will also be placed.                             ###
// ### - pipe color will also change whenever a change in direction is detected, pipe Color is randomized.   ###
// #############################################################################################################

public class PipeGenerator : MonoBehaviour
{
    [SerializeField]
    bool randomizeGeneratorAtStart;
    [SerializeField]
    float spawnAreaSize;
    [SerializeField]
    float spawnIntervall;
    [SerializeField]
    float startDelay;

    [SerializeField]
    GameObject pipePrefab;
    [SerializeField]
    GameObject jointPrefab;

    Dictionary<Vector3Int, GameObject> pipes;

    void Start()
    {
        // pipes keeps track of all pipe locations to avoid collisions
        pipes = new Dictionary<Vector3Int, GameObject>();

        if (randomizeGeneratorAtStart)
        {
            RandomizeGenerator();
        }

        // starts the pipe generation loop (pipe spawn parameters can be set in editor)
        InvokeRepeating("GeneratePipes", startDelay, spawnIntervall);
    }

    // the generation loop to spawn pipes
    private void GeneratePipes()
    {
        PositionGeneratorAtValidLocation();
        SpawnPipe();
    }

    // generator gets a random start position and rotation
    private void RandomizeGenerator()
    {
        // generator gets a random rotation
        float randomRotationX = Random.Range(0, 4) * 90;
        float randomRotationY = Random.Range(0, 4) * 90;
        float randomRotationZ = Random.Range(0, 4) * 90;

        Quaternion randomRotation;
        randomRotation = Quaternion.Euler(randomRotationX, randomRotationY, randomRotationZ);
        transform.rotation = randomRotation;

        // generator gets a random position within the spawn area that's not occupied by a pipe (attempts it 1000 times)
        Vector3 randomPosition = new Vector3();
        bool isPositionValid = false;
        int attempts = 1000;
        do
        {
            float randomPositionX = Random.Range(0, spawnAreaSize);
            float randomPositionY = Random.Range(0, spawnAreaSize);
            float randomPositionZ = Random.Range(0, spawnAreaSize);

            randomPosition = new Vector3(Mathf.Round(randomPositionX), Mathf.Round(randomPositionY), Mathf.Round(randomPositionZ));
            attempts--;
            isPositionValid = IsPositionFreeOfPipes(randomPosition) && IsPositionFreeOfPipes(randomPosition);
        }
        while (!isPositionValid && (attempts != 0));

        // if all attempts are used up, exits editor play mode
        if (attempts == 0)
        {
            EditorApplication.isPlaying = false;
            Debug.Log("Could not generate a random generator location");
        }
        // when valid random position, the generator is set to it and spawns an innitial joint
        else
        {
            transform.position = randomPosition;
        }
    }

    // makes sure the target destination is within area and not occupied by pipe
    private bool IsPositionWithinBounds(Vector3 position)
    {
        bool isWithinBoundsX = position.x >= 0 && position.x <= spawnAreaSize;
        bool isWithinBoundsY = position.y >= 0 && position.y <= spawnAreaSize;
        bool isWithinBoundsZ = position.z >= 0 && position.z <= spawnAreaSize;

        bool isWithinBounds = isWithinBoundsX && isWithinBoundsY && isWithinBoundsZ;
        return isWithinBounds;
    }

    // makes sure the target destination is not occupied by a pipe
    private bool IsPositionFreeOfPipes(Vector3 position)
    {
        bool isFreeOfPipes = !pipes.ContainsKey(Vector3Int.RoundToInt(position));
        return isFreeOfPipes;
    }

    private void PositionGeneratorAtValidLocation()
    {
        // by determining if the generator should turn directions or continue forward
        // the method tries to set a valid position

        bool shouldTurn = (Random.Range(0, 3) == 0);
        bool generatorTurned = false;
        bool generatorMoved = false;

        // if generator turns
        if (shouldTurn)
        {
            generatorTurned = TryToTurnGenerator();
        }
        // if turning is impossible or generator should go forward, generator tries to go forward
        if (!generatorTurned)
        {
            generatorMoved = TryToMoveGenerator();

            // if generator could not be moved but should be moved, tries to turn instead
            if (!generatorMoved && !shouldTurn)
            {
                generatorTurned = TryToTurnGenerator();
            }

            // if generator can't turn or move at all, the position will be randomized within spawn area to escape dead end.
            if (!generatorTurned && !generatorMoved)
            {
                Debug.Log("Generator got into dead end, randomizing new position.");
                RandomizeGenerator();
            }
        }
    }

    private bool TryToTurnGenerator()
    {
        // tries to find a valid turn direction by validating the corresponding destination position of the turn direction
        // uses generator's local space, negated elements in the list mean the opposite direction
        // returns bool with success state
        List<Vector3> possibleDirections = new List<Vector3> { transform.right, -transform.right, transform.up, -transform.up };
        do
        {
            int randomTurnIndex = Random.Range(0, possibleDirections.Count);
            Vector3 testDirection = possibleDirections[randomTurnIndex];
            Vector3 testDestination = transform.position + testDirection;

            if (IsPositionWithinBounds(testDestination) && IsPositionFreeOfPipes(testDestination))
            {
                transform.LookAt(testDestination);
                transform.position = testDestination;
                SpawnJoint();
                return true;
            }
            else
            {
                possibleDirections.Remove(testDirection);
            }
        } while (possibleDirections.Count > 0);
        return false;
    }

    private bool TryToMoveGenerator()
    {
        // tries to move generator forward, returns bool with success state
        Vector3 testDestination = transform.position + transform.forward;
        if (IsPositionWithinBounds(testDestination) && IsPositionFreeOfPipes(testDestination))
        {
            transform.LookAt(testDestination);
            transform.position = testDestination;
            return true;
        }
        else
            return false;
    }

    private GameObject SpawnPipe()
    {
        // create a pipe based on generator position and rotation
        GameObject pipeInstance = Instantiate(pipePrefab, transform.position, transform.rotation);

        // add the pipe into the pipe dictionary
        Vector3 floatPosition = pipeInstance.transform.position;
        Vector3Int intPosition = Vector3Int.RoundToInt(floatPosition);
        pipes.Add(intPosition, pipeInstance);
        return pipeInstance;
    }

    private GameObject SpawnJoint()
    {
        // create a pipe joint based on generator position and rotation
        GameObject jointInstance = Instantiate(jointPrefab, transform.position, transform.rotation);
        return jointInstance;
    }
}