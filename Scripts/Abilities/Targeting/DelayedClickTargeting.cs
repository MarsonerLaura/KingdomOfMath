using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;
using UnityEngine.InputSystem;
using Cursor = UnityEngine.Cursor;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "New DelayedClickTargeting", menuName = "AbilitySystem/Targeting/DelayedClick", order = 0)]
    public class DelayedClickTargeting : TargetingStrategy
    {
        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private Vector2 cursorHotspot;
        [SerializeField] private float areaEffectRadius ;
        [SerializeField] private LayerMask layerMask ;
        [SerializeField] private Transform targetingPrefab;

        private Transform _targetingPrefabInstance = null;
        
        public override void StartTargeting(AbilityData data, Action finishedTargeting)
        {
            PlayerController playerController = data.GetUser().GetComponent<PlayerController>();
            playerController.StartCoroutine(Targeting(data, playerController, finishedTargeting));
        }

        private IEnumerator Targeting(AbilityData data, PlayerController playerController, Action finishedTargeting)
        {
            playerController.enabled = false;
            if (_targetingPrefabInstance == null)
            {
                _targetingPrefabInstance = Instantiate(targetingPrefab);
            }
            else
            {
                _targetingPrefabInstance.gameObject.SetActive(true);
            }

            _targetingPrefabInstance.localScale = new Vector3(areaEffectRadius*2, 1, areaEffectRadius*2);
            while (!data.IsCancelled())
            { 
                //run every frame
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
                RaycastHit rayCastHit;
                if (Physics.Raycast(PlayerController.GetMouseRay(), out rayCastHit, 1000, layerMask))
                {
                    _targetingPrefabInstance.position = rayCastHit.point;
                    
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        //absorb whole mouseclick
                        yield return new WaitWhile(() => Mouse.current.leftButton.isPressed);
                        data.SetTargetedPoint(rayCastHit.point);
                        data.SetTargets(GetGameObjectsInRadius(rayCastHit.point));
                        break;
                    }
                }
                yield return null;
            }
            _targetingPrefabInstance.gameObject.SetActive(false);
            playerController.enabled = true;
            finishedTargeting?.Invoke();
        }

        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
        {
            RaycastHit[] hits = Physics.SphereCastAll(point, areaEffectRadius, Vector3.up, 0);
            foreach (var hit in hits)
            {
                yield return hit.collider.gameObject;
            }
        }
    }
}
