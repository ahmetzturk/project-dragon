using UnityEngine;
using UnityEngine.UI;

namespace RPG.Resources 
{
    public class HealthDisplay : MonoBehaviour
    {
        private Health health;
        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }
        
        void Update()
        {
            GetComponent<Text>().text = health.GetHealthPoints() + "% / " + health.GetMaxHealthPoints() + "%";         
        }
    }
}
