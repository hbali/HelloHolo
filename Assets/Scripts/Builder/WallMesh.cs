using UnityEngine;
using System.Linq;

namespace Common
{
	public class WallMesh
	{
		public static Mesh GenerateWallMesh(Vector3 frontStart, Vector3 frontEnd, Vector3 backStart, Vector3 backEnd, float height){
			Mesh mesh = new Mesh();

			Vector3 p0 = frontStart;
			Vector3 p1 = frontEnd;
			Vector3 p2 = backEnd;
			Vector3 p3 = backStart;

			Vector3 p7 = backStart + (Vector3.up * height);
			Vector3 p6 = backEnd + (Vector3.up * height);
			Vector3 p5 = frontEnd + (Vector3.up * height);
			Vector3 p4 = frontStart + (Vector3.up * height);

			Vector3[] vertices = new Vector3[]
			{
				// Back
				p6, p7, p3, p2,

				// Front
				p4, p5, p1, p0,

				// Bottom
				p0, p1, p2, p3,

				// Top
				p7, p6, p5, p4,

				// Left
				p7, p4, p0, p3,

				// Right
				p5, p6, p2, p1
			};

			#region UVs
			Vector2 _00 = new Vector2( 0f, 0f );
			Vector2 _10 = new Vector2( 1f, 0f );
			Vector2 _01 = new Vector2( 0f, 1f );
			Vector2 _11 = new Vector2( 1f, 1f );

			Vector2[] uvs = new Vector2[]
			{
				// Top
				_11, _01, _00, _10,

				// Bottom
				_11, _01, _00, _10,

				// Back
				_11, _01, _00, _10,

				// Front
				_11, _01, _00, _10,

				// Left
				_11, _01, _00, _10,

				// Right
				_11, _01, _00, _10,
			};
			#endregion

			#region Triangles
			int[] triangles = new int[]
			{

				// Top
				3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
				3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,

				// Bottom
				3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
				3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,

				// Back
				3, 1, 0,
				3, 2, 1,	

				// Front
				3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
				3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,		

				// Left
				3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
				3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

				// Right
				3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
				3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,

			};
			#endregion

			mesh.vertices = vertices;
			mesh.uv = uvs;

			mesh.subMeshCount = 4;

			//Outer - 0, Inner - 1, Vertical Frame - 2, Horizontal Frame - 3
			mesh.SetTriangles(triangles.Skip(12).Take(6).ToArray(), 0); //back
			mesh.SetTriangles(triangles.Skip(18).Take(6).ToArray(), 1); //front
			mesh.SetTriangles(triangles.Skip(24).Take(12).ToArray(), 2); //left, right
			mesh.SetTriangles(triangles.Take(12).ToArray(), 3); //top, bottom

			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
            //MeshUtility.Optimize(mesh);

            return mesh;
		}
	}
}

