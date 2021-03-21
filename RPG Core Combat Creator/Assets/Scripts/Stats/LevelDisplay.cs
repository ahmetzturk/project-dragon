using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats baseStats;
        void Start()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }
        
        void Update()
        {
            GetComponent<Text>().text = baseStats.GetLevel().ToString();
        }
    }
}
