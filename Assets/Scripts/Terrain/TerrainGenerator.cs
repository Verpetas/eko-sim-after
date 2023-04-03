using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public class TerrainGenerator : MonoBehaviour {

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureSettings;

	public Material mapMaterial;

	public int chunksFromCenter = 1;

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

		for (int yOffset = -chunksFromCenter; yOffset <= chunksFromCenter; yOffset++)
        {
            for (int xOffset = -chunksFromCenter; xOffset <= chunksFromCenter; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(xOffset, yOffset);
				Vector2 borderPos = GetBorderPos(xOffset, yOffset);

                TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, transform, mapMaterial, borderPos);
				newChunk.Generate();
            }
        }

    }

	Vector2 GetBorderPos(int xOffset, int yOffset)
	{
		if (chunksFromCenter > 0)
			return new Vector2(GetAxisBorderPos(-xOffset), GetAxisBorderPos(yOffset));
		else
			return new Vector2(0, 0);
	}

	int GetAxisBorderPos(int offset)
	{

		if (offset == -chunksFromCenter)
		{
			return -1;
		}
		else if (offset == chunksFromCenter)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}

}
