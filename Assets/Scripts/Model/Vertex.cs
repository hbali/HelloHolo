using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public enum VertexType
    {
        Wall,
        Floor,
        Ceiling,
        Opening
    }

    public class Vertex : BaseModel
    {
        private VertexType type;

        public Vector3 Point { get; private set; }

        protected override string PrefabPath
        {
            get
            {
                return "Prefabs/Vertex";
            }
        }

        public Vertex(Vector3 point, string guid = null)
        {
            Id = guid ?? Guid.NewGuid().ToString();
            this.Point = point;
        }

        public Vertex(Vector3 point, string guid = null, VertexType corner = default(VertexType)) : this(point, guid)
        {
            this.type = corner;
        }

        public override void LoadModel()
        {
            base.LoadModel();
            mainModel.name = type.ToString() + "_" + Id;
            SetPosition();
        }

        private void SetPosition()
        {
            mainModel.transform.position = Point;
        }
    }
}
