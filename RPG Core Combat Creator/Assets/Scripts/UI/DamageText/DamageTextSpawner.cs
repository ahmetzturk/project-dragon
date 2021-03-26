using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText enemyDamageText = null;
        [SerializeField] DamageText playerDamageText = null;       
      
        public void Spawn(float damageAmount)
        {
            if (transform.parent.CompareTag("Player"))
            {
                DamageText damageText = Instantiate(playerDamageText, transform);
                damageText.SetValue(damageAmount);
            }
            else
            {
                DamageText damageText = Instantiate(enemyDamageText, transform);
                damageText.SetValue(damageAmount);
            }
        }
    }
}
