using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float healthPoints = 100f;
        private bool isDead = false;

        public bool IsDead()
        {
            return isDead;
        }

        private void Update()
        {
            
        }

        public void TakeDamage(float damage)
        {
            healthPoints = Mathf.Max(0, healthPoints -= damage);

            if (healthPoints == 0)
            {
                if (isDead == false)
                {
                    Die();
                }
            }
        }

        private void Die()
        {
            GetComponent<Animator>().SetTrigger("die");
            isDead = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
