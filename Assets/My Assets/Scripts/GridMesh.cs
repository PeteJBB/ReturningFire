using UnityEngine;
using System.Collections;

public class GridMesh : MonoBehaviour 
{
	Vector3[] newVertices;
	Vector2[] newUV;
	int[] newTriangles;

	float spacing = 10;

	// Use this for initialization
	void Start () 
	{
		var mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		var verts = new Vector3[3];
		verts[0] = new Vector3(0,0,0);
		verts[1] = new Vector3(10,0,0);
		verts[2] = new Vector3(0,0,10);


		mesh.vertices = verts;
//		mesh.uv = newUV;
//		mesh.triangles = newTriangles;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
