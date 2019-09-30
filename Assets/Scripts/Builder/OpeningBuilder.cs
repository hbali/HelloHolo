using Assets.Scripts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Builder
{
    enum OpeningShotState
    {
        Left,
        Top, 
        Right,
        Bottom
    }
    public class OpeningBuilder : IOpeningBuilder
    {
        private static OpeningShotState FinalState(OpeningType opType)
        {
            return opType == OpeningType.Door ? OpeningShotState.Right : OpeningShotState.Bottom;
        }

        private IHoloRepository _repo;

        private Vector3 left;
        private Vector3 top;
        private Vector3 right;
        private Vector3 bottom;
        private Wall shotWall;

        private OpeningShotState state;
        private OpeningType type;

        public OpeningBuilder(IHoloRepository _repo)
        {
            this._repo = _repo;
            state = OpeningShotState.Left;
        }


        public void ShotEdge(Vector3 point, OpeningType type, Wall w)
        {
            if (state == FinalState(type))
            {
                SetEdge(point, w);
                CreateOpening();
            }
            else
            {
                this.type = type;
                SetEdge(point, w);
            }
        }

        private void SetEdge(Vector3 point, Wall w)
        {
            switch (state)
            {
                case OpeningShotState.Left:
                    shotWall = w;
                    left = point;
                    break;
                case OpeningShotState.Top:
                    top = point;
                    break;
                case OpeningShotState.Right:
                    right = point;
                    break;
                case OpeningShotState.Bottom:
                    bottom = point;
                    break;
            }
            state++;
        }

        private void CreateOpening()
        {
            Vertex start = new Vertex(new Vector3(left.x, Workspace.Instance.CurrentFloorVertex.Point.y, left.z), null, VertexType.Opening);
            start.LoadModel();

            Vertex end = new Vertex(new Vector3(right.x, Workspace.Instance.CurrentFloorVertex.Point.y, right.z), null, VertexType.Opening);
            end.LoadModel();

            float sill = type == OpeningType.Door ? bottom.y : bottom.y - Workspace.Instance.CurrentFloorVertex.Point.y;
            float height = type == OpeningType.Door ? top.y - Workspace.Instance.CurrentFloorVertex.Point.y : top.y - bottom.y;

            Opening op = new Opening(start, end, sill, height, shotWall, type);
            op.LoadModel();

            state = OpeningShotState.Left;
        }
    }

    public interface IOpeningBuilder
    {
        void ShotEdge(Vector3 point, OpeningType door, Wall w);
    }
}
