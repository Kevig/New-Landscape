using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Threading;
using System.Collections.Generic;

public class Module : MonoBehaviour
{
    private Mesh mesh;
    private MeshFilter filter;
    private MeshCollider meshCollider;
    private MeshRenderer meshRenderer;

    private Material lines;

    private bool isVisable = false;

	// Use this for initialization
	void Start ()
    {
        // Experimental
        Thread renderGL = new Thread(new ThreadStart(this.renderLines));
        renderGL.Start();

        GLRenderGrid.OnRenderLines += this.renderLines;

        Mesh m = Grid.getMesh();

        this.mesh = new Mesh();
        this.mesh.vertices = m.vertices;
        this.mesh.triangles = m.triangles;
        this.mesh.uv = m.uv;

        this.filter = this.gameObject.AddComponent<MeshFilter>();
        this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        this.meshCollider = this.gameObject.AddComponent<MeshCollider>();

        this.filter.mesh = this.mesh;
        this.meshCollider.sharedMesh = this.mesh;
        this.meshRenderer.material = Resources.Load<Material>("Ground");

        this.transform.position = Grid.getModuleOrigin(this.getIndex());
        this.transform.parent = GameObject.Find("Landscape").transform;

        this.mesh.RecalculateNormals();
        this.mesh.Optimize();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(this.isVisable)
        {
            this.isVisable = false;
        }
    }

    public Mesh getMesh()
    {
        return this.mesh;
    }

    public int getIndex()
    {
        int index = int.Parse(this.gameObject.name.Substring
                             (this.gameObject.name.IndexOf("e") + 1));
        return index;
    }

    // Modify Y axis Vertice
    public void modifyVert(Vector3 a, float b)
    {
        Vector3[] c = this.mesh.vertices;
        int i = this.search(a);
        if(i != -1)
        {
            c[i] = new Vector3(c[i].x, c[i].y + b, c[i].z);
        }
        this.updateMesh(c);
    }

    // Compare Two Vertices by x and z values
    private bool compare(Vector3 a, Vector3 b)
    {
        bool result = false;
        if(a.x == b.x && a.z == b.z)
        {
            result = true;
        }
        return result;
    }

    // Search vertices
    public int search(Vector3 b)
    {
        int result = -1;
        int i = 0;
        
        foreach(Vector3 v in this.mesh.vertices)
        {
            Vector3 a = this.transform.TransformPoint(v);
            if(compare(a,b))
            {
                result = i;
                break;
            }
            i++;
        }
        return result;
    }

    // Returns an array of float values representing the distance between
    // each vector3 in this Mesh and a provided Vector3
    private float[] getDistances(Vector3 a)
    {
        float[] dists = new float[this.mesh.vertices.Length];
        int i = 0;
        foreach(Vector3 b in this.mesh.vertices)
        {
            dists[i] = Vector3.Distance(a, this.transform.TransformPoint(b));
            i++;
        }

        return dists;
    }

    // Returns a Vector3 representing the closest Vector3 of this objects mesh
    // to the provided Vector3
    public Vector3 getSnapPoint(Vector3 a)
    {
        Vector3 result = new Vector3();
        float[] dists = this.getDistances(a);

        result = this.mesh.vertices[Array.IndexOf(dists, dists.Min())];
        result = this.transform.TransformPoint(result);
        return result;
    }

    // Returns a Vector3 array representing the closest 4 Vector3 points of this
    // Objects mesh to the provided Vector3
    public Vector3[] getSquarePoints(Vector3 a)
    {
        Vector3[] result = new Vector3[4];
        float[] dists = this.getDistances(a);
        
        for(int i = 0; i < 4; i++)
        {
            int index = Array.IndexOf(dists, dists.Min());
            result[i] = this.mesh.vertices[index];
            result[i] = this.transform.TransformPoint(result[i]);

            dists[index] = 100f;
        }
        return result;
    }

    // Render grid lines, including triangles
    private void renderLines()
    {
        if(this.isVisable)
        {
            Material line = Resources.Load<Material>("LineRender");
            line.SetPass(0);
            GL.Color(Color.black);
            GL.Begin(GL.LINES);

            int size = this.mesh.triangles.Length;
            for(int i = 0; i < size; i += 3)
            {
                Vector3 p1 = this.transform.TransformPoint(this.mesh.vertices[this.mesh.triangles[i]]);
                Vector3 p2 = this.transform.TransformPoint(this.mesh.vertices[this.mesh.triangles[i + 1]]);
                Vector3 p3 = this.transform.TransformPoint(this.mesh.vertices[this.mesh.triangles[i + 2]]);

                GL.Vertex3(p1.x, p1.y + .001f, p1.z);
                GL.Vertex3(p2.x, p2.y + .001f, p2.z);
                GL.Vertex3(p3.x, p3.y + .001f, p3.z);
                GL.Vertex3(p1.x, p1.y + .001f, p1.z);
            }
            GL.End();
        }
    }

    // Called during rendering process.
    // If object is visible to camera this method will be called.
    // isVisable control variable determines if GLRender lines will be calculated and drawn (FPS heavy)
    // Each frame, control variable is set to false, then if visible turned on here to increase fps
    void OnWillRenderObject()
    {
        if(Camera.current.name == "Main Camera")
        {
            this.isVisable = true;
        }

        this.meshRenderer.receiveShadows = true;

        // If object is not visible, deactivate components to reduce calculations per frame
        if(!this.isVisable)
        {
            this.meshRenderer.receiveShadows = false;
        }
    }

    // Update collider to this mesh.
    // Null the reference and reassign as this Mesh
    private void updateCollider()
    {
        this.meshCollider.sharedMesh = null;
        this.meshCollider.sharedMesh = this.mesh;
    }

    public Vector3 getVertByIndex(int i)
    {
        return this.transform.TransformPoint(this.mesh.vertices[i]);
    }

    private void updateMesh(Vector3[] verts)
    {
        int[] tris = this.mesh.triangles;
        Vector2[] uvs = this.mesh.uv;

        this.mesh.Clear();
        this.meshCollider.sharedMesh = null;

        this.mesh.vertices = verts;
        this.mesh.triangles = tris;
        this.mesh.uv = uvs;

        this.filter.mesh = this.mesh;
        this.meshCollider.sharedMesh = this.mesh;
        this.mesh.RecalculateNormals();
        this.mesh.Optimize();
        
    }
}
