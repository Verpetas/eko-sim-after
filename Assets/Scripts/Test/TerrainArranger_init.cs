using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainArranger_init : MonoBehaviour
{
    public GameObject chunk;
    public GameObject startingChunk;

    List<Vector2> chunkCoords = new List<Vector2>();
    bool[,] chunkLocations;
    int mapSize = 21;
    int centerCoord;

    private void Awake()
    {
        if(mapSize % 2 != 0) mapSize++;
        chunkLocations = new bool[mapSize, mapSize];

        centerCoord = (mapSize - 1) / 2;
    }

    void Start()
    {
        GenerateLocations();
        SpawnChunks();
    }

    void GenerateLocations()
    {
        AddChunkPoint(new Vector2(centerCoord, centerCoord));

        var random = new System.Random();
        int chunkCount = 1;
        while (chunkCount < mapSize)
        {
            int chunkIndex = random.Next(chunkCoords.Count);
            Vector2 newChunkCoords = RandomOffset(chunkCoords[chunkIndex]);

            if (!chunkLocations[(int)newChunkCoords.x, (int)newChunkCoords.y])
            {
                AddChunkPoint(newChunkCoords);
                chunkCount++;
            }
        }
    }

    void AddChunkPoint(Vector2 chunkPoint)
    {
        chunkCoords.Add(chunkPoint);
        chunkLocations[(int)chunkPoint.x, (int)chunkPoint.y] = true;
    }

    void SpawnChunks()
    {
        for(int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                if (chunkLocations[x, y])
                {
                    GameObject spawnedChunk = (x == centerCoord && y == centerCoord) ? startingChunk : chunk;
                    Instantiate(spawnedChunk, new Vector3(x, 0, y), Quaternion.identity);
                }
            }
        }
    }

    Vector2 RandomOffset(Vector2 baseVector)
    {
        Vector2 offset = ((Random.value < 0.5f) ? Vector2.up : Vector2.left) * ((Random.value < 0.5f) ? 1 : -1);

        return baseVector + offset;
    }

}
