using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Grid
{
    public static bool load = false;

    private static int gridSize = 6;
    private static int moduleSize = 3;
    private static float squareSize = 1f;

    private static Mesh mesh;

    public static Dictionary<int, string> gridEdges;
    public static Dictionary<string, List<int>> moduleEdges;

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
        gridEdges = calcGridEdges(gridSize);
        moduleEdges = calcModuleEdges(moduleSize);
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

    private static Dictionary<int, string> calcGridEdges(int s)
    {
        Dictionary<int, string> d = new Dictionary<int, string>();
        for(int m = 1; m < (s + 1); m++)
        {
            d.Add(m, "Bottom");
            //Debug.Log(m);
        }
        for(int m = s +1; m < (s*s) - s; m+= s)
        {
            d.Add(m, "Left");
            //Debug.Log(m);
        }
        for(int m = (s * 2); m < (s*s); m+= s)
        {
            d.Add(m, "Right");
            //Debug.Log(m);
        }
        for(int m = (s * s) - (s-1); m < (s * s)+1; m++)
        {
            d.Add(m, "Top");
            //Debug.Log(m);
        }
        return d;
    }

    private static Dictionary<string, List<int>> calcModuleEdges(int s)
    {
        Dictionary<string, List<int>> d = new Dictionary<string, List<int>>();

        d.Add("Bottom", new List<int>());
        for(int m = 0; m < s+1; m++)
        {
            d["Bottom"].Add(m);
        }

        d.Add("Left", new List<int>());
        for(int m = 0; m < (s * s)+(s+1); m += s+1)
        {
            d["Left"].Add(m);
        }

        d.Add("Right", new List<int>());
        for(int m = s; m < (s * s)+(s*2)+1; m += (s+1))
        {
            d["Right"].Add(m);
        }

        d.Add("Top", new List<int>());
        for(int m = (s * s) + s; m < ((s+1) * (s+1)); m++)
        {
            d["Top"].Add(m);
        }

        foreach(var pair in d)
        {
            foreach(int i in pair.Value)
            {
                Debug.Log("Side: " + pair.Key + " value: " + i);
            }
        }

        return d;
    }

    public static EdgeWrapper isEdgeModule(int m)
    {
        bool a = false;
        string b = "";
        if(gridEdges.ContainsKey(m))
        {
            a = true;
            b = gridEdges[m];
        }
        return new EdgeWrapper(a, b);
    }

    public static bool isEdgeVert(int s, string side)
    {
        if(moduleEdges.ContainsKey(side))
        {
            if(moduleEdges[side].Contains(s))
            {
                return true;
            }
        }
        return false;
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
        Vector3 result = new Vector3(pos + (step * x), 0.0f, pos + (step * z));
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
                gridVerts[i] = new Vector3(pos + (squareSize * x), 0.0f, pos + (squareSize * z));
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
