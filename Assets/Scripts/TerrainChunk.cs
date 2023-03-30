using UnityEngine;

public class TerrainChunk
{

    public Vector2 coord;

    GameObject meshObject;
    Vector2 sampleCentre;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    LODMesh[] lodMeshes;

    HeightMap heightMap;
    bool heightMapReceived;

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
        SetVisible(false);

        lodMeshes = new LODMesh[1];
        lodMeshes[0] = new LODMesh(0);
    }

    public void Load()
    {

        this.heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre);
        heightMapReceived = true;

        UpdateTerrainChunk();
    }

    public void UpdateTerrainChunk()
    {
        if (heightMapReceived)
        {
            LODMesh lodMesh = lodMeshes[0];

            MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, 0 /* lod */);
            lodMesh.mesh = meshData.CreateMesh();
            lodMesh.hasMesh = true;

            meshFilter.mesh = lodMesh.mesh;

            UpdateCollisionMesh();

            SetVisible(true);
        }
    }

    public void UpdateCollisionMesh()
    {

        meshCollider.sharedMesh = lodMeshes[0].mesh;
    }

    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }

}

class LODMesh
{

    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    int lod;
    public event System.Action updateCallback;

    public LODMesh(int lod)
    {
        this.lod = lod;
    }

    void OnMeshDataReceived(object meshDataObject)
    {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        hasMesh = true;

        updateCallback();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
    }

}


