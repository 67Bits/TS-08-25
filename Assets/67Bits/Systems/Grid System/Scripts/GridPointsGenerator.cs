using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SSB.GridSystem
{
    /// <summary>
    /// This class generates a hexagonal grid based on the specified size and cell radius.
    /// It provides functionality to visualize the grid, including hexagon points, rotation points, and circle points.
    /// </summary>
    [AddComponentMenu("SSB/SortingGame/HexagonGrid")]
    [RequireComponent(typeof(Grid))]
    public class GridPointsGenerator : MonoBehaviour
    {
        [SerializeField] private Vector2Int _gridSize;
        [SerializeField] private float _cellRadius;

        [SerializeField] private bool _showGrid;
        [SerializeField] private Grid _grid;

        [SerializeField] private bool _createMiddlePointsRotation;

        [Header("Gizmos Settings")]
        [Range(3, 12), SerializeField] private int _circlePoints = 6;
        [SerializeField] private float _circleOffset = 0;
        [SerializeField] private bool _enableCircleGizmos;
        [SerializeField] private bool _enableHexagonGizmos;
        [SerializeField] private bool _enableTriangleGizmos;
        [SerializeField] private bool _enableRotationPointsGizmos;

        [ReadOnly] public List<Vector3> GridPoints = new List<Vector3>();
        [ReadOnly] public List<Vector3> RotationPoints = new List<Vector3>();
        [ReadOnly] public List<Vector3> HexagonPoints = new List<Vector3>();

        public float CellRadius => _cellRadius;
        public int CirclePoints => _circlePoints;
        public float CircleOffset => _circleOffset;
        public Vector2 GridSize => _gridSize;

        [Button("Generate Grid", buttonSize: ButtonSizes.Medium)]
        private void GenerateGridPoints()
        {
            RotationPoints.Clear();
            GridPoints.Clear();
            HexagonPoints.Clear();

            if (!_grid) _grid = GetComponent<Grid>();

            if (_showGrid)
            {
                for (int x = 0; x < _gridSize.x; x++)
                {
                    for (int y = 0; y < _gridSize.y; y++)
                    {
                        Vector3 hexagonPosition = _grid.GetCellCenterWorld(new Vector3Int(x, y));

                        HexagonPoints.Add(hexagonPosition);
                        GridPoints.Add(hexagonPosition);
                        RotationPoints.Add(hexagonPosition);

                        for (int i = 0; i < _circlePoints; i++)
                        {
                            Vector3 currentPoint = GetPointPosition(i);
                            GridPoints.Add(hexagonPosition + currentPoint);
                        }
                    }

                }

                if (_createMiddlePointsRotation)
                {
                    GetPointsAvailableToRotate();
                }
            }
        }
        private Vector3 GetPointPosition(int i)
        {
            float angle = (2 * Mathf.PI / _circlePoints) * i + _circleOffset;
            Vector3 pointPosition = Vector2.zero;
            pointPosition.x = Mathf.Cos(angle) * _cellRadius;
            pointPosition.z = Mathf.Sin(angle) * _cellRadius;
            return pointPosition;
        }
        #region MiddlePointsRotation
        private void GetPointsAvailableToRotate()
        {
            for (int i = 0; i < GridPoints.Count; i++)
            {
                Vector3 item = GridPoints[i];
                bool isRotationPoint = CheckIsRotationPoint(item);
                bool isInList = CheckItemIsInList(item);
                if (isRotationPoint && !isInList)
                {
                    RotationPoints.Add(item);
                }
            }
        }
        private bool CheckItemIsInList(Vector3 point)
        {
            bool isInList = false;
            for (int i = 0; i < RotationPoints.Count; i++)
            {
                Vector3 item = RotationPoints[i];
                if (item == point) isInList = true;
            }

            return isInList;
        }
        private bool CheckIsRotationPoint(Vector3 position)
        {
            int repeats = 0;
            for (int i = 0; i < GridPoints.Count && repeats < 3; i++)
            {
                Vector3 currentPoint = GridPoints[i];
                if (currentPoint == position)
                {
                    repeats++;
                }
            }

            return repeats == 3;
        }
        #endregion
        private void OnDrawGizmos()
        {
            if (_showGrid)
            {
                if (HexagonPoints.Count == 0)
                    GenerateGridPoints();
                for (int x = 0; x < _gridSize.x; x++)
                {
                    for (int y = 0; y < _gridSize.y; y++)
                    {
                        Vector3 hexagonPosition = _grid.GetCellCenterWorld(new Vector3Int(x, y));
                        if (!HexagonPoints.Contains(hexagonPosition)) continue;

                        Gizmos.color = Color.black;
                        //Gizmos.DrawSphere(hexagonPosition, 0.2f);
                        if (_enableCircleGizmos)
                            Gizmos.DrawWireSphere(hexagonPosition, _cellRadius);

                        for (int i = 0; i < _circlePoints; i++)
                        {
                            Vector3 currentPoint = GetPointPosition(i);
                            Vector3 nextPoint = GetPointPosition((int)Mathf.Repeat(i + 1, _circlePoints));
                            Gizmos.color = Color.blue;
                            Gizmos.DrawWireSphere(hexagonPosition + currentPoint, .1f);
                            Gizmos.color = Color.yellow;
                            if (_enableHexagonGizmos)
                                Gizmos.DrawLine(hexagonPosition + currentPoint, hexagonPosition + nextPoint);
                            Gizmos.color = Color.grey;
                            if (_enableTriangleGizmos)
                                Gizmos.DrawLine(hexagonPosition + currentPoint, hexagonPosition);
                        }
                    }
                }
                if (_enableRotationPointsGizmos)
                    DrawRotationPoints();
            }
        }
        private void DrawRotationPoints()
        {
            for (int i = 0; i < RotationPoints.Count; i++)
            {
                Vector3 point = RotationPoints[i];
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(point, 0.2f);
            }
        }
        private void OnValidate()
        {
            GenerateGridPoints();
        }
    }
}
