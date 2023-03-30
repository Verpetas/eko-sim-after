﻿using UnityEngine;

public class TerrainChunk
{

    public Vector2 coord;

    GameObject meshObject;
    Vector2 sampleCentre;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    Mesh chunkMesh;

    HeightMap heightMap;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, Transform parent, Material material)
    {

        this.coord = coord;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;

        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;

    }

    public void Generate()
    {

        this.heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre);

        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, 0);
        chunkMesh = meshData.CreateMesh();

        meshFilter.mesh = chunkMesh;
        meshCollider.sharedMesh = chunkMesh;

    }

}