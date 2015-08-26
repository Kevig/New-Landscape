using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class Marker : MonoBehaviour {

    private SelectionInfoGUI selectionInfoGUI;

    private Mesh mesh;
    private MeshFilter filter;
    private Renderer meshRenderer;
    private MeshCollider meshCollider;

    public void Awake()
    {
        this.selectionInfoGUI = GameObject.Find("SelectionInfo").GetComponent<SelectionInfoGUI>();
    }

    public void initObj(int type, Vector3[] points)
    {
        this.gameObject.name = "Marker";
        switch(type)
        {
            case 1:
                this.meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
                this.transform.localScale = new Vector3(.6f, .6f, .6f);
                this.setPosition(points[0]);
                break;

            case 2:
                this.mesh = new Mesh();
                this.filter = this.gameObject.AddComponent<MeshFilter>();
                this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
                this.meshCollider = this.gameObject.AddComponent<MeshCollider>();
                this.setMesh(points);
                break;
        }

        this.meshRenderer.material = Resources.Load<Material>("Mat_Marker");
    }

    private Vector3[] sortVectors(Vector3[] v)
    {
        Vector3[] verts = new Vector3[4];

        float[] x = new float[4];
        float[] z = new float[4];
        for(int i = 0; i < 4; i++)
        {
            x[i] = v[i].x;
            z[i] = v[i].z;
        }

        float minX = x.Min();
        float maxX = x.Max();
        float minZ = z.Min();
        float maxZ = z.Max();

        for(int i = 0; i < 4; i++)
        {
            if(v[i].x == minX && v[i].z == minZ)
            {
                verts[0] = v[i];
            }
            if(v[i].x == maxX && v[i].z == minZ)
            {
                verts[1] = v[i];
            }
            if(v[i].x == minX && v[i].z == maxZ)
            {
                verts[2] = v[i];
            }
            if(v[i].x == maxX && v[i].z == maxZ)
            {
                verts[3] = v[i];
            }
        }
        return verts;
    }

    public void setMesh(Vector3[] v)
    {
        v = this.sortVectors(v);
        for(int i = 0; i < 4; i++)
        {
            v[i] = new Vector3(v[i].x, v[i].y + 0.001f, v[i].z);
        }
        Vector2[] uvs = new Vector2[4];
        for(int i = 0; i < 4; i++)
        {
            uvs[i] = new Vector2(v[i].x, v[i].z);
        }
        this.mesh.vertices = v;
        this.mesh.triangles = new int[6] { 1, 2, 3, 0, 2, 1 };
        this.mesh.uv = uvs;
        this.filter.mesh = this.mesh;
        this.meshCollider.sharedMesh = null;
        this.meshCollider.sharedMesh = this.mesh;

        this.updateGUI(v);
    }

    public Vector3[] getMeshVerts()
    {
        Vector3[] verts = new Vector3[4];
        int i = 0;
        foreach(Vector3 v in this.mesh.vertices)
        {
            verts[i] = new Vector3(v.x, v.y, v.z);
            verts[i] = this.transform.TransformPoint(v);
            i++;
        }
        return verts;
    }

    public void modifyMeshY(float n)
    {
        int i = 0;
        Vector3[] c = this.mesh.vertices;
        foreach(Vector3 v in this.mesh.vertices)
        {
            c[i] = new Vector3(v.x, v.y + n, v.z);
            i++;
        }

        this.meshCollider.sharedMesh = null;
        this.meshCollider.sharedMesh = this.mesh;
        this.mesh.vertices = c;
        this.updateGUI(c);
    }

    public void setPosition(Vector3 pos)
    {
        this.transform.position = pos;
        this.updateGUI(pos);
    }

    public Vector3 getPosition()
    {
        return this.transform.position;
    }

    private void updateGUI(Vector3[] v)
    {
        this.selectionInfoGUI.UpdateGUI(v);
    }

    private void updateGUI(Vector3 v)
    {
        this.selectionInfoGUI.UpdateGUI(v);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
