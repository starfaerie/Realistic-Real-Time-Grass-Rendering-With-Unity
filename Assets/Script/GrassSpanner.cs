//
// created by jiadong chen
// http://www.chenjd.me
//
using UnityEngine;
using System.Collections.Generic;

using Random = System.Random;

public class GrassSpanner : MonoBehaviour {


    #region 字段

    public Texture2D heightMap;
    public float terrainHeight;
    public int terrainSize = 250;
    public Material terrainMat;
    public Material grassMat;
    private List<Vector3> verts = new List<Vector3>();
    private Random random;

    #endregion

    private void Start()
    {
        random = new Random();
        GenerateTerrain();
        GenerateField(50, 50);
    }


    /// <summary>
    /// 生成地形
    /// </summary>
    private void GenerateTerrain()
    {
        List<Vector3> _verts = new List<Vector3>();
        List<int> tris = new List<int>();

        for (int i = 0; i < terrainSize; i++)
        {
            for (int j = 0; j < terrainSize; j++)
            {
                _verts.Add(new Vector3(i, heightMap.GetPixel(i, j).grayscale * terrainHeight , j));
                if (i == 0 || j == 0)
                    continue;
                tris.Add(terrainSize * i + j);
                tris.Add(terrainSize * i + j - 1);
                tris.Add(terrainSize * (i - 1) + j - 1);
                tris.Add(terrainSize * (i - 1) + j - 1);
                tris.Add(terrainSize * (i - 1) + j);
                tris.Add(terrainSize * i + j);
            }
        }

        Vector2[] uvs = new Vector2[_verts.Count];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(_verts[i].x, _verts[i].z);
        }

        GameObject plane = new GameObject("groundPlane");
        plane.AddComponent<MeshFilter>();
        MeshRenderer _renderer = plane.AddComponent<MeshRenderer>();
        _renderer.sharedMaterial = terrainMat;

        Mesh groundMesh = new Mesh
        {
            vertices = _verts.ToArray(), uv = uvs, triangles = tris.ToArray()
        };

        groundMesh.RecalculateNormals(); 
        plane.GetComponent<MeshFilter>().mesh = groundMesh;

        verts.Clear();
    }

    /// <summary>
    /// 生成草地
    /// </summary>
    /// <param name="grassPatchRowCount"></param>
    /// <param name="grassCountPerPatch"></param>
    private void GenerateField(int grassPatchRowCount, int grassCountPerPatch)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < 65000; i++)
        {
            indices.Add(i);
        }

        Vector3 startPosition = new Vector3(0, 0, 0);
        Vector3 patchSize = new Vector3((float)terrainSize / grassPatchRowCount, 0, (float)terrainSize / grassPatchRowCount);

        for (int x = 0; x < grassPatchRowCount; x++)
        {
            for (int y = 0; y < grassPatchRowCount; y++)
            {
                GenerateGrass(startPosition, patchSize, grassPatchRowCount);
                startPosition.x += patchSize.x;
            }

            startPosition.x = 0;
            startPosition.z += patchSize.z;
        }

        GameObject grassLayer;
        MeshFilter mf;
        MeshRenderer _renderer;
        Mesh m;

        while (verts.Count > 65000)
        {
            m = new Mesh
            {
                vertices = verts.GetRange(0, 65000).ToArray()
            };
            m.SetIndices(indices.ToArray(), MeshTopology.Points, 0);

            grassLayer = new GameObject("grassLayer");
            mf = grassLayer.AddComponent<MeshFilter>();
            _renderer = grassLayer.AddComponent<MeshRenderer>();
            _renderer.sharedMaterial = grassMat;
            mf.mesh = m;
            verts.RemoveRange(0, 65000);
        }

        m = new Mesh
        {
            vertices = verts.ToArray()
        };
        m.SetIndices(indices.GetRange(0, verts.Count).ToArray(), MeshTopology.Points, 0);
        grassLayer = new GameObject("grassLayer");
        mf = grassLayer.AddComponent<MeshFilter>();
        _renderer = grassLayer.AddComponent<MeshRenderer>();
        _renderer.sharedMaterial = grassMat;
        mf.mesh = m;
    }

    private void GenerateGrass(Vector3 startPosition, Vector3 patchSize, int grassCountPerPatch)
    {
        for (int i = 0; i < grassCountPerPatch; i++)
        {
            float randomizedZDistance = (float)random.NextDouble() * patchSize.z;
            float randomizedXDistance = (float)random.NextDouble() * patchSize.x;

            int indexX = (int)(startPosition.x + randomizedXDistance);
            int indexZ = (int)(startPosition.z + randomizedZDistance);

            if (indexX >= terrainSize)
            {
                indexX = terrainSize - 1;
            }

            if (indexZ >= terrainSize)
            {
                indexZ = terrainSize - 1;
            }

            Vector3 currentPosition = new Vector3(startPosition.x + randomizedXDistance, heightMap.GetPixel(indexX, indexZ).grayscale * (terrainHeight + 1), startPosition.z + randomizedZDistance);
            verts.Add(currentPosition);
        }
    }

}
