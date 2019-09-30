using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public abstract class BaseModel
    {

        private Dictionary<Type, Type> modelToViewType = new Dictionary<Type, Type>()
        {
            { typeof(Model.Wall), typeof(View.Wall) },
            { typeof(Model.Vertex), typeof(View.Vertex) },
            { typeof(Model.Opening), typeof(View.Opening) },
        };

        private const string parentPath = "Workspace";

        protected abstract string PrefabPath { get; }

        protected GameObject mainModel;

        public string Id { get; set; }

        public virtual void LoadModel()
        {
            _repo.AddModel(this, this.GetType(), Id);
            if(mainModel == null)
            {
                mainModel = GameObject.Instantiate(Resources.Load<GameObject>(PrefabPath));
                mainModel.transform.SetParent(Workspace.Instance.transform, false);
                mainModel.AddComponent(modelToViewType[this.GetType()]);
            }
        }

        public static IHoloRepository _repo;
    }
}
