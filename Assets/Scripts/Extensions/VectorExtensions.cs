using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 ToVector2(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }
    }
}
