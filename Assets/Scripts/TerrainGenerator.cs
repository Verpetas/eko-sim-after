using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour {

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureSettings;

	public Material mapMaterial;

	public int mapSize = 1;

	void Start()
	{

		textureSettings.ApplyToMaterial(mapMaterial);
		textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

		UpdateVisibleChunks();

    }

	void UpdateVisibleChunks()
	{

		for (int yOffset = -mapSize; yOffset <= mapSize; yOffset++)
        {
            for (int xOffset = -mapSize; xOffset <= mapSize; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(xOffset, yOffset);

                TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, transform, mapMaterial);
				newChunk.Load();
            }
        }

    }

}
