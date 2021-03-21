using RPG.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter playerFighter;
        private Health targetHealth;
        // Start is called before the first frame update
        void Awake()
        {
            playerFighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        // Update is called once per frame
        void Update()
        {
            targetHealth = playerFighter.GetTarget();
            if (targetHealth == null)
            {
                GetComponent<Text>().text = "N/A";
                return;
            }
            GetComponent<Text>().text = targetHealth.GetHealthPoints() + "% / " + targetHealth.GetMaxHealthPoints() + "%";
        }
    }
}
