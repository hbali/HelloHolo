using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using Model.Builders;
using UnityEngine;

namespace Assets.Scripts.Builder
{
    class StructureBuilder : IStructureBuilder
    {
        private IHoloRepository _repo;
        private Vertex v1;
        private Vertex v2;
        private Wall wall;

        private Vertex first;
        public Location ActiveLoc { get; private set; }
        
        public StructureBuilder(IHoloRepository repo)
        {
            _repo = repo;
        }


        public void CreateVertex(Vector3 point)
        {
            point.y = ActiveLoc.FloorVertex.Point.y;
            if (v1 == null)
            {
                first = v1 = CreateVertex(point, VertexType.Wall);
            }
            else if(v2 == null)
            {
                if (!TooCloseToFirst(point))
                {
                    v2 = CreateVertex(point, VertexType.Wall);
                    CreateWall(v1, v2);
                    v1 = v2;
                    v2 = null;
                }
                else
                {
                    FinishLocation();
                }
            }
        }

        private bool TooCloseToFirst(Vector3 point)
        {
            Vector2 v1 = first.Point.ToVector2();
            Vector2 v2 = point.ToVector2();
            return Vector2.Distance(v1, v2) < 0.2f;
        }

        private void FinishLocation()
        {
            CreateWall(v1, first);
        }

        private Vertex CreateVertex(Vector3 point, VertexType type)
        {
            Vertex v = new Vertex(point, null, type);
            v.LoadModel();
            return v;
        }

        private void CreateWall(Vertex v1, Vertex v2)
        {
            /*if (wall == null)
            {
                TryReverse(ref v1, ref v2);
            }*/
            wall = new Wall(ActiveLoc, v1, v2);
            wall.LoadModel();
            ActiveLoc.AddWall(wall);
        }

        private void TryReverse(ref Vertex v1, ref Vertex v2)
        {
            bool reverse = WallBuilder.NormalCloserToPoint(v1.Point, v2.Point, Camera.main.transform.position);
            if (reverse)
            {
                Vertex temp = v1;
                v1 = v2;
                v2 = temp;
                temp = null;
            }
        }

        public void CreateFloor(Vector3 point)
        {
            point.x = point.z = 0;
            ActiveLoc.FloorVertex = CreateVertex(point, VertexType.Floor);
        }

        public void CreateCeiling(Vector3 point)
        {
            point.x = point.z = 0;
            ActiveLoc.CeilingVertex = CreateVertex(point, VertexType.Ceiling);
        }

        public void Init()
        {
            Location loc = new Location();
            ActiveLoc = loc;
        }
    }

    internal interface IStructureBuilder
    {
        Location ActiveLoc { get; }

        void CreateVertex(Vector3 point);
        void CreateFloor(Vector3 point);
        void CreateCeiling(Vector3 point);
        void Init();
    }
}
