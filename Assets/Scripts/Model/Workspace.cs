using Assets.Scripts.Builder;
using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model
{
    class Workspace : Singleton<Workspace>
    {
        private IStructureBuilder _builder;

        public Vertex CurrentFloorVertex { get { return _builder.ActiveLoc.FloorVertex; } }
        public Vertex CurrentCeilingVertex { get { return _builder.ActiveLoc.CeilingVertex; } }


        public void SetDependencies(IStructureBuilder _builder)
        {
            this._builder = _builder;
        }
    }
}
