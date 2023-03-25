
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Utils
{
    [System.Serializable]
    public class Condition
    {
        [SerializeField] private Disjunction[] and;

        #region Main Methods

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (Disjunction disjunction in and)
            {
                if (!disjunction.Check(evaluators))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        [System.Serializable]
        class Disjunction
        {
            [SerializeField] private Predicate[] or;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (Predicate predicate in or)
                {
                    if (predicate.Check(evaluators))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        
        
        [System.Serializable]
        class Predicate
        {
            [SerializeField] private IPredicateEvaluator.PredicateType predicate;
            [SerializeField] private string[] parameters;
            [SerializeField] private bool negate = false;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (var evaluator in evaluators)
                {
                    bool? result = evaluator.Evaluate(predicate, parameters);
                    if (result == null)
                    {
                        continue;
                    }

                    if (result == negate)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
