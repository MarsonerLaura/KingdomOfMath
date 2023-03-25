using System;
using Cinemachine;
using RPG.Control;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Core
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField][Range(0f,10f)] private float mouseLookSensitivityX;
        [SerializeField][Range(0f,0.1f)] private float mouseLookSensitivityY;
        [SerializeField][Range(0f,1f)] private float mouseLookSensitivityYScroll;
        [SerializeField] private float _zoomSmoothTime;
        [SerializeField] private bool useMouseYForZoom = false;
        [SerializeField] GameObject freeLookCamera;
        private CinemachineFreeLook _freeLookComponent;
        private PlayerController _playerController;
        private PlayerInput _playerInput;
        private float _endPoint;
        private float _smoothPoint;
        private float _currentPoint;

        #region Basic Unity Methods

        private void Awake()
        {
            _freeLookComponent = freeLookCamera.GetComponent<CinemachineFreeLook>();
            _playerController = GetComponent<PlayerController>();
        }

        private void LateUpdate()
        {
            _freeLookComponent.m_YAxis.Value = Mathf.SmoothDamp(_freeLookComponent.m_YAxis.Value, _endPoint, ref _smoothPoint, _zoomSmoothTime);
        }

        private void OnEnable()
        {
            _playerInput = InputManager.Instance.GetPlayerInput();
            _playerInput.Player.CameraRotate.performed += RotateCamera;
            _playerInput.Player.CameraZoom.performed += ZoomCamera;
        }

        private void OnDisable()
        {
            _playerInput.Player.CameraRotate.performed -= RotateCamera;
            _playerInput.Player.CameraZoom.performed -= ZoomCamera;
        }

        #endregion

        #region Main Methods

        private void ZoomCamera(InputAction.CallbackContext context)
        {
            if (_endPoint == 0)
            {
                _endPoint = _freeLookComponent.m_YAxis.Value;
            }
            _endPoint += context.ReadValue<Vector2>().y * mouseLookSensitivityYScroll;
        }

        private void RotateCamera(InputAction.CallbackContext context)
        {
            if (Mouse.current.rightButton.isPressed)
            {
                if (_playerController.isDraggingUI)
                {
                    return;
                }
                var pointerDelta = context.ReadValue<Vector2>();
                _freeLookComponent.m_XAxis.Value += pointerDelta.x*mouseLookSensitivityX*Time.deltaTime;
                if (useMouseYForZoom)
                {
                    _endPoint += pointerDelta.y * mouseLookSensitivityY*Time.deltaTime;
                }
            }
        }

        #endregion

    }
}