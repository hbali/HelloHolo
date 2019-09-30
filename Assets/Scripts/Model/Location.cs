using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class Location : BaseModel
    {
        public Vertex FloorVertex { get; set; }
        public Vertex CeilingVertex { get; set; }

        public float LocationHeight
        {
            get
            {
                return CeilingVertex.Point.y - FloorVertex.Point.y;
            }
        }

        protected override string PrefabPath
        {
            get
            {
                return "Prefabs/Location";
            }
        }

        public List<Wall> Walls { get; private set; }

        public IEnumerable<Vertex> Vertices
        {
            get
            {
                List<Vertex> vertices = new List<Vertex>();
                foreach(Wall w in Walls)
                {
                    if (!vertices.Contains(w.Start)) vertices.Add(w.Start);
                    if (!vertices.Contains(w.End)) vertices.Add(w.End);
                }
                return vertices;
            }
        }

        public override void LoadModel()
        {
            base.LoadModel();
            if (Walls == null) Walls = new List<Wall>();
        }

        public void AddWall(Wall w)
        {
            Walls?.Add(w);
        }

        public void RemoveWall(Wall w)
        {
            Walls?.Remove(w);
        }
    }
}
