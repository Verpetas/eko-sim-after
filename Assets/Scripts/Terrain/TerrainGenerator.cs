using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public class TerrainGenerator : MonoBehaviour {

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureSettings;

	public Material mapMaterial;

	public int mapSize = 21;

	Node[,] nodeGrid;
	Grid terrain;

	int chunkVertsPerLine;
    int mapVertsPerLine;

	float chunkWidth;
	bool chunkWidthFound = false;

	bool[,] chunkLocations;
    List<Vector2> chunkCoords = new List<Vector2>();

    void Awake()
	{
		chunkVertsPerLine = meshSettings.numVertsPerLine - 2;
        mapVertsPerLine = chunkVertsPerLine * mapSize - (mapSize - 1);
		nodeGrid = new Node[mapVertsPerLine, mapVertsPerLine];

        chunkLocations = new bool[mapSize, mapSize];

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        textureSettings.ApplyToMaterial(mapMaterial);
		textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

		GenerateChunks();
	}

	void GenerateChunks()
	{
		GenerateChunkLocations();

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
				if (chunkLocations[x, y])
				{
					Vector2 viewedChunkCoord = new Vector2(x, y);
					//Vector2 borderPos = Vector2.zero; //GetBorderPos(x, y);
					bool[,] borderPos = new bool[3, 3];
					for(int offsetY = -1; offsetY <= 1; offsetY++)
					{
                        for (int offsetX = -1; offsetX <= 1; offsetX++)
                        {
							int borderX = x + offsetX;
                            int borderY = y + offsetY;

							if (borderX < 0 || borderY < 0)
								borderPos[offsetX + 1, offsetY + 1] = false;
							else
								borderPos[offsetX + 1, offsetY + 1] = chunkLocations[borderX, borderY];
                        }
                    }

					//Debug.Log("****************************");

                    TerrainChunk chunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, transform, mapMaterial, borderPos);
					chunk.Generate();
					AddToNodeGrid(chunk);
				}
            }
        }

        AdjustMapPosition();
        float mapWidth = chunkWidth * mapSize;
        terrain = new Grid(nodeGrid, mapVertsPerLine, mapWidth);
    }

	void GenerateChunkLocations()
	{
		int centerCoord = mapSize / 2;
		AddChunkPoint(new Vector2(centerCoord, centerCoord));

		int chunkCount = 1;
		var random = new System.Random();
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

    void AddToNodeGrid(TerrainChunk chunk)
	{
		Vector3[,] vertices = chunk.GetVertices();
		FindChunkWidth(chunk);

        int startNodeX = (chunkVertsPerLine - 1) * (int)chunk.coord.x;
        int startNodeY = (chunkVertsPerLine - 1) * (int)chunk.coord.y;

        for (int y = 0; y < chunkVertsPerLine; y++)
		{
			for (int x = 0; x < chunkVertsPerLine; x++)
			{
                AddNode(vertices[x, y], startNodeX + x, startNodeY + y, chunk.coord);
            }
        }
    }

	void AddNode(Vector3 vertex, int nodeX, int nodeY, Vector2 chunkCoord)
	{
        Vector3 vertexPosActual = vertex + GetVertexOffset(chunkWidth, chunkCoord);
		bool walkable = vertexPosActual.y > 10;
        nodeGrid[nodeX, nodeY] = new Node(walkable, vertexPosActual, nodeX, nodeY);

		//if (walkable)
		//{
		//	GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		//	sphere.transform.position = vertexPosActual;
		//}
	}

	Vector3 GetVertexOffset(float chunkSize, Vector2 chunkCoord)
	{
		return new Vector3((0.5f + chunkCoord.x) * chunkSize, 0, (0.5f + chunkCoord.y) * chunkSize);
    }

    void AdjustMapPosition()
    {
        transform.position = new Vector3(chunkWidth/2, 0, chunkWidth/2);
    }

    Vector2 GetBorderPos(int xOffset, int yOffset)
	{
		if (mapSize > 1)
			return new Vector2(-GetAxisBorderPos(xOffset), -GetAxisBorderPos(yOffset));
		else
			return Vector2.zero;
	}

	int GetAxisBorderPos(int offset)
	{
		if (offset == 0)
			return -1;
		else if (offset == mapSize - 1)
			return 1;
		else
			return 0;
	}

	void FindChunkWidth(TerrainChunk chunk)
	{
		if (!chunkWidthFound)
		{
			chunkWidth = chunk.chunkMesh.bounds.extents.x * 2;
			chunkWidthFound = true;
        }
	}

    Vector2 RandomOffset(Vector2 baseVector)
    {
        Vector2 offset = ((Random.value < 0.5f) ? Vector2.up : Vector2.left) * ((Random.value < 0.5f) ? 1 : -1);

        return baseVector + offset;
    }

    public Grid GetTerrainGrid()
	{
		return terrain;
	}

}
