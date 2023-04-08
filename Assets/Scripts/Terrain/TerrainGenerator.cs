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
    TerrainChunk chunk;

	int chunkVertsPerLine;
    int mapVertsPerLine;

	Vector3 chunkExtents;
	bool chunkExtentsReceived = false;

    void Awake()
	{
		chunkVertsPerLine = meshSettings.numVertsPerLine - 2;
        mapVertsPerLine = chunkVertsPerLine * mapSize;
		nodeGrid = new Node[mapVertsPerLine, mapVertsPerLine];

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        textureSettings.ApplyToMaterial(mapMaterial);
		textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

		GenerateChunks();
	}

	// for testing
	//void OnValidate()
	//{

	//	textureSettings.ApplyToMaterial(mapMaterial);
	//	textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

	//	UpdateVisibleChunks();

	//}

	void GenerateChunks()
	{

		//for (int yOffset = -chunksFromCenter; yOffset <= chunksFromCenter; yOffset++)
		//{
		//	for (int xOffset = -chunksFromCenter; xOffset <= chunksFromCenter; xOffset++)
		//	{
		//		Vector2 viewedChunkCoord = new Vector2(xOffset, yOffset);
		//		Vector2 borderPos = GetBorderPos(xOffset, yOffset);

		//		TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, transform, mapMaterial, borderPos);
		//		newChunk.Generate();
		//	}
		//}

		for (int yOffset = 0; yOffset < mapSize; yOffset++)
        {
            for (int xOffset = 0; xOffset < mapSize; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(xOffset, yOffset);
                Vector2 borderPos = GetBorderPos(xOffset, yOffset);

                chunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, transform, mapMaterial, borderPos);
                chunk.Generate();
				AddToNodeGrid(chunk);
            }
        }

        AdjustMapPosition();
        float mapWidth = chunkExtents.x * 2 * mapSize;
        terrain = new Grid(nodeGrid, mapVertsPerLine, mapWidth);
    }

	void AddToNodeGrid(TerrainChunk chunk)
	{
		Vector3[,] vertices = chunk.GetVertices();
		FindChunkExtents();
		float chunkSize = chunkExtents.x * 2;

        int startNodeX = chunkVertsPerLine * (int)chunk.coord.x;
        int startNodeY = chunkVertsPerLine * (int)chunk.coord.y;

        for (int y = 0; y < chunkVertsPerLine; y++)
		{
			for (int x = 0; x < chunkVertsPerLine; x++)
			{
				Vector3 vertexPosActual = vertices[x, y] + GetVertexOffset(chunkSize, chunk.coord);
                nodeGrid[startNodeX + x, startNodeY + y] = new Node(true, vertexPosActual, startNodeX + x, startNodeY + y);
			}
        }
    }

	Vector3 GetVertexOffset(float chunkSize, Vector2 chunkCoord)
	{
		return new Vector3((0.5f + chunkCoord.x) * chunkSize, 0, (0.5f + chunkCoord.y) * chunkSize);
    }

    void AdjustMapPosition()
    {
        transform.position = new Vector3(chunkExtents.x, 0, chunkExtents.z);
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

	void FindChunkExtents()
	{
		if (!chunkExtentsReceived)
		{
			chunkExtents = chunk.chunkMesh.bounds.extents;
			chunkExtentsReceived = true;
        }
	}

	public Grid GetTerrainGrid()
	{
		return terrain;
	}

}
