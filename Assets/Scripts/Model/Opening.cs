using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public enum OpeningType
    {
        Door, 
        Window
    }
    public class Opening : BaseModel
    {
        private static float orig_height(OpeningType type)
        {
            return type == OpeningType.Door ? 2.1f : 0.8f;
        }
        private static float orig_width(OpeningType type)
        {
            return type == OpeningType.Door ? 1.2f : 1.5f;
        }


        public Vertex Start;
        public Vertex End;
        private Wall wall;

        public float Sill { get; set; }
        public float Height { get; set; }

        public Opening(Vertex start, Vertex end, float sill, float height, Wall w, OpeningType type, string guid = null)
        {
            Id = guid ?? Guid.NewGuid().ToString();
            this.Start = start;
            this.End = end;
            this.Sill = sill;
            this.Height = height;
            this.type = type;
            wall = w;
            wall.Openings.Add(this);
        }

        public OpeningType type { get; private set; }

        protected override string PrefabPath
        {
            get
            {
                return "Prefabs/" + type.ToString() + "/prefab";
            }
        }

        public override void LoadModel()
        {
            base.LoadModel();
            mainModel.transform.position = new Vector3(((Start.Point + End.Point) / 2).x, 
                wall.Location.FloorVertex.Point.y + Sill,
                ((Start.Point + End.Point) / 2).z);

            float angle = Mathf.Atan2((End.Point - Start.Point).z, (End.Point - Start.Point).x);
            mainModel.transform.localEulerAngles = new UnityEngine.Vector3(mainModel.transform.localEulerAngles.x,
                180 - (angle * Mathf.Rad2Deg),
                mainModel.transform.localEulerAngles.z);

            if (type == OpeningType.Door)
            {
                mainModel.transform.localScale = new Vector3(Vector3.Distance(Start.Point, End.Point) / orig_width(this.type), Height / orig_height(this.type), wall.Thickness);
            }
            else
            {
                mainModel.transform.localScale = new Vector3(Vector3.Distance(Start.Point, End.Point) / orig_width(this.type), wall.Thickness, Height / orig_height(this.type));
                mainModel.transform.localScale *= 100;  
            }
            wall.LoadModel();
        }
    }
}
