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

		textureSettings.ApplyToMaterial(mapMaterial);
		textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

		UpdateVisibleChunks();

    }

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
		return new Vector2(GetAxisBorderPos(-xOffset), GetAxisBorderPos(yOffset));
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
