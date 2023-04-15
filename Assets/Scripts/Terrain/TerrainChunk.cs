using UnityEngine;

public class TerrainChunk
{

    public Vector2 coord;

    bool[,] borderPos;

    GameObject meshObject;
    Vector2 sampleCentre;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    [HideInInspector]
    public Mesh chunkMesh;

    HeightMap heightMap;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;

    MeshData meshData;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, Transform parent, Material material, bool[,] borderPos)
    {

        this.coord = coord;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.borderPos = borderPos;

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;

        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;
        meshObject.layer = LayerMask.NameToLayer("Ground");

        meshObject.transform.parent = parent;
        meshObject.transform.localPosition = new Vector3(position.x, 0, position.y);

    }

    public void Generate()
    {

        this.heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre, borderPos);

        meshData = MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, 1);
        chunkMesh = meshData.CreateMesh();

        meshFilter.mesh = chunkMesh;
        meshCollider.sharedMesh = chunkMesh;

    }

    public Vector3[,] GetVertices()
    {
        return meshData.GetNodeVertices();
    }

}