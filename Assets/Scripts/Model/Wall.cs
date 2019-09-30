using Model.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class Wall : BaseModel
    {

        private static Material wallMat = Resources.Load<Material>("Materials/WallMaterial");

        public Vertex Start;
        public Vertex End;

        public Location Location { get; }

        private Transform trans;
        public float Height { get { return Location.LocationHeight; } }
        public float Thickness { get; private set; }

        private GameObject[] wallSegments;

        public Wall(Location loc, Vertex v1, Vertex v2, string guid = null)
        {
            Id = guid ?? Guid.NewGuid().ToString();
            Openings = new List<Opening>();
            this.Start = v1;
            this.End = v2;
            this.Location = loc;
            Thickness = .1f;
        }

        protected override string PrefabPath
        {
            get
            {
                return "Prefabs/Wall";
            }
        }

        public List<Opening> Openings { get; internal set; }

        public override void LoadModel()
        {
            base.LoadModel();
            mainModel.name = Id;
            Mesh[] meshes = this.MeshesForWall();
            if(wallSegments != null)
            {
                PurgeSegments(wallSegments);
            }
            wallSegments = new GameObject[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                wallSegments[i] = new GameObject();
                wallSegments[i].layer = (int)LayerEnums.Wall;
                wallSegments[i].transform.SetParent(mainModel.transform, true);
                wallSegments[i].AddComponent<MeshRenderer>().materials = new Material[]
                {
                    wallMat,
                    wallMat,
                    wallMat,
                    wallMat
                };
                wallSegments[i].AddComponent<MeshFilter>().sharedMesh = meshes[i];
                wallSegments[i].AddComponent<MeshCollider>();
            }
        }

        private void PurgeSegments(GameObject[] wallSegments)
        {
            foreach(GameObject go in wallSegments)
            {
                GameObject.Destroy(go);
            }
        }
    }
}
