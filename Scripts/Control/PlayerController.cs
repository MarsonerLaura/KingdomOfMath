using System;
using RPG.Attributes;
using RPG.Combat;
using RPG.Inventories;
using RPG.Movement;
using RPG.Utils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cursor = UnityEngine.Cursor;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour,IPredicateEvaluator
    {
        [SerializeField] private CursorMapping[] cursorMappings = null;
        [SerializeField] private float maxNavMeshProjectionDistance = 1f;
        [SerializeField] private float raycastRadius = 0.5f;

        private Camera _camera;
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;
        private ActionStore _actionStore;

        public bool isDraggingUI = false;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot; // The offset from the top left of the texture to use as the target point (must be within the bounds of the cursor).
        }

        #region Basic Unity Methods

        private void Awake()
        {
            _camera = Camera.main;
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _actionStore = GetComponent<ActionStore>();
        }

        private void Update()
        {
            if (InteractWithUI())
            {
                return;
            }
            if (_health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            UseAbilities();
            
            if (InteractWithComponent())
            {
                return;
            }
            if (InteractWithMovement())
            {
                return;
            }
            SetCursor(CursorType.None);
        }

        #endregion

        #region Main Methods

        private bool InteractWithMovement()
        {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!_mover.CanMoveTo(target))
                {
                    return false;
                }
                if (Mouse.current.leftButton.isPressed)
                {
                    _mover.StartMoveAction(target,1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }
         
        //returns true if cursor is over UI, else false
        private bool InteractWithUI()
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isDraggingUI = false;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Mouse.current.leftButton.isPressed)
                {
                    isDraggingUI = true;
                }
                SetCursor(CursorType.UI);
                return true;
            }

            if (isDraggingUI)
            {
                return true;
            }
            return false;
        }
        
        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }
        
        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type.Equals(type))
                {
                    return mapping;
                }
            }

            return cursorMappings[0];
        }
        
        public static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        }

        //returns all raycasthits sorted by distance
        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }
        
        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            
            //did the raycast hit anything?
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit)
            {
                return false;
            }

            //was there a navmesh found near the point the cursor hit?
            NavMeshHit navMeshHit;
            bool hasCastNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastNavMesh)
            {
                return false;
            }
            target = navMeshHit.position;
            
            //if there is no path, or no path from navmesh a to b, or pathlenght to long -> discard
            _mover.CanMoveTo(target);
            return true;
        }

        private void UseAbilities()
        {
            if (Keyboard.current[Key.Q].wasPressedThisFrame)
            {
                _actionStore.Use(0, gameObject);
            }
            if (Keyboard.current[Key.W].wasPressedThisFrame)
            {
                _actionStore.Use(1, gameObject);
            }
            if (Keyboard.current[Key.E].wasPressedThisFrame)
            {
                _actionStore.Use(2, gameObject);
            }
            if (Keyboard.current[Key.R].wasPressedThisFrame)
            {
                _actionStore.Use(3, gameObject);
            }
            if (Keyboard.current[Key.Digit1].wasPressedThisFrame)
            {
                _actionStore.Use(4, gameObject);
            }
            if (Keyboard.current[Key.Digit2].wasPressedThisFrame)
            {
                _actionStore.Use(5, gameObject);
            }
        }

        private bool? IsAtPosition(string[] parameters)
        {
            float posX = float.Parse(parameters[0]);
            float posZ = float.Parse(parameters[1]);
            float maxDistance = float.Parse(parameters[2]);
            Vector3 targetPosition = new Vector3(posX,transform.position.y, posZ);
           
            if (Vector3.Distance(transform.position, targetPosition)<maxDistance)
            {
                return true;
            }

            return false;
        }
        
        #endregion
        
        #region Getters/Setters

        public Fighter GetFighter()
        {
            return _fighter;
        }

        #endregion

        #region Evaluation Methods

        public bool? Evaluate(IPredicateEvaluator.PredicateType predicate, string[] parameters)
        {
            switch (predicate)
            {
                case IPredicateEvaluator.PredicateType.IsAtPlace:
                    return IsAtPosition(parameters);
            }

            return null;
        }

        #endregion
        
    }
}
