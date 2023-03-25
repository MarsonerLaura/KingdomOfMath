using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityData : IAction
    {
        private GameObject _user;
        private IEnumerable<GameObject> _targets;
        private Vector3 _targetedPoint;
        private bool _cancelled = false;
        private static readonly int StopAttackTrigger = Animator.StringToHash("stopAttack");

        public AbilityData(GameObject user)
        {
            _user = user;
        }

        #region Getters and Setters

        public IEnumerable<GameObject> GetTargets()
        {
            return _targets;
        }

        public GameObject GetUser()
        {
            return _user;
        }

        public Vector3 GetTargetedPoint()
        {
            return _targetedPoint;
        }

        
        public void SetTargets(IEnumerable<GameObject> targets)
        {
            _targets = targets;
        }

        public void SetTargetedPoint(Vector3 targetedPoint)
        {
            _targetedPoint = targetedPoint;
        }

        public bool IsCancelled()
        {
            return _cancelled;
        }
        #endregion

        #region Main Methods

        public void StartCoroutine(IEnumerator coroutine)
        {
            _user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
        }

        #endregion

        #region IAction Methods

        public void Cancel()
        {
            _cancelled = true;
            _user.GetComponent<Animator>().SetTrigger(StopAttackTrigger);
        }

        #endregion

        
    }

}

