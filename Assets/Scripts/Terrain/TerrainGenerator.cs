using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public class TerrainGenerator : MonoBehaviour {

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureSettings;

	public Material mapMaterial;

	public int mapSize = 3;

	Node[,] nodeGrid;
	Grid terrain;

    TerrainChunk chunk;

    void Start()
	{

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        textureSettings.ApplyToMaterial(mapMaterial);
		textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

		UpdateVisibleChunks();

	}

	// for testing
	//void OnValidate()
	//{

	//	textureSettings.ApplyToMaterial(mapMaterial);
	//	textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

	//	UpdateVisibleChunks();

	//}

	void UpdateVisibleChunks()
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
            }
        }

        CreateNodeGrid(chunk);
        AdjustMapPosition();

    }

	void CreateNodeGrid(TerrainChunk chunk)
	{
		Vector3[,] vertices = chunk.GetVertices();
		int vertsPerLine = vertices.GetLength(0);

		nodeGrid = new Node[vertsPerLine, vertsPerLine];
		//Debug.Log(vertices[0, 0] + ", " + vertices[50, 50]);

		for (int y = 0; y < vertsPerLine; y++)
		{
			for (int x = 0; x < vertsPerLine; x++)
			{
                nodeGrid[x, y] = new Node(true, vertices[x, y], x, y);
			}
        }

        terrain = new Grid(nodeGrid, vertsPerLine);
    }

    void AdjustMapPosition()
    {
        Bounds chunkBounds = chunk.chunkMesh.bounds;
        transform.position = new Vector3(chunkBounds.extents.x, 0, chunkBounds.extents.z);
    }

    Vector2 GetBorderPos(int xOffset, int yOffset)
	{
		if (mapSize > 1)
			return new Vector2(-GetAxisBorderPos(xOffset), GetAxisBorderPos(yOffset));
		else
			return Vector2.zero;
	}

	int GetAxisBorderPos(int offset)
	{

		if (offset == 0)
		{
			return -1;
		}
		else if (offset == mapSize - 1)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}

}
