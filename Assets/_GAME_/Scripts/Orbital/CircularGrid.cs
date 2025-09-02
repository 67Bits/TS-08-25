using System.Collections.Generic;
using UnityEngine;
using Bello;
using System;
using Mono.CSharp;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using TS;
using System.Linq;

public class CircularGrid : MonoBehaviour
{
    public static CircularGrid Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<CircularGrid>();
            }
            return instance;
        }
    }
    private static CircularGrid instance;
    [Header("Grid Settings")]
    [Range(1, 20)] public int radialDivisions = 3;
    [Range(3, 64)] public int angularDivisions = 8;
    public float gridRadius = 5f;
    public Color gridColor = Color.white;
    public Color sectionCenterColor = Color.red;
    public float centerPointSize = 0.2f;
    public GameObject gridPointPrefab; // Optional prefab for grid points

    [Header("Motion Settings")]
    [Range(1, 50)] public int movingPoints = 8;
    [Range(0.1f, 100f)] public float moveSpeed = 2f;
    public Color pointColor = Color.green;
    [Range(0.1f, 1f)] public float pointSize = 0.3f;
    public bool showVelocityIndicators = true;
    public Color velocityColor = Color.yellow;
    public GameObject movingPointPrefab; // Optional prefab for moving points

    // Lists to store all created GameObjects
    public List<GridElement> gridCenterElements = new List<GridElement>();
    public List<GridElement> movingElements = new List<GridElement>();
    public List<GameObject> gridBorderObjects = new List<GameObject>();
    public List<GameObject> towers = new List<GameObject>();
    public List<float> radiusPerLevel;
    public List<float> velocityPerLevel;
    public List<float> outherRadiusPerLevel;

    private float[] pointAngles;
    private Vector3[] pointPositions;
    public int gridLevel = 0; // Current level of the grid, can be used for dynamic updates

    public UnityEvent onUpgrade;
    public UnityEvent onFullGrid;

    public float CurrentGridRadius => gridRadius/radialDivisions * (gridLevel + 1); // Adjust radius based on grid level

    [Header("Outer Ring Settings")]
    public float outerRingRadius = 6f; // Independent radius for outer points
    public GameObject outerRingPointPrefab;
    public Color outerRingColor = Color.cyan;
    public float outerRingPointSize = 0.25f;
    public List<GridElement> outerRingElements = new List<GridElement>();
    private bool firsUpgrade;

    [System.Serializable]
    public class GridElement
    {
        public int radialIndex;
        public int angularIndex;
        public GameObject gridObject;
        public Vector3 position;
        public float angle;
        public EnemyOrbiterAI _assignedEnemy;

        public bool IsEmpty
        {
            get
            {
                if (_assignedEnemy == null)
                    return true;

                return !_assignedEnemy.gameObject.activeSelf;
            }
        }

        public GridElement(int radialIndex, int angularIndex, GameObject gridObject, Vector3 position, float angle)
        {
            this.radialIndex = radialIndex;
            this.angularIndex = angularIndex;
            this.gridObject = gridObject;
            this.position = position;
            this.angle = angle;
        }

        public void UpdatePosition(Vector3 newPosition, float newAngle)
        {
            position = newPosition;
            angle = newAngle;
            if (gridObject != null)
            {
                gridObject.transform.position = position;
            }
        }

        public void AssignEnemyToGridElement(EnemyOrbiterAI enemy)
        {
            if (_assignedEnemy == null || !_assignedEnemy.gameObject.activeSelf)
                _assignedEnemy = enemy;
            else
            {
                Debug.Log("Cant add here");
            }
        }
    }

    public void UpdateVelocity(int lvl)
    {
         moveSpeed = velocityPerLevel[lvl - 1];
    }

    public void UpdateRadius(int lvl)
    {
        gridRadius = radiusPerLevel[lvl - 1];
    }

    public void UpdateOtherRadius(int lvl)
    {
        outerRingRadius = outherRadiusPerLevel[lvl - 1];
        UpdateOuterRingRadius(outerRingRadius);
    }

    public bool GridElementActiveInLevel(GridElement grtidElelemnt)
    {
        return grtidElelemnt.radialIndex < gridLevel + 1;
    }

    public bool IsRadialLevelFull(int  lvl)
    {
        if (gridLevel >= 2) return false;
        for (int i = 0; i < gridCenterElements.Count; i++)
        {
            if (gridCenterElements[i].radialIndex == lvl)
            {
                if (gridCenterElements[i]._assignedEnemy == null)
                {
                    return false;
                }
            }
        }
        return true; 
    }

    [Button]
    void InitializeAllPoints()
    {
        ClearExistingPoints();
        //InitializeGridPoints();
        //InitializeOuterRingPoints();
        InitializeMovingPoints();
    }

    public void ClearExistingPoints()
    {
        movingPoints = 0;
        //foreach (var point in gridCenterElements)
        //    if (point != null) DestroyImmediate(point.gridObject);
        foreach (var point in movingElements)
            if (point != null) DestroyImmediate(point.gridObject);
        //foreach (var point in outerRingElements)
        //    if (point != null) DestroyImmediate(point.gridObject);

        //gridCenterElements.Clear();
        movingElements.Clear();

        //outerRingElements.Clear(); // ← Clear outer ring
    }

    void InitializeOuterRingPoints()
    {
        outerRingElements.Clear();

        float angleStep = 360f / angularDivisions;

        for (int i = 0; i < angularDivisions; i++)
        {
            float midAngle = (i + 0.5f) * angleStep;
            Vector3 position = CalculatePosition(midAngle, outerRingRadius);

            GameObject pointObj;
            if (outerRingPointPrefab != null)
            {
                pointObj = Instantiate(outerRingPointPrefab, position, Quaternion.identity, transform);
            }
            else
            {
                pointObj = new GameObject($"OuterRingPoint_A{i}");
                pointObj.transform.position = position;
                pointObj.transform.parent = transform;
                pointObj.transform.LookAt(transform.position);
                pointObj.transform.rotation = Quaternion.LookRotation((transform.position - position).normalized, Vector3.up);
            }

            GridElement element = new GridElement(
                radialIndex: -1, // Not part of the regular radial grid
                angularIndex: i,
                gridObject: pointObj,
                position: position,
                angle: midAngle
            );

            outerRingElements.Add(element);
        }
    }

    [Button]
    public void UpdateOuterRingRadius(float newRadius)
    {
        outerRingRadius = newRadius;

        float angleStep = 360f / angularDivisions;

        for (int i = 0; i < outerRingElements.Count; i++)
        {
            float midAngle = (i + 0.5f) * angleStep;
            Vector3 newPosition = CalculatePosition(midAngle, outerRingRadius);

            outerRingElements[i].UpdatePosition(newPosition, midAngle);
        }
    }

    [Button]
    void InitializeGridPoints()
    {
        if (radialDivisions < 1) radialDivisions = 1;
        if (angularDivisions < 3) angularDivisions = 3;

        float angleStep = 360f / angularDivisions;
        float radiusStep = gridRadius / radialDivisions;

        for (int i = 0; i < gridCenterElements.Count; i++)
        {
            DestroyImmediate(gridCenterElements[i].gridObject);
        }
        // Clear existing elements first
        gridCenterElements.Clear();

        // Create center points for each grid section
        for (int r = 0; r < radialDivisions; r++)
        {
            float midRadius = (r + 0.5f) * radiusStep;

            for (int a = 0; a < angularDivisions; a++)
            {
                float midAngle = (a + 0.5f) * angleStep;
                Vector3 centerPoint = CalculatePosition(midAngle, midRadius);

                GameObject pointObj;
                if (gridPointPrefab != null)
                {
                    pointObj = Instantiate(gridPointPrefab, centerPoint, Quaternion.identity, transform);
                }
                else
                {
                    pointObj = new GameObject($"GridPoint_R{r}_A{a}");
                    pointObj.transform.position = centerPoint;
                    pointObj.transform.parent = transform;
                }

                // Create and add new GridElement
                GridElement element = new GridElement(
                    radialIndex: r,
                    angularIndex: a,
                    gridObject: pointObj,
                    position: centerPoint,
                    angle: midAngle
                );

                gridCenterElements.Add(element);
            }
        }
    }

    [Button]
    void InitializeMovingPoints()
    {
        pointAngles = new float[movingPoints];
        pointPositions = new Vector3[movingPoints];

        // Clear existing elements
        for (int i = 0; i < movingElements.Count; i ++)
        {
            Destroy(movingElements[i].gridObject);
        }
        movingElements.Clear();

        float angleStep = 360f / movingPoints;

        for (int i = 0; i < movingPoints; i++)
        {
            pointAngles[i] = i * angleStep;
            pointPositions[i] = CalculatePosition(pointAngles[i], gridRadius);

            GameObject pointObj;
            if (movingPointPrefab != null)
            {
                pointObj = Instantiate(movingPointPrefab, pointPositions[i], Quaternion.identity, transform);
            }
            else
            {
                pointObj = new GameObject($"MovingPoint_{i}");
                pointObj.transform.position = pointPositions[i];
                pointObj.transform.parent = transform;
            }

            // Calculate grid indices
            int radialIndex = Mathf.FloorToInt((pointPositions[i] - transform.position).magnitude / (gridRadius / radialDivisions));
            radialIndex = Mathf.Clamp(radialIndex, 0, radialDivisions - 1);
            int angularIndex = Mathf.FloorToInt(pointAngles[i] / (360f / angularDivisions)) % angularDivisions;

            // Create and add GridElement
            GridElement element = new GridElement(
                radialIndex: radialIndex,
                angularIndex: angularIndex,
                gridObject: pointObj,
                position: pointPositions[i],
                angle: pointAngles[i]
            );

            movingElements.Add(element);
        }
    }

    public Transform AddMovingPoint(EnemyOrbiterAI enemyOrbiterAI)
    {
        if (movingElements == null)
            movingElements = new List<GridElement>();

        // Create new point
        int newIndex = movingElements.Count;
        int totalPoints = newIndex + 1; // total points after adding
        float angleStep = 360f / totalPoints;

        Vector3 localPos = Quaternion.Euler(0, newIndex * angleStep, 0) * Vector3.forward * gridRadius;

        GameObject pointObj = movingPointPrefab != null
            ? Instantiate(movingPointPrefab, transform)
            : new GameObject($"MovingPoint_{newIndex}", typeof(Transform));

        pointObj.transform.localPosition = localPos;

        GridElement element = new GridElement(
            radialIndex: 0,
            angularIndex: newIndex,
            gridObject: pointObj,
            position: localPos,
            angle: newIndex * angleStep
        );
        element.AssignEnemyToGridElement(enemyOrbiterAI);

        movingElements.Add(element);

        // Recalculate positions for all points to evenly distribute them
        RepositionAllPoints();

        return pointObj.transform;
    }

    private void RepositionAllPoints()
    {
        int count = movingElements.Count;
        if (count == 0) return;

        float angleStep = 360f / count;
        for (int i = 0; i < count; i++)
        {
            Vector3 localPos = Quaternion.Euler(0, i * angleStep, 0) * Vector3.forward * gridRadius;
            movingElements[i].gridObject.transform.localPosition = localPos;
            movingElements[i].position = localPos;
            movingElements[i].angle = i * angleStep;
            movingElements[i].angularIndex = i;
        }
    }


    public void RemoveMovingPoint(EnemyOrbiterAI enemyOrbiterAI)
    {
        if (movingElements == null || movingElements.Count == 0)
            return;

        // Find the element for this enemy
        GridElement elementToRemove = movingElements.Find(e => e._assignedEnemy == enemyOrbiterAI);
        if (elementToRemove == null)
            return;

        // Destroy the GameObject
        if (elementToRemove.gridObject != null)
            Destroy(elementToRemove.gridObject);

        //// roatate
        float angleStep = 360f / Mathf.Max(movingElements.Count - 1, 1);
        transform.Rotate(Vector3.up, angleStep);

        // Remove from the list
        movingElements.Remove(elementToRemove);

        // Recalculate positions for remaining points
        if (movingElements.Count >= 2) RepositionAllPoints();

        // Update movingPoints count
        movingPoints = movingElements.Count;

        
    }


    private void RebuildPoints(EnemyOrbiterAI newEnemy)
    {
        float angleStep = 360f / Mathf.Max(movingPoints, 1);
        for (int i = 0; i < movingPoints; i++)
        {
            Vector3 localPos = Quaternion.Euler(0, i * angleStep, 0) * Vector3.forward * gridRadius;

            if (i < movingElements.Count)
            {
                movingElements[i].gridObject.transform.localPosition = localPos;
            }
            else
            {
                GameObject pointObj;
                if (movingPointPrefab != null)
                {
                    pointObj = Instantiate(movingPointPrefab, transform);
                }
                else
                {
                    pointObj = new GameObject($"MovingPoint_{i}");
                    pointObj.transform.parent = transform;
                }

                pointObj.transform.localPosition = localPos;

                GridElement element = new GridElement(
                    radialIndex: 0,
                    angularIndex: i,
                    gridObject: pointObj,
                    position: localPos,
                    angle: i * angleStep
                );

                if (newEnemy != null)
                    element.AssignEnemyToGridElement(newEnemy);

                movingElements.Add(element);
            }
        }
    }



        void Update()
    {
        if (Application.isPlaying && movingPoints > 0)
        {
            // Rotate the whole orbit
            transform.Rotate(Vector3.up, moveSpeed * Time.deltaTime, Space.Self);
        }
    }

    public GridElement GetGridSectionFromPosition(Vector3 worldPosition)
    {
        // Calculate vector from grid center to target position
        Vector3 toPosition = worldPosition - transform.position;
        toPosition.y = 0; // Ignore vertical difference

        // Early exit if position is at center
        if (toPosition.magnitude < 0.001f)
        {
            Debug.LogWarning("Position is at grid center");
            return null;
        }

        // Calculate raw angle and normalize to 0-360
        float rawAngle = Vector3.SignedAngle(Vector3.forward, toPosition, Vector3.up);
        rawAngle = (rawAngle + 360f) % 360f;

        // Grid calculations
        float angleStep = 360f / angularDivisions;
        float radiusStep = gridRadius / radialDivisions;

        // Find section indices
        int angularIndex = Mathf.FloorToInt(rawAngle / angleStep);
        float sectionAngle = (angularIndex + 0.5f) * angleStep; // Center angle of section
        sectionAngle = sectionAngle % 360f;

        // Distance calculations
        float distance = toPosition.magnitude;
        int radialIndex = Mathf.FloorToInt(distance / radiusStep);
        radialIndex = Mathf.Clamp(radialIndex, 0, radialDivisions - 1);

        // Get grid element
        int gridIndex = radialIndex * angularDivisions + angularIndex;
        if (gridIndex >= 0 && gridIndex < gridCenterElements.Count)
        {
            GridElement element = gridCenterElements[gridIndex];
            Debug.Log($"Found grid section at R:{radialIndex} A:{angularIndex} (Angle: {sectionAngle:F1}°)");
            return element;
        }

        return null;
    }

    void OnDrawGizmos()
    {
        DrawCircularGrid();
        DrawMovingPointsGizmos();
        DrawOuterRingPointsGizmos();
    }

    void DrawMovingPointsGizmos()
    {
        if (pointAngles == null || pointAngles.Length != movingPoints)
        {
            return;
        }

        for (int i = 0; i < movingPoints; i++)
        {
            if (i >= movingElements.Count || movingElements[i] == null) continue;

            // Draw velocity indicator if enabled
            if (showVelocityIndicators)
            {
                Vector3 tangent = Vector3.Cross(movingElements[i].gridObject.transform.position - transform.position, Vector3.up).normalized;
                Gizmos.color = velocityColor;
                Gizmos.DrawLine(movingElements[i].gridObject.transform.position,
                                movingElements[i].gridObject.transform.position + tangent * pointSize * 2f);
            }
        }
    }

    void DrawCircularGrid()
    {
        if (radialDivisions < 1) radialDivisions = 1;
        if (angularDivisions < 3) angularDivisions = 3;

        float angleStep = 360f / angularDivisions;
        float radiusStep = gridRadius / radialDivisions;

        // Draw radial lines
        for (int a = 0; a < angularDivisions; a++)
        {
            float angle = a * angleStep;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Gizmos.color = gridColor;
            Gizmos.DrawLine(transform.position, transform.position + direction * gridRadius);
        }

        // Draw concentric circles
        for (int r = 1; r <= radialDivisions; r++)
        {
            float currentRadius = r * radiusStep;
            DrawCircle(transform.position, currentRadius, angularDivisions * 2);
        }

        // Draw center points of each section
        for (int r = 0; r < radialDivisions; r++)
        {
            float innerRadius = r * radiusStep;
            float outerRadius = (r + 1) * radiusStep;
            float midRadius = (innerRadius + outerRadius) / 2f;

            for (int a = 0; a < angularDivisions; a++)
            {
                float midAngle = (a + 0.5f) * angleStep;
                Vector3 centerPoint = CalculatePosition(midAngle, midRadius);

                Gizmos.color = sectionCenterColor;
                Gizmos.DrawSphere(centerPoint, centerPointSize);
            }
        }
    }

    Vector3 CalculatePosition(float angle, float radius)
    {
        float rad = angle * Mathf.Deg2Rad;
        return transform.position + new Vector3(
            Mathf.Sin(rad) * radius,
            0,
            Mathf.Cos(rad) * radius
        );
    }

    void DrawCircle(Vector3 center, float radius, int segments)
    {
        if (segments < 3) return;

        Vector3 prevPoint = CalculatePosition(0, radius);
        for (int i = 1; i <= segments; i++)
        {
            float angle = (i * 360f) / segments;
            Vector3 nextPoint = CalculatePosition(angle, radius);
            Gizmos.color = gridColor;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }

    void DrawOuterRingPointsGizmos()
    {
        Gizmos.color = outerRingColor;

        foreach (var element in outerRingElements)
        {
            if (element != null)
                Gizmos.DrawSphere(element.position, outerRingPointSize);
        }
    }

    public Vector3 GetGridSectionCenter(int radialIndex, int angularIndex)
    {
        if (radialIndex < 0 || radialIndex >= radialDivisions ||
            angularIndex < 0 || angularIndex >= angularDivisions)
        {
            Debug.LogWarning("Invalid section indices");
            return transform.position;
        }

        float angleStep = 360f / angularDivisions;
        float radiusStep = gridRadius / radialDivisions;

        float midRadius = (radialIndex + 0.5f) * radiusStep;
        float midAngle = (angularIndex + 0.5f) * angleStep;

        return CalculatePosition(midAngle, midRadius);
    }
}
