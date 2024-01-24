using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] 
    private NavGrid _grid;
    [SerializeField] 
    float _movementSpeed = 5f;
    [SerializeField] 
    private float _rotationSpeed = 2;
    [SerializeField] 
    private float _minDistanceForWaypointChange = 0.2f;

    // How much to increase the rotation speed as the angle between the forward axis of the player and the next node increases
    // This prevents wide turns if the target is behind the player object at path start, and gradually corrects the objects path after smoothing is applied
    [SerializeField] 
    private float _rotationSpeedIncreaseFactor = 0.2f;
    
    private List<NavGridPathNode> _currentPath = new List<NavGridPathNode>();
    private int _currentPathIndex = 0;
    private CharacterController _characterController;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo))
            {
                SetTargetPosition(hitInfo.point);
            }
        }

        if (_currentPath != null && _currentPath.Count > 0)
        {
            MoveTowardsWaypoint();
        }
    }

    public void SetTargetPosition(Vector3 targetPos)
    {
        _currentPath = AStar.Instance.GetPath(_grid, transform.position, targetPos);
        _currentPathIndex = 0;
    }

    void MoveTowardsWaypoint()
    {
        if (_currentPathIndex >= 0 && _currentPathIndex < _currentPath.Count)
        {
            NavGridPathNode targetNode = _currentPath[_currentPathIndex];
            Vector3 targetPosition = targetNode.Position;

            float currentSpeed = _movementSpeed;
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            Vector3 moveDirectionForward = transform.forward;
            float moveDistance = currentSpeed * Time.deltaTime;
            
            Vector3 newPosition = transform.position + moveDirectionForward * moveDistance;
            newPosition.y = targetPosition.y;

            _characterController.Move(newPosition - transform.position);

            // Get the angle between the players forward vector and the next node
            float angleDifference = Vector3.Angle(moveDirectionForward, moveDirection);
            
            // Increase rotation speed as the angle gets larger
            float adjustedRotationSpeed = _rotationSpeed + (_rotationSpeedIncreaseFactor * angleDifference);
            float targetYRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

            float currentYRotation = transform.rotation.eulerAngles.y;
            float interpolatedYRotation = Mathf.LerpAngle(currentYRotation, targetYRotation, Time.deltaTime * adjustedRotationSpeed);

            transform.rotation = Quaternion.Euler(0f, interpolatedYRotation, 0f);

            float distanceToWaypoint = Vector3.Distance(new Vector3(transform.position.x, targetPosition.y, transform.position.z), targetPosition);
            
            if (distanceToWaypoint < _minDistanceForWaypointChange || distanceToWaypoint < moveDistance)
            {
                SetNextWaypoint();
            }
        }
    }

    void SetNextWaypoint()
    {
        _currentPathIndex++;

        if (_currentPathIndex >= _currentPath.Count)
            _currentPath.Clear();
    }
}