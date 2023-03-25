using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Utils;
using UnityEngine;

namespace RPG.Stats
{
    
    public class TraitStore : MonoBehaviour, IModifierProvider, IJsonSaveable, IPredicateEvaluator
    {
        [SerializeField] private TraitBonus[] bonusConfig;

        private Dictionary<Trait, int> _assignedPoints = new Dictionary<Trait, int>();
        private Dictionary<Trait, int> _stagedPoints = new Dictionary<Trait, int>();

        private Dictionary<Stat, Dictionary<Trait, float>> _additiveBonusCache;
        private Dictionary<Stat, Dictionary<Trait, float>> _percentageBonusCache;

        [System.Serializable]
        class TraitBonus
        {
            public Trait trait;
            public Stat stat;
            public float additiveBonusPerPoint = 0;
            public float percentageBonusPerPoint = 0;
        }

        private void Awake()
        {
            _additiveBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
            _percentageBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
            foreach (var bonus in bonusConfig)
            {
                if (!_additiveBonusCache.ContainsKey(bonus.stat))
                {
                    _additiveBonusCache[bonus.stat] = new Dictionary<Trait, float>();
                }
                if (!_percentageBonusCache.ContainsKey(bonus.stat))
                {
                    _percentageBonusCache[bonus.stat] = new Dictionary<Trait, float>();
                }

                _additiveBonusCache[bonus.stat][bonus.trait] = bonus.additiveBonusPerPoint;
                _percentageBonusCache[bonus.stat][bonus.trait] = bonus.percentageBonusPerPoint;
            }
        }

        public int GetProposedPoints(Trait trait)
        {
            return GetPoints(trait) + GetStagedPoints(trait);
        }
        
        public int GetStagedPoints(Trait trait)
        {
            if (_stagedPoints.ContainsKey(trait))
            {
                return _stagedPoints[trait];
            }
            return 0;
        }
        public int GetPoints(Trait trait)
        {
            if (_assignedPoints.ContainsKey(trait))
            {
                return _assignedPoints[trait];
            }
            return 0;
        }

        public void AssignPoints(Trait trait, int points)
        {
            if (!CanAssignPOints(trait, points))
            {
                return;
            }
            _stagedPoints[trait] = GetStagedPoints(trait) + points;
        }

        public bool CanAssignPOints(Trait trait, int points)
        {
            if (GetStagedPoints(trait) + points < 0)
            {
                return false;
            }

            if (GetUnassignedPoints() < points)
            {
                return false;
            }

            return true;
        }
        
        public int GetUnassignedPoints()
        {
            return GetAssignablePoints() - GetTotalProposedPoints();
        }

        private int GetTotalProposedPoints()
        {
            int total = 0;
            foreach (int points in _assignedPoints.Values)
            {
                total += points;
            }
            foreach (int points in _stagedPoints.Values)
            {
                total += points;
            }
            return total;
        }

        public void Commit()
        {
            foreach (var trait in _stagedPoints.Keys)
            {
                _assignedPoints[trait] = GetProposedPoints(trait);
            }
            _stagedPoints.Clear();
        }

        public int GetAssignablePoints()
        {
            return (int) GetComponent<BaseStats>().GetStat(Stat.MaxAttributPunkte);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (!_additiveBonusCache.ContainsKey(stat))
            {
                yield break;
            }

            foreach (var trait in _additiveBonusCache[stat].Keys)
            {
                float bonus = _additiveBonusCache[stat][trait];
                yield return bonus * GetPoints(trait);
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (!_percentageBonusCache.ContainsKey(stat))
            {
                yield break;
            }

            foreach (var trait in _percentageBonusCache[stat].Keys)
            {
                float bonus = _percentageBonusCache[stat][trait];
                yield return bonus * GetPoints(trait);
            }
        }
        
        public IEnumerable<float> GetBonusModifiers(Stat stat)
        {
            yield break;
        }
        
        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            foreach (KeyValuePair<Trait,int> pair in _assignedPoints)
            {
                stateDict[pair.Key.ToString()] = JToken.FromObject(pair.Value);
            }
            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JObject stateObject)
            {
                _assignedPoints.Clear();
                IDictionary<string, JToken> stateDict= stateObject;
                foreach (KeyValuePair<string, JToken> pair in stateDict)
                {
                    if (Enum.TryParse(pair.Key, true, out Trait trait))
                    {
                        _assignedPoints[trait] = pair.Value.ToObject<int>();
                    }
                }
            }
        }

        public bool? Evaluate(IPredicateEvaluator.PredicateType predicate, string[] parameters)
        {
            if (predicate == IPredicateEvaluator.PredicateType.MinimumTrait)
            {
                if (Enum.TryParse<Trait>(parameters[0], out Trait trait))
                {
                    return GetPoints(trait) >= Int32.Parse(parameters[1]);
                }
            }
            return null;
        }
    }
}