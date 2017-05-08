//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HUX.Utility;
using HoloToolkit.Unity.SpatialMapping;

#if UNITY_WSA
using UnityEngine.VR.WSA;
#endif

#if UNITY_WSA && !UNITY_EDITOR
using System.Threading;
using System.Threading.Tasks;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HUX.Spatial
{
    /// <summary>
    /// SurfaceMeshesToPlanes will find and create planes based on the meshes returned by the SpatialMappingManager's Observer.
    /// </summary>
    public class SurfacePlaneManager : HoloToolkit.Unity.Singleton<SurfacePlaneManager>
    {

#region Editor Variables
		[Header("Plane Generation")]
		/// <summary>
		/// Minimum area required for a plane to be created.
		/// </summary>
		[SerializeField, Tooltip("Minimum area required for a plane to be created.")]
		private float m_MinArea = 0.025f;

		/// <summary>
		/// Threshold for acceptable normals (the closer to 1, the stricter the standard). Used when determining plane type.
		/// </summary>
		[SerializeField, Range(0.0f, 1.0f), Tooltip("Threshold for acceptable normals (the closer to 1, the stricter the standard). Used when determining plane type.")]
		private float m_UpNormalThreshold = 0.9f;

		/// <summary>
		/// The thickness to create the planes at.
		/// </summary>
		[SerializeField, Range(0.0f, 1.0f), Tooltip("Thickness to make each plane.")]
		private float m_PlaneThickness = 0.01f;

		/// <summary>
		/// Buffer to use when determining if a horizontal plane near the floor should be considered part of the floor.
		/// </summary>
		[SerializeField, Range(0.0f, 1.0f), Tooltip("Buffer to use when determining if a horizontal plane near the floor should be considered part of the floor.")]
		private float m_FloorBuffer = 0.1f;

		/// <summary>
		/// Buffer to use when determining if a horizontal plane near the ceiling should be considered part of the ceiling.
		/// </summary>
		[SerializeField, Range(0.0f, 1.0f), Tooltip("Buffer to use when determining if a horizontal plane near the ceiling should be considered part of the ceiling.")]
		private float m_CeilingBuffer = 0.1f;

		/// <summary>
		/// The maximum percentage different a plane can have before it will no longer be considered as possibly the same plane.
		/// </summary>
		[SerializeField, Range(0.0f, 1.0f), Tooltip("The maximum percentage different a surface can have before it will no longer be considered as possibly the same plane.")]
		private float m_MaxAreaDiffPercent = 0.2f;

		/// <summary>
		/// The maximum distance a plane can be from another surface before it will no longer be considered as possibly the same plane.
		/// </summary>
		[SerializeField, Tooltip("The maximum distance a plane can be from another surface before it will no longer be considered as possibly the same plane.")]
		private float m_MaxDistChange = 0.15f;

		/// <summary>
		/// Determines which plane types should be discarded.
		/// Use this when the spatial mapping mesh is a better fit for the surface (ex: round tables).
		/// </summary>
		[SerializeField, HideInInspector]
		private PlaneTypes m_DestroyPlanesMask = PlaneTypes.Unknown;

		[Header("Plane Rendering")]
		/// <summary>
		/// Toggle for turn the drawing of the planes on and off.
		/// </summary>
		[SerializeField]
		private bool m_ShowPlanes = true;

		/// <summary>
		/// Determines which plane types should be rendered.
		/// </summary>
		[SerializeField, HideInInspector]
		private PlaneTypes m_DrawPlanesMask = (PlaneTypes.Wall | PlaneTypes.Floor | PlaneTypes.Ceiling | PlaneTypes.Table);

		/// <summary>
		/// The Material to use for Wall Planes.
		/// </summary>
		[SerializeField]
		private Material m_WallMaterial;

		/// <summary>
		/// The Material to use for Floor planes.
		/// </summary>
		[SerializeField]
		private Material m_FloorMaterial;

		/// <summary>
		/// The Material to use for Ceiling planes.
		/// </summary>
		[SerializeField]
		private Material m_CeilingMaterial;

		/// <summary>
		/// The Material to use for Table planes.
		/// </summary>
		[SerializeField]
		private Material m_TableMaterial;

        #endregion

        //-----------------------------------------------------------------------------------------------

        #region Event Handlers
        /// <summary>
        /// Delegate which is called when the MakePlanesCompleted event is triggered.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        public delegate void EventHandler(object source, EventArgs args);

        /// <summary>
        /// EventHandler which is triggered when the MakePlanesRoutine is finished.
        /// </summary>
        public event EventHandler MakePlanesComplete;
#endregion

		//-----------------------------------------------------------------------------------------------

#region Private Variables
		/// <summary>
		/// All of the currently active planes.
		/// </summary>
		private List<SurfacePlane> m_ActivePlanes = new List<SurfacePlane>();

		/// <summary>
		/// Searchable tree of the current wall planes.
		/// </summary>
		private BoundedPlaneKDTree<SurfacePlane> m_WallPlanes = new BoundedPlaneKDTree<SurfacePlane>();

		/// <summary>
		/// Searchable tree of the current floor planes.
		/// </summary>
		private BoundedPlaneKDTree<SurfacePlane> m_FloorPlanes = new BoundedPlaneKDTree<SurfacePlane>();

		/// <summary>
		/// Searchable tree of the current table planes.
		/// </summary>
		private BoundedPlaneKDTree<SurfacePlane> m_TablePlanes = new BoundedPlaneKDTree<SurfacePlane>();

		/// <summary>
		/// Searchable tree of the current ceiling planes.
		/// </summary>
		private BoundedPlaneKDTree<SurfacePlane> m_CeilingPlanes = new BoundedPlaneKDTree<SurfacePlane>();

		/// <summary>
		/// Empty game object used to contain all planes created by the SurfaceToPlanes class.
		/// </summary>
		private GameObject planesParent;

        /// <summary>
        /// Used to align planes with gravity so that they appear more level.
        /// </summary>
        private float snapToGravityThreshold = 5.0f;

		/// <summary>
		/// The current plane id to assign to the next created plane.
		/// </summary>
		private int m_PlaneId = 1;

        private SpatialMappingObserver m_MappingObserver = null;


#if UNITY_EDITOR
        /// <summary>
        /// How much time (in sec), while running in the Unity Editor, to allow RemoveSurfaceVertices to consume before returning control to the main program.
        /// </summary>
        private static readonly float FrameTime = .016f;
#else
        /// <summary>
        /// How much time (in sec) to allow RemoveSurfaceVertices to consume before returning control to the main program.
        /// </summary>
        private static readonly float FrameTime = .008f;
#endif

#endregion

		//-----------------------------------------------------------------------------------------------

#region Accessors

		/// <summary>
		/// Floor y value, which corresponds to the maximum horizontal area found below the user's head position.
		/// This value is reset by SurfaceMeshesToPlanes when the max floor plane has been found.
		/// </summary>
		public float FloorYPosition { get; private set; }

		/// <summary>
		/// Ceiling y value, which corresponds to the maximum horizontal area found above the user's head position.
		/// This value is reset by SurfaceMeshesToPlanes when the max ceiling plane has been found.
		/// </summary>
		public float CeilingYPosition { get; private set; }

		/// <summary>
		/// The minimum threshold for being considered pointing up.
		/// </summary>
		public float UpNormalThreshold
		{
			get
			{
				return m_UpNormalThreshold;
			}
		}

		/// <summary>
		/// If true the planes set to draw will be shown, otherwise no plane will be shown.
		/// </summary>
		public bool ShowPlanes
		{
			get
			{
				return m_ShowPlanes;
			}

			set
			{
				m_ShowPlanes = value;
				foreach (SurfacePlane plane in m_ActivePlanes)
				{
					SetPlaneVisibility(plane);
				}
			}
		}

		/// <summary>
		/// Indicates if SurfaceToPlanes is currently creating planes based on the Spatial Mapping Mesh.
		/// </summary>
		public bool MakingPlanes
		{
			get; set;
		}

#endregion

		//-----------------------------------------------------------------------------------------------

#region MonoBehaviour Functions

		/// <summary>
		/// Standard Start function.
		/// </summary>
		private void Start()
        {
			MakingPlanes = false;
            planesParent = new GameObject("SurfacePlanes");
            planesParent.transform.position = Vector3.zero;
            planesParent.transform.rotation = Quaternion.identity;

            m_MappingObserver = SpatialMappingManager.Instance.Source as SpatialMappingObserver;
        }

        private void OnEnable()
        {
            StartCoroutine(RebuildPlanesFromMeshCoroutine());
        }

        #endregion

        //-----------------------------------------------------------------------------------------------

        #region Public Static Functions
        /// <summary>
        /// Gets the possible types a plane might be.  This does not take into account the current floor or ceiling height.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="upNormalThreshold"></param>
        /// <returns></returns>
        public static PlaneTypes GetPossibleType(BoundedPlane bounds, float upNormalThreshold = 0.9f)
		{
			PlaneTypes type;

			Vector3 surfaceNormal = bounds.Plane.normal;
			// Determine what type of plane this is.
			// Use the upNormalThreshold to help determine if we have a horizontal or vertical surface.
			if (surfaceNormal.y >= upNormalThreshold)
			{
				type = PlaneTypes.Floor | PlaneTypes.Table;
			}
			else if (surfaceNormal.y <= -(upNormalThreshold))
			{
				type = PlaneTypes.Ceiling | PlaneTypes.Table;
			}
			else if (Mathf.Abs(surfaceNormal.y) <= (1 - upNormalThreshold))
			{
				// If the plane is vertical, then classify it as a wall.
				type = PlaneTypes.Wall;
			}
			else
			{
				// The plane has a strange angle, classify it as 'unknown'.
				type = PlaneTypes.Unknown;
			}

			return type;
		}
        #endregion

        //-----------------------------------------------------------------------------------------------

        #region Public Functions

        /// <summary>
        /// Coroutine for RebuildPlanesFromMesh.
        /// </summary>
        /// <param name="meshes">List of meshes to use to build planes.</param>
        public IEnumerator RebuildPlanesFromMeshCoroutine()
        {
            yield return new WaitForSeconds(1.0f);

            while (isActiveAndEnabled)
            {
                if (m_MappingObserver != null)
                {
                    List<MeshFilter> meshFilters = m_MappingObserver.GetMeshFilters();
                    var convertedMeshes = new List<PlaneFinding.MeshData>(meshFilters.Count);

                    for (int i = 0; i < meshFilters.Count; i++)
                    {
                        convertedMeshes.Add(new PlaneFinding.MeshData(meshFilters[i]));
                    }

                    if (convertedMeshes.Count > 0)
                    {
                        yield return MakePlanesRoutine(convertedMeshes);
                    }

                    yield return new WaitForSeconds(2.0f);
                }
            }
        }

        /// <summary>
        /// Returns all currently active planes.
        /// </summary>
        /// <returns></returns>
        public List<SurfacePlane> GetActivePlanes()
		{
			return new List<SurfacePlane>(m_ActivePlanes);
		}

		/// <summary>
		/// Gets all active planes of the specified type(s).
		/// </summary>
		/// <param name="planeTypes">A flag which includes all plane type(s) that should be returned.</param>
		/// <returns>A collection of planes that match the expected type(s).</returns>
		public List<SurfacePlane> GetActivePlanes(PlaneTypes planeTypes)
		{
			List<SurfacePlane> typePlanes = new List<SurfacePlane>();

			foreach (SurfacePlane plane in m_ActivePlanes)
			{
				if ((planeTypes & plane.PlaneType) == plane.PlaneType)
				{
					typePlanes.Add(plane);
				}
			}

			return typePlanes;
		}

		/// <summary>
		/// Gets the closest plane to a provided world position of one of the types defined by validTypes
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="validTypes"></param>
		/// <returns></returns>
		public SurfacePlane GetClosestPlane(Vector3 pos, PlaneTypes validTypes)
		{
			SurfacePlane closestPlane = null;
			List<SurfacePlane> possiblePlanes = new List<SurfacePlane>();

			if ((validTypes & PlaneTypes.Ceiling) == PlaneTypes.Ceiling)
			{
				SurfacePlane possiblePlane = null;
				BoundedPlane possibleBounds = new BoundedPlane();
				if (m_CeilingPlanes.FindClosestBoundedPlane(pos, out possibleBounds, out possiblePlane))
				{
					possiblePlanes.Add(possiblePlane);
				}
			}

			if ((validTypes & PlaneTypes.Floor) == PlaneTypes.Floor)
			{
				SurfacePlane possiblePlane = null;
				BoundedPlane possibleBounds = new BoundedPlane();
				if (m_FloorPlanes.FindClosestBoundedPlane(pos, out possibleBounds, out possiblePlane))
				{
					possiblePlanes.Add(possiblePlane);
				}
			}

			if ((validTypes & PlaneTypes.Table) == PlaneTypes.Table)
			{
				SurfacePlane possiblePlane = null;
				BoundedPlane possibleBounds = new BoundedPlane();
				if (m_TablePlanes.FindClosestBoundedPlane(pos, out possibleBounds, out possiblePlane))
				{
					possiblePlanes.Add(possiblePlane);
				}
			}

			if ((validTypes & PlaneTypes.Wall) == PlaneTypes.Wall)
			{
				SurfacePlane possiblePlane = null;
				BoundedPlane possibleBounds = new BoundedPlane();
				if (m_WallPlanes.FindClosestBoundedPlane(pos, out possibleBounds, out possiblePlane))
				{
					possiblePlanes.Add(possiblePlane);
				}
			}


			float closestDist = float.MaxValue;
			//Of the possible planes figure out which is closest.
			foreach (SurfacePlane possiblePlane in possiblePlanes)
			{
				if ((possiblePlane.PlaneType & validTypes) == possiblePlane.PlaneType)
				{
					float dist = possiblePlane.Plane.GetSqrDistance(pos);
					if (closestPlane == null || dist < closestDist)
					{
						closestDist = dist;
						closestPlane = possiblePlane;
					}
				}
			}

			if (closestPlane != null)
			{
				float distanceToPlane = closestPlane.Plane.Plane.GetDistanceToPoint(pos);
				Vector3 worldPosOnPlane = pos - closestPlane.Plane.Plane.normal * distanceToPlane;

				Debug.DrawLine(pos, worldPosOnPlane, Color.red, 15);
				Debug.DrawLine(pos, closestPlane.Plane.GetClosestWorldPoint(pos), Color.green, 15);
			}

			return closestPlane;
		}

		/// <summary>
		/// Classifies the surface as a floor, wall, ceiling, table, etc.
		/// </summary>
		public PlaneTypes GetPlaneType(BoundedPlane plane)
		{
			Vector3 surfaceNormal = plane.Plane.normal;
			PlaneTypes planeType = PlaneTypes.Unknown;

			// Determine what type of plane this is.
			// Use the upNormalThreshold to help determine if we have a horizontal or vertical surface.
			if (surfaceNormal.y >= UpNormalThreshold)
			{
				// If we have a horizontal surface with a normal pointing up, classify it as a floor.
				planeType = PlaneTypes.Floor;

				if (plane.Bounds.Center.y > (FloorYPosition + m_FloorBuffer))
				{
					// If the plane is too high to be considered part of the floor, classify it as a table.
					planeType = PlaneTypes.Table;
				}
			}
			else if (surfaceNormal.y <= -(UpNormalThreshold))
			{
				// If we have a horizontal surface with a normal pointing down, classify it as a ceiling.
				planeType = PlaneTypes.Ceiling;

				if (plane.Bounds.Center.y < (CeilingYPosition - m_CeilingBuffer))
				{
					// If the plane is not high enough to be considered part of the ceiling, classify it as a table.
					planeType = PlaneTypes.Table;
				}
			}
			else if (Mathf.Abs(surfaceNormal.y) <= (1 - UpNormalThreshold))
			{
				// If the plane is vertical, then classify it as a wall.
				planeType = PlaneTypes.Wall;
			}
			else
			{
				// The plane has a strange angle, classify it as 'unknown'.
				planeType = PlaneTypes.Unknown;
			}

			return planeType;
		}
#endregion

		//-----------------------------------------------------------------------------------------------

#region Private Functions

        /// <summary>
        /// Iterator block, analyzes surface meshes to find planes and create new 3D cubes to represent each plane.
        /// </summary>
        /// <returns>Yield result.</returns>
        private IEnumerator MakePlanesRoutine(List<HoloToolkit.Unity.SpatialMapping.PlaneFinding.MeshData> meshData)
        {
			MakingPlanes = true;
#if UNITY_WSA && !UNITY_EDITOR
            // When not in the unity editor we can use a cool background task to help manage FindPlanes().
            Task<BoundedPlane[]> planeTask = Task.Run(() => PlaneFinding.FindPlanes(meshData, snapToGravityThreshold, m_MinArea));
        
            while (planeTask.IsCompleted == false)
            {
                yield return null;
            }

            BoundedPlane[] planes = planeTask.Result;
#else
			// In the unity editor, the task class isn't available, but perf is usually good, so we'll just wait for FindPlanes to complete.
			BoundedPlane[] planes = PlaneFinding.FindPlanes(meshData, snapToGravityThreshold, m_MinArea);
#endif

			// Pause our work here, and continue on the next frame.
			yield return null;
			float start = Time.realtimeSinceStartup;

            float maxFloorArea = 0.0f;
            float maxCeilingArea = 0.0f;
            FloorYPosition = 0.0f;
            CeilingYPosition = 0.0f;

            // Find the floor and ceiling.
            // We classify the floor as the maximum horizontal surface below the user's head.
            // We classify the ceiling as the maximum horizontal surface above the user's head.
            for (int i = 0; i < planes.Length; i++)
            {
                BoundedPlane boundedPlane = planes[i];
                if (boundedPlane.Bounds.Center.y < 0 && boundedPlane.Plane.normal.y >= m_UpNormalThreshold)
                {
                    maxFloorArea = Mathf.Max(maxFloorArea, boundedPlane.Area);
                    if (maxFloorArea == boundedPlane.Area)
                    {
                        FloorYPosition = boundedPlane.Bounds.Center.y;
                    }
                }
                else if (boundedPlane.Bounds.Center.y > 0 && boundedPlane.Plane.normal.y <= -(m_UpNormalThreshold))
                {
                    maxCeilingArea = Mathf.Max(maxCeilingArea, boundedPlane.Area);
                    if (maxCeilingArea == boundedPlane.Area)
                    {
                        CeilingYPosition = boundedPlane.Bounds.Center.y;
                    }
                }
            }

			int newPlanes = 0;
			List<SurfacePlane> oldPlanes = new List<SurfacePlane>(m_ActivePlanes);
			// Create SurfacePlane objects to represent each plane found in the Spatial Mapping mesh.
			for (int index = 0; index < planes.Length; index++)
            {
				BoundedPlane boundedPlane = planes[index];
				boundedPlane.Bounds.Extents.z = m_PlaneThickness /2.0f;
				SurfacePlane plane = CheckForExistingPlane(oldPlanes, boundedPlane);
				bool planeExisted = plane != null;

				if (plane == null)
				{
					newPlanes++;
					// This is a new plane.
					GameObject newPlaneObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
					plane = newPlaneObj.AddComponent<SurfacePlane>();
					newPlaneObj.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					newPlaneObj.transform.parent = planesParent.transform;

					newPlaneObj.name = "Plane " + m_PlaneId;
					m_PlaneId++;

					plane.PlaneType = GetPlaneType(boundedPlane);
					SetPlaneMaterial(plane);
				}
				else
				{
					oldPlanes.Remove(plane);
				}

				// Set the Plane property to adjust transform position/scale/rotation and determine plane type.
				plane.PlaneThickness = m_PlaneThickness;
				plane.Plane = boundedPlane;

                // Set the plane to use the same layer as the SpatialMapping mesh.  Do this every time incase the layer has changed.
                plane.gameObject.layer = SpatialMappingManager.Instance.PhysicsLayer;

				
                SetPlaneVisibility(plane);

                if ((m_DestroyPlanesMask & plane.PlaneType) == plane.PlaneType)
                {
                    DestroyImmediate(plane.gameObject);
                }
                else if (!planeExisted)
				{
					AddPlane(plane);
                }

                // If too much time has passed, we need to return control to the main game loop.
                if ((Time.realtimeSinceStartup - start) > FrameTime)
                {
					// Pause our work here, and continue making additional planes on the next frame.
					yield return null;
                    start = Time.realtimeSinceStartup;
                }
            }

			for (int index = 0; index < oldPlanes.Count; index++)
			{
				RemovePlane(oldPlanes[index]);
				Destroy(oldPlanes[index].gameObject);
			}

			// We are done creating planes, trigger an event.
			EventHandler handler = MakePlanesComplete;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

			MakingPlanes = false;
		}

		/// <summary>
		/// Adds the plane to tracking.
		/// </summary>
		/// <param name="plane"></param>
		private void AddPlane(SurfacePlane plane)
		{
			m_ActivePlanes.Add(plane);

			switch (plane.PlaneType)
			{
				case PlaneTypes.Ceiling:
				{
					m_CeilingPlanes.Add(plane.Plane, plane);
					break;
				}

				case PlaneTypes.Floor:
				{
					m_FloorPlanes.Add(plane.Plane, plane);
					break;
				}

				case PlaneTypes.Table:
				{
					m_TablePlanes.Add(plane.Plane, plane);
					break;
				}

				case PlaneTypes.Wall:
				{
					m_WallPlanes.Add(plane.Plane, plane);
					break;
				}
			}
		}

		/// <summary>
		/// Removes the plane from tracking.
		/// </summary>
		/// <param name="plane"></param>
		private void RemovePlane(SurfacePlane plane)
		{
			m_ActivePlanes.Remove(plane);

			switch (plane.PlaneType)
			{
				case PlaneTypes.Ceiling:
				{
					m_CeilingPlanes.Remove(plane);
					break;
				}

				case PlaneTypes.Floor:
				{
					m_FloorPlanes.Remove(plane);
					break;
				}

				case PlaneTypes.Table:
				{
					m_TablePlanes.Remove(plane);
					break;
				}

				case PlaneTypes.Wall:
				{
					m_WallPlanes.Remove(plane);
					break;
				}
			}
		}

		/// <summary>
		/// Checks the list of passed in planes for one that might match the passed in bounding plane.
		/// </summary>
		/// <param name="planes"></param>
		/// <param name="plane"></param>
		/// <returns></returns>
		private SurfacePlane CheckForExistingPlane(List<SurfacePlane> planes, BoundedPlane plane)
		{
			SurfacePlane bestMatch = null;
			float bestAreaDiff = float.MaxValue;
			float bestDistance = float.MaxValue;
			float bestDistPercent = float.MaxValue;

			PlaneTypes type = GetPossibleType(plane, m_UpNormalThreshold);

			foreach (SurfacePlane possiblePlane in planes)
			{
				if ((possiblePlane.PlaneType & type) == 0)
				{
					//Skip this one.
					continue;
				}

				//What is the area difference?
				float areaDiff = Mathf.Abs(possiblePlane.Plane.Area - plane.Area);
				float areaDiffPercent = areaDiff / ((possiblePlane.Plane.Area + plane.Area ) / 2);

				//What is the distance difference?
				float distDiff = (possiblePlane.Plane.Bounds.Center - plane.Bounds.Center).sqrMagnitude;
				float distChangePercent = distDiff /(possiblePlane.Plane.Bounds.Center.sqrMagnitude + plane.Bounds.Center.sqrMagnitude) / 2;

				if (areaDiffPercent >= m_MaxAreaDiffPercent || distDiff > m_MaxDistChange)
				{
					//The difference in these planes are to different so we can ignore this one.
					continue;
				}
				else if (areaDiffPercent < bestAreaDiff && distDiff < bestDistance)
				{
					bestMatch = possiblePlane;
					bestAreaDiff = areaDiffPercent;
					bestDistPercent = distChangePercent;
					distDiff = bestDistance;
				}
				else if (areaDiffPercent < bestAreaDiff && areaDiffPercent <= bestDistPercent)
				{
					bestMatch = possiblePlane;
					bestAreaDiff = areaDiffPercent;
					bestDistPercent = distChangePercent;
					distDiff = bestDistance;
				}
				else if (distDiff < bestDistance && distChangePercent <= areaDiffPercent)
				{
					bestMatch = possiblePlane;
					bestAreaDiff = areaDiffPercent;
					bestDistPercent = distChangePercent;
					distDiff = bestDistance;
				}

			}

			return bestMatch;
		}

		/// <summary>
		/// Sets the material on the renderer based on the type of plane it is.
		/// </summary>
		/// <param name="plane"></param>
		private void SetPlaneMaterial(SurfacePlane plane)
		{
			Material mat = null;
			switch (plane.PlaneType)
			{
				case PlaneTypes.Ceiling:
				{
					mat = m_CeilingMaterial;
					break;
				}

				case PlaneTypes.Floor:
				{
					mat = m_FloorMaterial;
					break;
				}

				case PlaneTypes.Table:
				{
					mat = m_TableMaterial;
					break;
				}

				case PlaneTypes.Wall:
				{
					mat = m_WallMaterial;
					break;
				}
			}

			plane.SetPlaneMaterial(mat);
		}

		/// <summary>
		/// Sets visibility of planes based on their type.
		/// </summary>
		/// <param name="surfacePlane"></param>
		private void SetPlaneVisibility(SurfacePlane surfacePlane)
        {
            surfacePlane.IsVisible = m_ShowPlanes && ((m_DrawPlanesMask & surfacePlane.PlaneType) == surfacePlane.PlaneType);
        }
#endregion
	}

#region Inspector
#if UNITY_EDITOR
	/// <summary>
	/// Editor extension class to enable multi-selection of the 'Draw Planes' and 'Destroy Planes' options in the Inspector.
	/// </summary>
	[CustomEditor(typeof(SurfacePlaneManager))]
    public class PlaneTypesEnumEditor : Editor
    {
        public SerializedProperty drawPlanesMask;
        public SerializedProperty destroyPlanesMask;

        void OnEnable()
        {
            drawPlanesMask = serializedObject.FindProperty("m_DrawPlanesMask");
            destroyPlanesMask = serializedObject.FindProperty("m_DestroyPlanesMask");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            drawPlanesMask.intValue = (int)((PlaneTypes)EditorGUILayout.EnumMaskField
                    ("Draw Planes", (PlaneTypes)drawPlanesMask.intValue));

            destroyPlanesMask.intValue = (int)((PlaneTypes)EditorGUILayout.EnumMaskField
                    ("Destroy Planes", (PlaneTypes)destroyPlanesMask.intValue));

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

#endregion
}
