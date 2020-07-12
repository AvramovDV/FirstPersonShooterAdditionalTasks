using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveCharacterByPoints : MonoBehaviour
{
    #region Fields

    [SerializeField] private NavMeshAgent _character;
    [SerializeField] private Camera _camera;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private LayerMask _layerMask;

    private Queue<Vector3> _points = new Queue<Vector3>();

    private NavMeshPath _path;

    #endregion


    #region UnityMethods

    private void Start()
    {
        _path = new NavMeshPath();
    }

    private void Update()
    {
        InputLogic();
        CharacterLogic();
        DrawPath();
    }

    #endregion


    #region Methods

    private void InputLogic()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetPoint();
        }
    }

    private void GetPoint()
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit))
        {
            _points.Enqueue(hit.point);
        }
    }

    private void CharacterLogic()
    {
        if (_points.Count > 0 && !_character.hasPath)
        {
            _character.SetDestination(_points.Dequeue());
        }
    }

    private void DrawPath()
    {
        List<Vector3> positions = new List<Vector3>();

        positions.Add(_character.transform.position);

        NavMesh.CalculatePath(positions[0], _character.destination, _layerMask, _path);

        positions.AddRange(_path.corners);

        Vector3[] points = _points.ToArray();

        if (points.Length > 0)
        {
            NavMesh.CalculatePath(_character.destination, points[0], _layerMask, _path);
            positions.AddRange(_path.corners);
            for (int i = 0; i < points.Length - 1; i++)
            {
                NavMesh.CalculatePath(points[i], points[i + 1], _layerMask, _path);
                positions.AddRange(_path.corners);
            }
        }

        _lineRenderer.positionCount = positions.Count;
        _lineRenderer.SetPositions(positions.ToArray());
    }

    #endregion
}
