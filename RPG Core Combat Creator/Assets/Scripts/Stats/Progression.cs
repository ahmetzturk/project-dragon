using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;
        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            float [] levels = lookupTable[characterClass][stat];
            if (levels.Length < level) return 0;
            return levels[level-1];
            //foreach (var item in characterClasses)
            //{
            //    if(item.characterClass == characterClass)
            //    {
            //        foreach (var itemStat in item.stats)
            //        {
            //            if (itemStat.stat == stat)
            //            {
            //                if (itemStat.levels.Length < level) continue;
            //                return itemStat.levels[level-1];
            //            }
            //        }
            //    }
            //}
            //return 0;

        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (var item in characterClasses)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();

                foreach (var itemStat in item.stats)
                {
                    statLookupTable[itemStat.stat] = itemStat.levels;
                }
                lookupTable[item.characterClass] = statLookupTable;
            }
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        [System.Serializable]
        public class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        public class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}
