using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 70f;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }

        private float healthPoints = -1f;
        private bool isDead = false;
        
        private void Start()
        {
            if(healthPoints < 0)
            {
                healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
            }
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().OnLevelUp -= RegenerateHealth;
        }

        public float GetPercentage()
        {
            return GetFraction() * 100;
        }

        public float GetFraction()
        {
            return (healthPoints / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints = Mathf.Max(0, healthPoints -= damage);
            if (healthPoints == 0)
            {
                if (isDead == false)
                {
                    onDie.Invoke();
                    Die();
                    AwardExperience(instigator);                              
                }             
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        public void Heal(float healthToTestore)
        {
            healthPoints = Mathf.Min(healthPoints + healthToTestore, GetMaxHealthPoints());
        }

        public float GetHealthPoints()
        {
            return healthPoints;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }     

        private void Die()
        {
            GetComponent<Animator>().SetTrigger("die");
            isDead = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealth()
        {
            float regenerateHealthPoints = (GetComponent<BaseStats>().GetStat(Stat.Health) * regenerationPercentage) / 100;
            healthPoints = Mathf.Max(healthPoints, regenerateHealthPoints);
        }

        public object CaptureState()
        {
            return healthPoints;
        }

        public void RestoreState(object state)
        {
            healthPoints = (float)state;

            if (healthPoints == 0)
            {
                if (isDead == false)
                {
                    Die();
                }
            }
        }
    }
}
