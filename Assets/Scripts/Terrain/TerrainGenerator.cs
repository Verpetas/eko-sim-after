using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public class TerrainGenerator : MonoBehaviour {

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureSettings;

	public Material mapMaterial;

	public int mapSize = 1;

	Node[,] nodeGrid;
	Grid terrain;

	int chunkVertsPerLine;
    int mapVertsPerLine;

	float chunkWidth;
	bool chunkWidthFound = false;

    void Awake()
	{
		chunkVertsPerLine = meshSettings.numVertsPerLine - 2;
        mapVertsPerLine = chunkVertsPerLine * mapSize - (mapSize - 1);
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

		for (int yOffset = 0; yOffset < mapSize; yOffset++)
        {
            for (int xOffset = 0; xOffset < mapSize; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(xOffset, yOffset);
                Vector2 borderPos = GetBorderPos(xOffset, yOffset);

                TerrainChunk chunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, transform, mapMaterial, borderPos);
                chunk.Generate();
				AddToNodeGrid(chunk);
            }
        }

        AdjustMapPosition();
        float mapWidth = chunkWidth * mapSize;
        terrain = new Grid(nodeGrid, mapVertsPerLine, mapWidth);
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

	public Grid GetTerrainGrid()
	{
		return terrain;
	}

}
