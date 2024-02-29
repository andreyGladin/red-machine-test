using Connection;
using Player;
using UnityEngine;
using Utils.Scenes;

namespace Camera
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private float _deaccelerationMultiplier = 1f;
        [SerializeField] private float _dragSpeedMultiplier = 1f;
        [SerializeField] private float _afterDragAccelerationMultiplier = 1f;
        
        private ColorConnectionManager _colorConnectionManager;
        private Vector2 _moveSpeed;
        private UnityEngine.Camera _mainCamera;
        private Vector2 _startPosition;
        private Vector2 _previousPosition;
        private Vector3 _cameraStartPosition;
        private bool _scrolling = false;
        private Bounds _fieldBounds;

        private void Start()
        {
            _mainCamera = CameraHolder.Instance.MainCamera;
            
            ScenesChanger.SceneLoadedEvent += CalculateBorders;
        }

        private void OnDestroy()
        {
            ScenesChanger.SceneLoadedEvent -= CalculateBorders;
        }

        private void CalculateBorders()
        {
            _colorConnectionManager = FindObjectOfType<ColorConnectionManager>();

            var camTransform = _mainCamera.transform;
            camTransform.position = new Vector3(0f, 0f, camTransform.position.z);
            _fieldBounds = _colorConnectionManager.CalculateNodesBounds();
        }

        private void LateUpdate()
        {
            var camTransform = _mainCamera.transform;
            var camPosition = camTransform.position;
            
            if (PlayerController.PlayerState == PlayerState.Scrolling)
            {
                if (!_scrolling)
                {
                    _scrolling = true;
                    _startPosition = Input.mousePosition;
                    _cameraStartPosition = camPosition;
                }

                var diffStart = _mainCamera.ScreenToWorldPoint(_startPosition) - 
                                 _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var targetPosition = _cameraStartPosition + diffStart;
                var calculatedPos = _fieldBounds.ClosestPoint(Vector3.MoveTowards(camPosition,
                    new Vector3(targetPosition.x, targetPosition.y, camPosition.z), 
                    Time.deltaTime * _dragSpeedMultiplier));
                
                _mainCamera.transform.position = new Vector3(calculatedPos.x, calculatedPos.y, camPosition.z);
                
                var diffPrevious = _mainCamera.ScreenToWorldPoint(_previousPosition) - 
                                   _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                _moveSpeed = diffPrevious.normalized * _afterDragAccelerationMultiplier;
                _previousPosition = Input.mousePosition;
            }
            else
            {
                _scrolling = false;
                var calculatedPos = _fieldBounds.ClosestPoint(_mainCamera.transform.position +
                                                         (Vector3)(Time.deltaTime * _moveSpeed));
                camTransform.position = new Vector3(calculatedPos.x, calculatedPos.y, camPosition.z);
                _previousPosition = Input.mousePosition;
                
                if (_moveSpeed.magnitude > 0)
                {
                    _moveSpeed = Vector2.MoveTowards(_moveSpeed, Vector2.zero, 
                        Time.deltaTime * _deaccelerationMultiplier);
                }
                else
                {
                    _moveSpeed = Vector2.zero;
                }
            }
        }
    }
}