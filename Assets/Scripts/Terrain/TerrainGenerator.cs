using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class TerrainGenerator : MonoBehaviour {

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureSettings;

	public Material mapMaterial;

	public int chunksToGenerateNo = 21;

    public GameObject oceanTile;

    Node[,] nodeGrid;
	Grid terrain;

	const int chunkVertsPerLine = 121;
    (int, int) mapVertsPerLine;

    float chunkWidth;

	bool[,] chunkLocations;
    List<Vector2> chunkCoords = new List<Vector2>();

	Vector2 closestChunkCoord;
	Vector2 farthestChunkCoord;

	int mapMaxSize;

	void Awake()
	{
		chunkWidth = meshSettings.meshWorldSize;

        mapMaxSize = (chunksToGenerateNo % 2 == 0) ? chunksToGenerateNo + 3 : chunksToGenerateNo + 2;
        chunkLocations = new bool[mapMaxSize, mapMaxSize];

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

		(int, int) yBounds = ((int)closestChunkCoord.y, (int)farthestChunkCoord.y + 1);
		(int, int) xBounds = ((int)closestChunkCoord.x, (int)farthestChunkCoord.x + 1);
		(int, int) chunksPerLine = (xBounds.Item2 - xBounds.Item1, yBounds.Item2 - yBounds.Item1);
		mapVertsPerLine = (chunksPerLine.Item1 * chunkVertsPerLine, chunksPerLine.Item2 * chunkVertsPerLine);
		nodeGrid = new Node[mapVertsPerLine.Item1, mapVertsPerLine.Item2];

        for (int y = yBounds.Item1; y < yBounds.Item2; y++)
        {
            for (int x = xBounds.Item1; x < xBounds.Item2; x++)
            {
                Vector2 viewedChunkCoord = new Vector2(x, y) - closestChunkCoord; // bring to zero coord

                if (chunkLocations[x, y])
				{
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
                else
                {
                    AssignOceanNodes(viewedChunkCoord);
                }
            }
        }

        AdjustMapPosition();
		(float, float) mapSize = (chunksPerLine.Item1 * chunkWidth, chunksPerLine.Item2 * chunkWidth);
        terrain = new Grid(nodeGrid, mapVertsPerLine, mapSize);
    }

	void AssignOceanNodes(Vector2 chunkCoord)
	{
        int startNodeX = chunkVertsPerLine * (int)chunkCoord.x;
        int startNodeY = chunkVertsPerLine * (int)chunkCoord.y;

        for (int y = startNodeY; y < startNodeY + chunkVertsPerLine; y++)
        {
            for (int x = startNodeX; x < startNodeX + chunkVertsPerLine; x++)
            {
                nodeGrid[x, y] = new Node(false, Vector3.zero, 0, 0);
            }
        }
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

        int startNodeX = chunkVertsPerLine * (int)chunk.coord.x;
        int startNodeY = chunkVertsPerLine * (int)chunk.coord.y;

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
        bool walkable = vertexPosActual.y > 6 && vertexPosActual.y < 25;
        nodeGrid[nodeX, nodeY] = new Node(walkable, vertexPosActual, nodeX, nodeY);

        //if (walkable)
        //{
        //    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    sphere.transform.position = vertexPosActual;
        //}
    }

	Vector3 GetVertexOffset(float chunkSize, Vector2 chunkCoord)
	{
		return new Vector3((0.5f + chunkCoord.x) * chunkSize, 0, (0.5f + chunkCoord.y) * chunkSize);
    }

    void AdjustMapPosition()
    {
        transform.position = new Vector3(chunkWidth / 2, 0, chunkWidth / 2);
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
