using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

[CreateAssetMenu(fileName = "Progression", menuName = "CombatSystem/Stats/New Progression",order = 0)]
public class Progression : ScriptableObject
{
    [SerializeField] private ProgressionCharacterClass[] characterClasses = null;
    
    private Dictionary<CharacterClass, Dictionary<Stat, float[]>> _lookupTable = null;

    #region Main Methods

    public float GetStat(Stat stat, CharacterClass characterClass, int level)
    {
        BuildLookup();
        if (!_lookupTable[characterClass].ContainsKey(stat))
        {
            return 0;
        }
        float[] levels = _lookupTable[characterClass][stat];

        if (levels.Length == 0)
        {
            return 0;
        }
        if (levels.Length < level)
        {
            return levels[levels.Length - 1];
        }
        return levels[level-1];
    }

    public int GetLevels(Stat stat, CharacterClass characterClass)
    {
        BuildLookup();
        float[] levels = _lookupTable[characterClass][stat];
        return levels.Length;
    }

    
    private void BuildLookup()
    {
        if (_lookupTable != null)
        {
            return;
        }
        
        _lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
        foreach (ProgressionCharacterClass progCharacterClass in characterClasses)
        {
            Dictionary<Stat, float[]> temp = new Dictionary<Stat, float[]>();
            foreach (ProgressionStat progressionStat in progCharacterClass.stats)
            {
                temp[progressionStat.stat] = progressionStat.levels;
            }
            _lookupTable[progCharacterClass.characterClass] = temp;
        }
    }
    
    #endregion
    

    [System.Serializable]
    private class ProgressionCharacterClass
    {
        public CharacterClass characterClass;
        public ProgressionStat[] stats;
    }
    
    [System.Serializable]
    private class ProgressionStat
    {
        public Stat stat;
        public float[] levels;
    }
    
}
