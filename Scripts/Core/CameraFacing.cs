using System;
using Cinemachine;
using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        [SerializeField] CinemachineFreeLook playerFramingCamera;

        #region Basic Unity Methods

        private void Start()
        {
            playerFramingCamera = GameObject.FindGameObjectWithTag("PlayerFramingCamera").GetComponent<CinemachineFreeLook>();
        }

        void LateUpdate()
        {
            transform.LookAt(2 * transform.position - playerFramingCamera.transform.position);
        }

        #endregion
        
    }
}
