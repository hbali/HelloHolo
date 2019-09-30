﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.SpatialMapping;
using Assets.Scripts;

#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#else
using UnityEngine.VR.WSA;
#endif
#endif

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Handles the custom meshes generated by the understanding DLL. The meshes
    /// are generated during the scanning phase and once more on scan finalization.
    /// The meshes can be used to visualize the scanning progress.
    /// </summary>
    public class SpatialUnderstandingCustomMesh : SpatialMappingSource
    {
        // Config
        [Tooltip("Indicate the time in seconds between mesh imports, during the scanning phase. A value of zero will disable pulling meshes from the DLL")]
        public float ImportMeshPeriod = 1.0f;

        [SerializeField]
        [Tooltip("Material used to render the custom mesh generated by the DLL")]
        private Material meshMaterial;

        public Material MeshMaterial
        {
            get
            {
                return meshMaterial;
            }
            set
            {
                meshMaterial = value;

                if (spatialUnderstanding.ScanState == SpatialUnderstanding.ScanStates.Done)
                {
                    for (int i = 0; i < SurfaceObjects.Count; ++i)
                    {
                        SurfaceObjects[i].Renderer.sharedMaterial = meshMaterial;
                    }
                }
            }
        }

        /// <summary>
        /// Used to keep our processing from exceeding our frame budget.
        /// </summary>
        [Tooltip("Max time per frame in milliseconds to spend processing the mesh")]
        public float MaxFrameTime = 5.0f;
        private float MaxFrameTimeInSeconds
        {
            get { return (MaxFrameTime / 1000); }
        }

        /// <summary>
        ///  Whether to create mesh colliders. If unchecked, mesh colliders will be empty and disabled.
        /// </summary>
        public bool CreateMeshColliders = true;

        private bool drawProcessedMesh = true;

        // Properties
        /// <summary>
        /// Controls rendering of the mesh. This can be set by the user to hide or show the mesh.
        /// </summary>
        public bool DrawProcessedMesh
        {
            get
            {
                return drawProcessedMesh;
            }
            set
            {
                drawProcessedMesh = value;
                for (int i = 0; i < SurfaceObjects.Count; ++i)
                {
                    SurfaceObjects[i].Renderer.enabled = drawProcessedMesh;
                }
            }
        }

        /// <summary>
        /// Have any mesh sectors been scanned
        /// </summary>
        public bool HasMeshSectors { get { return meshSectors != null && meshSectors.Count > 0; } }

        /// <summary>
        /// Indicates if the previous import is still being processed.
        /// </summary>
        public bool IsImportActive { get; private set; }

        protected override int SurfaceObjectLayer { get { return (int)LayerEnums.SpatialUnderstanding; } }


        /// <summary>
        /// The material to use if rendering.
        /// </summary>
        protected override Material RenderMaterial { get { return MeshMaterial; } }

        /// <summary>
        /// To prevent us from importing too often, we keep track of the last import.
        /// </summary>
        private float timeLastImportedMesh = 0;

        /// <summary>
        /// For a cached SpatialUnderstanding.Instance.
        /// </summary>
        private SpatialUnderstanding spatialUnderstanding;

        /// <summary>
        /// The spatial understanding mesh will be split into pieces so that we don't have to
        /// render the whole mesh, rather just the parts that are visible to the user.
        /// </summary>
        private Dictionary<Vector3, MeshData> meshSectors = new Dictionary<Vector3, MeshData>();

        /// <summary>
        /// A data structure to manage collecting triangles as we
        /// subdivide the spatial understanding mesh into smaller sub meshes.
        /// </summary>
        private class MeshData
        {
            /// <summary>
            /// Lists of vertices/triangles that describe the mesh geometry.
            /// </summary>
            private readonly List<Vector3> verts = new List<Vector3>();
            private readonly List<int> tris = new List<int>();

            /// <summary>
            /// The mesh object based on the triangles passed in.
            /// </summary>
            public readonly Mesh MeshObject = new Mesh();

            /// <summary>
            /// The MeshCollider with which this mesh is associated. Must be set even if
            /// no collision mesh will be created.
            /// </summary>
            public MeshCollider SpatialCollider = null;

            /// <summary>
            /// Whether to create collision mesh. If false, the MeshCollider attached to this
            /// object will also be disabled when Commit() is called.
            /// </summary>
            public bool CreateMeshCollider = false;

            /// <summary>
            /// Clears the geometry, but does not clear the mesh.
            /// </summary>
            public void Reset()
            {
                verts.Clear();
                tris.Clear();
            }

            /// <summary>
            /// Commits the new geometry to the mesh.
            /// </summary>
            public void Commit()
            {
                MeshObject.Clear();
                if (verts.Count > 2)
                {
                    MeshObject.SetVertices(verts);
                    MeshObject.SetTriangles(tris, 0);
                    MeshObject.RecalculateNormals();
                    MeshObject.RecalculateBounds();
                    // The null assignment is required by Unity in order to pick up the new mesh
                    SpatialCollider.sharedMesh = null;
                    if (CreateMeshCollider)
                    {
                        SpatialCollider.sharedMesh = MeshObject;
                        SpatialCollider.enabled = true;
                    }
                    else
                    {
                        SpatialCollider.enabled = false;
                    }
                }
            }

            /// <summary>
            /// Adds a triangle composed of the specified three points to our mesh.
            /// </summary>
            /// <param name="point1">First point on the triangle.</param>
            /// <param name="point2">Second point on the triangle.</param>
            /// <param name="point3">Third point on the triangle.</param>
            public void AddTriangle(Vector3 point1, Vector3 point2, Vector3 point3)
            {
                // Currently spatial understanding in the native layer voxellizes the space
                // into ~2000 voxels per cubic meter.  Even in a degenerate case we
                // will use far fewer than 65000 vertices, this check should not fail
                // unless the spatial understanding native layer is updated to have more
                // voxels per cubic meter.
                if (verts.Count < 65000)
                {
                    tris.Add(verts.Count);
                    verts.Add(point1);

                    tris.Add(verts.Count);
                    verts.Add(point2);

                    tris.Add(verts.Count);
                    verts.Add(point3);
                }
                else
                {
                    Debug.LogError("Mesh would have more vertices than Unity supports");
                }
            }
        }

        private void Start()
        {
            
            spatialUnderstanding = SpatialUnderstanding.Instance;
#if UNITY_WSA
            if (gameObject.GetComponent<WorldAnchor>() == null)
            {
                gameObject.AddComponent<WorldAnchor>();
            }
#endif
        }

        private void Update()
        {
            Update_MeshImport();
        }

        /// <summary>
        /// Adds a triangle with the specified points to the specified sector.
        /// </summary>
        /// <param name="sector">The sector to add the triangle to.</param>
        /// <param name="point1">First point of the triangle.</param>
        /// <param name="point2">Second point of the triangle.</param>
        /// <param name="point3">Third point of the triangle.</param>
        private void AddTriangleToSector(Vector3 sector, Vector3 point1, Vector3 point2, Vector3 point3)
        {
            // Grab the mesh container we are using for this sector.
            MeshData nextSectorData;
            if (!meshSectors.TryGetValue(sector, out nextSectorData))
            {
                nextSectorData = new MeshData();
                nextSectorData.CreateMeshCollider = CreateMeshColliders;

                int surfaceObjectIndex = SurfaceObjects.Count;

                SurfaceObject surfaceObject = CreateSurfaceObject(
                  mesh: nextSectorData.MeshObject,
                  objectName: string.Format("SurfaceUnderstanding Mesh-{0}", surfaceObjectIndex),
                  parentObject: transform,
                  meshID: surfaceObjectIndex,
                  drawVisualMeshesOverride: DrawProcessedMesh);                

                nextSectorData.SpatialCollider = surfaceObject.Collider;

                AddSurfaceObject(surfaceObject);

                // Or make it if this is a new sector.
                meshSectors.Add(sector, nextSectorData);
            }

            // Add the vertices to the sector's mesh container.
            nextSectorData.AddTriangle(point1, point2, point3);
        }

        /// <summary>
        /// Imports the custom mesh from the DLL. This a a coroutine which will take multiple frames to complete.
        /// </summary>
        /// <returns></returns>
        public IEnumerator Import_UnderstandingMesh()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            int startFrameCount = Time.frameCount;

            if (!spatialUnderstanding.AllowSpatialUnderstanding || IsImportActive)
            {
                yield break;
            }

            IsImportActive = true;

            SpatialUnderstandingDll dll = spatialUnderstanding.UnderstandingDLL;

            Vector3[] meshVertices = null;
            Vector3[] meshNormals = null;
            Int32[] meshIndices = null;

            // Pull the mesh - first get the size, then allocate and pull the data
            int vertCount;
            int idxCount;

            if ((SpatialUnderstandingDll.Imports.GeneratePlayspace_ExtractMesh_Setup(out vertCount, out idxCount) > 0) &&
                (vertCount > 0) &&
                (idxCount > 0))
            {
                meshVertices = new Vector3[vertCount];
                IntPtr vertPos = dll.PinObject(meshVertices);
                meshNormals = new Vector3[vertCount];
                IntPtr vertNorm = dll.PinObject(meshNormals);
                meshIndices = new Int32[idxCount];
                IntPtr indices = dll.PinObject(meshIndices);

                SpatialUnderstandingDll.Imports.GeneratePlayspace_ExtractMesh_Extract(vertCount, vertPos, vertNorm, idxCount, indices);
            }

            // Wait a frame
            stopwatch.Stop();
            yield return null;
            stopwatch.Start();

            // Create output meshes
            if ((meshVertices != null) &&
                (meshVertices.Length > 0) &&
                (meshIndices != null) &&
                (meshIndices.Length > 0))
            {
                // first get all our mesh data containers ready for meshes.
                foreach (MeshData meshdata in meshSectors.Values)
                {
                    meshdata.Reset();
                }

                float startTime = Time.realtimeSinceStartup;
                // first we need to split the playspace up into segments so we don't always
                // draw everything.  We can break things up in to cubic meters.
                for (int index = 0; index < meshIndices.Length; index += 3)
                {
                    Vector3 firstVertex = meshVertices[meshIndices[index]];
                    Vector3 secondVertex = meshVertices[meshIndices[index + 1]];
                    Vector3 thirdVertex = meshVertices[meshIndices[index + 2]];

                    // The triangle may belong to multiple sectors.  We will copy the whole triangle
                    // to all of the sectors it belongs to.  This will fill in seams on sector edges
                    // although it could cause some amount of visible z-fighting if rendering a wireframe.
                    Vector3 firstSector = VectorToSector(firstVertex);

                    AddTriangleToSector(firstSector, firstVertex, secondVertex, thirdVertex);

                    // If the second sector doesn't match the first, copy the triangle to the second sector.
                    Vector3 secondSector = VectorToSector(secondVertex);
                    if (secondSector != firstSector)
                    {
                        AddTriangleToSector(secondSector, firstVertex, secondVertex, thirdVertex);
                    }

                    // If the third sector matches neither the first nor second sector, copy the triangle to the
                    // third sector.
                    Vector3 thirdSector = VectorToSector(thirdVertex);
                    if (thirdSector != firstSector && thirdSector != secondSector)
                    {
                        AddTriangleToSector(thirdSector, firstVertex, secondVertex, thirdVertex);
                    }

                    // Limit our run time so that we don't cause too many frame drops.
                    // Only checking every few iterations or so to prevent losing too much time to checking the clock.
                    if ((index % 30 == 0) && ((Time.realtimeSinceStartup - startTime) > MaxFrameTimeInSeconds))
                    {
                        //  Debug.LogFormat("{0} of {1} processed", index, meshIndices.Length);
                        stopwatch.Stop();
                        yield return null;
                        stopwatch.Start();
                        startTime = Time.realtimeSinceStartup;
                    }
                }

                startTime = Time.realtimeSinceStartup;

                // Now we have all of our triangles assigned to the correct mesh, we can make all of the meshes.
                // Each sector will have its own mesh.
                foreach (MeshData meshData in meshSectors.Values)
                {
                    // Construct the mesh.
                    meshData.Commit();

                    // Make sure we don't build too many meshes in a single frame.
                    if ((Time.realtimeSinceStartup - startTime) > MaxFrameTimeInSeconds)
                    {
                        stopwatch.Stop();
                        yield return null;
                        stopwatch.Start();
                        startTime = Time.realtimeSinceStartup;
                    }
                }
            }

            // Wait a frame
            stopwatch.Stop();
            yield return null;
            stopwatch.Start();

            // All done - can free up marshal pinned memory
            dll.UnpinAllObjects();

            // Done
            IsImportActive = false;

            // Mark the timestamp
            timeLastImportedMesh = Time.time;

            stopwatch.Stop();
            int deltaFrameCount = (Time.frameCount - startFrameCount + 1);

            if (stopwatch.Elapsed.TotalSeconds > 0.75)
            {
                Debug.LogWarningFormat("Import_UnderstandingMesh took {0:N0} frames ({1:N3} ms)",
                    deltaFrameCount,
                    stopwatch.Elapsed.TotalMilliseconds
                    );
            }
        }

        /// <summary>
        /// Basically floors the Vector so we can use it to subdivide our spatial understanding mesh into parts based
        /// on their position in the world.
        /// </summary>
        /// <param name="vector">The vector to floor.</param>
        /// <returns>A floored vector</returns>
        private Vector3 VectorToSector(Vector3 vector)
        {
            return new Vector3(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.z));
        }

        /// <summary>
        /// Updates the mesh import process. This function will kick off the import
        /// coroutine at the requested internal.
        /// </summary>
        private void Update_MeshImport()
        {
            // Only update every so often
            if (IsImportActive || (ImportMeshPeriod <= 0.0f) ||
                ((Time.time - timeLastImportedMesh) < ImportMeshPeriod) ||
                (spatialUnderstanding.ScanState != SpatialUnderstanding.ScanStates.Scanning))
            {
                return;
            }

            StartCoroutine(Import_UnderstandingMesh());
        }

        private void OnDestroy()
        {
            Cleanup();
        }
    }
}
