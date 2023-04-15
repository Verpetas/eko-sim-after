using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine.UIElements;

public class TerrainGenerator : MonoBehaviour {

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureSettings;

	public Material mapMaterial;

	public int chunksToGenerateNo = 21;

	Node[,] nodeGrid;
	Grid terrain;

	const int chunkVertsPerLine = 126;
    int mapVertsPerLine;

	float chunkWidth;
	bool chunkWidthFound = false;

	bool[,] chunkLocations;
    List<Vector2> chunkCoords = new List<Vector2>();

	Vector2 closestChunkCoord; // <-- MOVE TO A SEPARATE CLASS / SCRIPT
	Vector2 farthestChunkCoord;

	int mapMaxSize;

	void Awake()
	{
        mapMaxSize = (chunksToGenerateNo % 2 == 0) ? chunksToGenerateNo + 5 : chunksToGenerateNo + 4;
        chunkLocations = new bool[mapMaxSize, mapMaxSize];

        //chunkVertsPerLine = meshSettings.numVertsPerLine - 2;
        mapVertsPerLine = chunkVertsPerLine * chunksToGenerateNo - (chunksToGenerateNo - 1); // <-- still to change
		nodeGrid = new Node[mapVertsPerLine, mapVertsPerLine];

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

		(int, int) yBounds = ((int)closestChunkCoord.y - 1, (int)farthestChunkCoord.y + 2);
		(int, int) xBounds = ((int)closestChunkCoord.x - 1, (int)farthestChunkCoord.x + 2);

        for (int y = yBounds.Item1; y < yBounds.Item2; y++)
        {
            for (int x = xBounds.Item1; x < xBounds.Item2; x++)
            {
				if (chunkLocations[x, y])
				{
					Vector2 viewedChunkCoord = new Vector2(x, y) + Vector2.one - closestChunkCoord; // bring closer to zero coord
					
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

                    TerrainChunk chunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, transform, mapMaterial, borderPos);
					chunk.Generate();
					AddToNodeGrid(chunk);
				}
            }
        }

        AdjustMapPosition();
        float mapWidth = chunkWidth * mapMaxSize;
        terrain = new Grid(nodeGrid, mapVertsPerLine, mapWidth);
    }

	void GenerateChunkLocations()
	{
		int centerCoord = (mapMaxSize - 1) / 2;
		AddChunkPoint(new Vector2(centerCoord, centerCoord));

		float minX, minY, maxX, maxY;
		minX = minY = maxX = maxY = centerCoord;

        int chunkCount = 1;
		var random = new System.Random();
		while (chunkCount < chunksToGenerateNo)
		{
			int chunkIndex = random.Next(chunkCoords.Count);
			Vector2 newChunkCoord = RandomOffset(chunkCoords[chunkIndex]);

            if (!chunkLocations[(int)newChunkCoord.x, (int)newChunkCoord.y])
            {
                AddChunkPoint(newChunkCoord);
                chunkCount++;

				if (newChunkCoord.x < minX)
					minX = newChunkCoord.x;
                else if (newChunkCoord.x > maxX)
                    maxX = newChunkCoord.x;

                if (newChunkCoord.y < minY)
					minY = newChunkCoord.y;
                else if (newChunkCoord.y > maxY)
                    maxY = newChunkCoord.y;
            }
        }

		closestChunkCoord = new Vector2(minX, minY);
        farthestChunkCoord = new Vector2(maxX, maxY);
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
		//Debug.Log("Vertex count: " + vertices.GetLength(1));

        int startNodeX = (chunkVertsPerLine - 1) * (int)chunk.coord.x;
        int startNodeY = (chunkVertsPerLine - 1) * (int)chunk.coord.y;

        for (int y = 0; y < 126/*chunkVertsPerLine*/; y++)
		{
			for (int x = 0; x < 126/*chunkVertsPerLine*/; x++)
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

		//GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		//sphere.transform.position = vertexPosActual;
	}

	Vector3 GetVertexOffset(float chunkSize, Vector2 chunkCoord)
	{
		return new Vector3((0.5f + chunkCoord.x) * chunkSize, 0, (0.5f + chunkCoord.y) * chunkSize);
    }

    void AdjustMapPosition()
    {
		//Debug.Log(chunkWidth);
        transform.position = new Vector3(chunkWidth / 2, 0, chunkWidth / 2);
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
