using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Grid
{
    public static bool load = false;

    private static int gridSize = 5;
    private static int moduleSize = 5;
    private static float squareSize = 3f;
    private static int oceanShelf = 2;

    private static Mesh mesh;

    public static Dictionary<int, string> gridEdges;
    public static List<int> shelfEdges;
    public static Dictionary<string, List<int>> moduleEdges;

    public static int[] raisedCorners;

    static Grid()
    {
        gridSize += (oceanShelf * 2);
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
        calcRaisedCorners();
        shelfEdges = calcShelfEdges(gridSize);
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

    public static float getShelfSize()
    {
        return oceanShelf;
    }

    // Calculates and stores references to each landmass corner, for additional side
    // height modifications
    private static void calcRaisedCorners()
    {
        raisedCorners = new int[4];
        raisedCorners[0] = (gridSize * 2) + (oceanShelf + 1);
        raisedCorners[1] = (gridSize * (oceanShelf + 1)) - oceanShelf;
        raisedCorners[2] = (gridSize * (gridSize - (oceanShelf+1))) + (oceanShelf+1);
        raisedCorners[3] = (gridSize * (gridSize - oceanShelf)) - oceanShelf;
    }

    // Calculates the modules reference numbers of the modules that form the ocean shelf
    // TODO: FIX INEFFICENCIES AND SET TO SCALE WITH SHELF SIZE
    private static List<int> calcShelfEdges(int s)
    {
        List<int> d = new List<int>();
        for(int m = 1; m < ((s*2) + 1); m++)
        {
            d.Add(m);
        }
        for(int m = s + 1; m < (s * s) - s; m += s)
        {
            d.Add(m);
        }
        for(int m = s + 2; m < (s * s) - s; m+= s)
        {
            d.Add(m);
        }
        for(int m = (s * 2); m < (s * s); m += s)
        {
            d.Add(m);
        }
        for(int m = (s * 2)-1; m < (s * s); m += s)
        {
            d.Add(m);
        }
        for(int m = (s * s) - ((s*2) - 1); m < (s * s) + 1; m++)
        {
            d.Add(m);
        }
        return d;
    }

    // Calculates landmass edges after ocean shelf modules
    // Returns a dictionary containing module number references as keys
    // and the corresponding side of the land mass which those edges relate
    private static Dictionary<int, string> calcGridEdges(int s)
    {
        Dictionary<int, string> d = new Dictionary<int, string>();

        for(int m = (s*2) + (oceanShelf+1); m < (s* (oceanShelf+1))-(oceanShelf-1); m++ )
        {
            d.Add(m, "Bottom");
        }
        for(int m = (s * (oceanShelf+1)) + (oceanShelf + 1); m < (s * (s - (oceanShelf+oceanShelf))) + (oceanShelf + 2); m += s)
        {
            d.Add(m, "Left");
        }
        for(int m = (s*(oceanShelf+oceanShelf))-oceanShelf; m < (s*(s-(oceanShelf+(oceanShelf-1))))- (oceanShelf-1); m += s)
        {
            d.Add(m, "Right");
        }
        for(int m = (s*(s-(oceanShelf+1)))+(oceanShelf+1); m < (s*(s-oceanShelf))-1; m++)
        {
            d.Add(m, "Top");
        }
        return d;
    }

    // Calculates which vectors represent module edges and returns a dictionary
    // containing a side string as keys and a vertice index as values
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
        return d;
    }

    public static bool isOceanShelf(int m)
    {
        if(shelfEdges.Contains(m))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Returns a wrapper class containing a boolean and a string
    // aBool is true if gridEdges array contains m (The modules reference number)
    // aString provides reference to the edge type
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

    // 
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
