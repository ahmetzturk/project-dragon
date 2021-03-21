using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience = null;
        void Start()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        void Update()
        {
            GetComponent<Text>().text = experience.GetExperience().ToString();
        }
    }
}
