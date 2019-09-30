using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class GeometryExtensions
    {
        /// <summary>
        /// Calculates cartesian coordinates from polar coordinates
        /// and an offset.
        /// </summary>
        /// <param name="origin">Origin of the polar coordinate system</param>
        /// <param name="angle">Angle to x axis in radians</param>
        /// <param name="distance">Distance from origin</param>
        /// <returns></returns>
        public static Vector2 Polar2Cartesian(Vector2 origin, float angle, float distance)
        {
            return origin + Polar2Cartesian(angle, distance);
        }

        /// <summary>
        /// Calculates cartesian coordinates from polar coordinates.
        /// </summary>
        /// <param name="angle">Angle to x axis in radians</param>
        /// <param name="distance">Distance from (0,0)</param>
        /// <returns></returns>
        public static Vector2 Polar2Cartesian(float angle, float distance)
        {
            return new Vector2(
                Mathf.Cos(angle) * distance,
                Mathf.Sin(angle) * distance
                );
        }

        /// <summary>
        /// Returns the angle of the line described by the two
        /// parameter points to the x axis, in degrees.
        /// </summary>
        /// <param name="A">A point on the line</param>
        /// <param name="B">Another point on the line</param>
        /// <returns></returns>
        public static float GetAngle(Vector2 A, Vector2 B)
        {
            Vector2 C = new Vector2(B.x + 1, B.y);
            Vector2 v1 = new Vector2(C.x - B.x, C.y - B.y);
            Vector2 v2 = new Vector2(A.x - B.x, A.y - B.y);
            return 180 + ((Mathf.Atan2(v2.y, v2.x) - Mathf.Atan2(v1.y, v1.x)) * Mathf.Rad2Deg);
        }
        
        /// <summary>
        /// Returns the angle of the line described by the two
        /// parameter points to the x axis, in radians.
        /// </summary>
        /// <param name="A">A point on the line</param>
        /// <param name="B">Another point on the line</param>
        /// <returns></returns>
        public static float GetAngleRadian(Vector2 A, Vector2 B)
        {
            return GetAngle(A, B) * Mathf.Deg2Rad;
        }

        public static Vector2 GetPointFrom2Points2Distances(Vector2 p1, float d1, Vector2 p2, float d2, bool left)
        {
            float d = Vector2.Distance(p1, p2);
            float distV1 = (d1 * d1 - d2 * d2 + d * d) / (2 * d);
            float h = Mathf.Sqrt(Mathf.Abs(d1 * d1 - distV1 * distV1));
            float x3 = p1.x + (distV1 * (p2.x - p1.x)) / d;
            float y3 = p1.y + (distV1 * (p2.y - p1.y)) / d;

            float x4_1 = x3 + (h * (p2.y - p1.y)) / d;
            float y4_1 = y3 - (h * (p2.x - p1.x)) / d;
            Vector2 crossPoint1 = new Vector2(x4_1, y4_1);

            float x4_2 = x3 - (h * (p2.y - p1.y)) / d;
            float y4_2 = y3 + (h * (p2.x - p1.x)) / d;
            Vector2 crossPoint2 = new Vector2(x4_2, y4_2);

            float dir1 = (p2.x - p1.x) * (crossPoint1.y - p1.y) - (p2.y - p1.y) * (crossPoint1.x - p1.x);

            if (dir1 > 0.0)
            {
                if (left)
                    return crossPoint1;
                else
                    return crossPoint2;
            }
            else
            {
                if (left)
                    return crossPoint2;
                else
                    return crossPoint1;
            }
        }


    }
}
