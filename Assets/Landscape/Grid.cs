using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Grid
{
    public static bool load = false;

    private static int gridSize = 30;
    private static int moduleSize = 3;
    private static float squareSize = 1f;

    private static Mesh mesh;

    static Grid()
    {
        buildMesh();
    }

    // Build Mesh
    private static void buildMesh()
    {
        int size = moduleSize + 1;
        Vector3[] verts = mapVerts(size);

        mesh = new Mesh();

        mesh.vertices = verts;
        mesh.uv = mapUvs(verts);
        mesh.triangles = mapTris();
    }

    // Mesh accessor
    public static Mesh getMesh()
    {
        return mesh;
    }

    // gridSize accessor
    public static int getGridSize()
    {
        return gridSize;
    }
    
    public static int getModuleSize()
    {
        return moduleSize;
    }

    public static float getSquareSize()
    {
        return squareSize;
    }

    // Get module origin coordinates
    public static Vector3 getModuleOrigin(int s)
    {
        float offset = 0;
        float step = moduleSize * squareSize;

        if(gridSize % 2 == 0)
        {
            offset = (moduleSize * squareSize)/2;
        }
        int z = (s-1) / gridSize;
        int x = (s-1) - (z * gridSize);

        float pos = -((gridSize / 2) * (moduleSize * squareSize)) + offset;
        Vector3 result = new Vector3(pos + (step * x), 0f, pos + (step * z));
        return result;
    }

    // Map grid coordinates
    private static Vector3[] mapVerts(int size)
    {
        Vector3[] gridVerts = new Vector3[size * size];

        float pos = -((moduleSize * squareSize) / 2);

        int i = 0;
        for(int z = 0; z < size; z++)
        {
            for(int x = 0; x < size; x++)
            {
                gridVerts[i] = new Vector3(pos + (squareSize * x), 0f, pos + (squareSize * z));
                i++;
            }
        }
        return gridVerts;
    }

    // Map grid uvs
    private static Vector2[] mapUvs(Vector3[] verts)
    {
        Vector2[] gridUvs = new Vector2[verts.Length];

        int i = 0;
        foreach(Vector3 v in verts)
        {
            gridUvs[i] = new Vector2(v.x, v.z);
        }
        return gridUvs;
    }

    // Map grid triangles
    private static int[] mapTris()
    {
        int size = moduleSize;
        int[] gridTris = new int[((size*size) * 2) * 3];

        int p = 0;
        int p1 = moduleSize + 1;
        int i = 0;

        for(int z = 0; z < size; z++)
        {
            for(int x = 0; x < size; x++)
            {
                gridTris[i] = p;
                gridTris[i + 1] = p1;
                gridTris[i + 2] = p + 1;
                gridTris[i + 3] = p1;
                gridTris[i + 4] = p1 + 1;
                gridTris[i + 5] = p + 1;
                p++;
                p1++;
                i += 6;
            }
            p++;
            p1++;
        }
        return gridTris;
    }
}
