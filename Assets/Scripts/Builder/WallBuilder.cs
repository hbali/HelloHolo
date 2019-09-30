using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model.Builders
{

    public static class WallBuilder
    {

        private static Vector3 sortTo;


        private class OpeningComparer : IComparer<Opening>
        {
            public int Compare(Opening x, Opening y)
            {
                double dist1 = Vector3.Distance(x.Start.Point, sortTo);
                double dist2 = Vector3.Distance(y.Start.Point, sortTo);
                return dist1.CompareTo(dist2);
            }
        }
        public static Mesh[] MeshesForWall(this Wall wall)
        {
            sortTo = wall.Start.Point;

            float angle = GeometryExtensions.GetAngleRadian(wall.Start.Point.ToVector2(), wall.End.Point.ToVector2());
            Vector3 frontStart = wall.Start.Point;
            Vector3 frontEnd = wall.End.Point;

            Vector2 outerStart = GeometryExtensions.Polar2Cartesian(wall.Start.Point.ToVector2(), angle - Mathf.PI / 2, wall.Thickness);
            Vector2 outerEnd = GeometryExtensions.Polar2Cartesian(wall.End.Point.ToVector2(), angle - Mathf.PI / 2, wall.Thickness);
            Vector3 backStart = new Vector3(outerStart.x, wall.Start.Point.y, outerStart.y);
            Vector3 backEnd = new Vector3(outerEnd.x, wall.End.Point.y, outerEnd.y);

            if (wall.Openings.Count == 0)
            {
               
                return new Mesh[] { WallMesh.GenerateWallMesh(frontStart, frontEnd, backStart, backEnd, wall.Height) };
            }
            else
            {
                int openingsLength = wall.Openings.Count;
                string[] tagsForMeshes = new string[1 + openingsLength * 3];

                Mesh[] meshes = new Mesh[1 + openingsLength * 3];
                var openings = wall.Openings.ToArray();
                Array.Sort(openings, new OpeningComparer());

                //miért az elsõ opening méreteit vesszük alapul és nem ciklusban mindig a sajátját

                float length = Vector3.Distance(openings[0].Start.Point, openings[0].End.Point);
                float ll = Mathf.Sqrt(wall.Thickness * wall.Thickness + length * length);

                Vector2 openingStart = new Vector2(openings[0].Start.Point.x, openings[0].Start.Point.z);
                Vector2 openingEnd = new Vector2(openings[0].End.Point.x, openings[0].End.Point.z);

                Vector2 outerOpeningStart = GeometryExtensions.GetPointFrom2Points2Distances(openingStart, wall.Thickness, openingEnd, ll, false);
                Vector2 outerOpeningEnd = GeometryExtensions.GetPointFrom2Points2Distances(openingEnd, wall.Thickness, openingStart, ll, false);

                meshes[0] = WallMesh.GenerateWallMesh(
                    frontStart,
                    new Vector3(openingStart.x, frontStart.y, openingStart.y),
                    backStart,
                    new Vector3(outerOpeningStart.x, frontStart.y, outerOpeningStart.y),
                    wall.Height
                );
                tagsForMeshes[0] = "fullWallSegment";

                for (int i = 0; i < openingsLength; i++)
                {
                    length = Vector3.Distance(openings[i].Start.Point, openings[i].End.Point);
                    ll = Mathf.Sqrt(wall.Thickness * wall.Thickness + length * length);

                    openingStart = new Vector2(openings[i].Start.Point.x, openings[i].Start.Point.z);
                    openingEnd = new Vector2(openings[i].End.Point.x, openings[i].End.Point.z);

                    outerOpeningStart = GeometryExtensions.GetPointFrom2Points2Distances(openingStart, wall.Thickness, openingEnd, ll, false);
                    outerOpeningEnd = GeometryExtensions.GetPointFrom2Points2Distances(openingEnd, wall.Thickness, openingStart, ll, true);

                    //Debug.Log(i + ". opening start:  " + openings[i].Start.Position.x + ", " + openings[i].Start.Position.y + ", " + openings[i].Start.Position.z);	
                    meshes[1 + i * 3] = WallMesh.GenerateWallMesh(
                        new Vector3(openingStart.x, frontStart.y, openingStart.y),
                        new Vector3(openingEnd.x, frontStart.y, openingEnd.y),
                        new Vector3(outerOpeningStart.x, frontStart.y, outerOpeningStart.y),
                        new Vector3(outerOpeningEnd.x, frontStart.y, outerOpeningEnd.y),
                        openings[i].Sill
                    );
                    tagsForMeshes[1 + i * 3] = "partialWallSegment";

                    meshes[2 + i * 3] = WallMesh.GenerateWallMesh(
                        new Vector3(openingStart.x, frontStart.y + openings[i].Height + openings[i].Sill, openingStart.y),
                        new Vector3(openingEnd.x, frontStart.y + openings[i].Height + openings[i].Sill, openingEnd.y),
                        new Vector3(outerOpeningStart.x, frontStart.y + openings[i].Height + openings[i].Sill, outerOpeningStart.y),
                        new Vector3(outerOpeningEnd.x, frontStart.y + openings[i].Height + openings[i].Sill, outerOpeningEnd.y),
                        wall.Height - (openings[i].Sill + openings[i].Height)
                    );
                    tagsForMeshes[2 + i * 3] = "partialWallSegment";

                    if (i < openingsLength - 1)
                    {
                        float lengthn = Vector3.Distance(openings[i + 1].Start.Point, openings[i + 1].End.Point);
                        float lln = Mathf.Sqrt(wall.Thickness * wall.Thickness + lengthn * lengthn);

                        Vector2 openingStartn = new Vector2(openings[i + 1].Start.Point.x, openings[i + 1].Start.Point.z);
                        Vector2 openingEndn = new Vector2(openings[i + 1].End.Point.x, openings[i + 1].End.Point.z);

                        Vector2 outerOpeningStartn = GeometryExtensions.GetPointFrom2Points2Distances(openingStartn, wall.Thickness, openingEndn, lln, true);

                        meshes[3 + i * 3] = WallMesh.GenerateWallMesh(
                            new Vector3(openingEnd.x, frontStart.y, openingEnd.y),
                            new Vector3(openingStartn.x, frontStart.y, openingStartn.y),
                            new Vector3(outerOpeningEnd.x, frontStart.y, outerOpeningEnd.y),
                            new Vector3(outerOpeningStartn.x, frontStart.y, outerOpeningStartn.y),
                            wall.Height
                        );
                        tagsForMeshes[3 + i * 3] = "fullWallSegment";
                    }
                }

                float lengthlast = Vector3.Distance(openings[openingsLength - 1].Start.Point, openings[openingsLength - 1].End.Point);
                float lllast = Mathf.Sqrt(wall.Thickness * wall.Thickness + lengthlast * lengthlast);

                Vector2 openingStartLast = new Vector2(openings[openingsLength - 1].Start.Point.x, openings[openingsLength - 1].Start.Point.z);
                Vector2 openingEndLast = new Vector2(openings[openingsLength - 1].End.Point.x, openings[openingsLength - 1].End.Point.z);

                Vector2 outerOpeningEndLast = GeometryExtensions.GetPointFrom2Points2Distances(openingEndLast, wall.Thickness, openingStartLast, lllast, true);

                meshes[openingsLength * 3] = WallMesh.GenerateWallMesh(
                    new Vector3(openingEndLast.x, frontStart.y, openingEndLast.y),
                    frontEnd,
                    new Vector3(outerOpeningEndLast.x, frontStart.y, outerOpeningEndLast.y),
                    backEnd,
                    wall.Height
                );
                tagsForMeshes[openingsLength * 3] = "fullWallSegment";
                return meshes;
            }
        }

        private static Vector3 GetExtendedPos(Vector3 p, Vector3 dir, float dist)
        {
            return p + dir * dist;
        }

        /// <summary>
        /// Returns whether the extended point is closer or farther from the checkP
        /// Normal calculation : -y, x
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="checkP"></param>
        /// <returns></returns>
        public static bool NormalCloserToPoint(Vector3 p1, Vector3 p2, Vector3 checkP)
        {
            Vector3 dir = p1 - p2;
            Vector3 normal = new Vector3(-dir.y, dir.x);
            Vector3 extended = GetExtendedPos(p1, new Vector3(normal.x, 0, normal.y), 1);
            return Vector3.Distance(p1, checkP) < Vector3.Distance(extended, checkP);
        }
    }
}
