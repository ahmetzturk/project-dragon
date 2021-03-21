using RPG.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        Health target = null;
        [SerializeField] float speed = 1f;
        [SerializeField] GameObject hitEffect = null;
        private float damage = 0;
        [SerializeField] private bool isHoming = true;
        [SerializeField] private float maxLifeTime = 10f;
        [SerializeField] private GameObject[] DestroyOnHit;
        [SerializeField] private float lifeAfterImpact = 0.2f;
        private GameObject instigator = null;
        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (target == null) return;
            if(isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage) 
        {          
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            Destroy(gameObject, maxLifeTime);
        }          

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.gameObject.GetComponent<CapsuleCollider>();
            if (targetCapsule == null) return target.transform.position;
            return target.transform.position + Vector3.up * (targetCapsule.height / 2);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() == target)
            {
                if (target.IsDead()) return;
                if (hitEffect != null)
                {
                    GameObject hitEffect = Instantiate(this.hitEffect, GetAimLocation(), transform.rotation);
                }
                target.TakeDamage(instigator, damage);

                speed = 0;

                foreach (var item in DestroyOnHit)
                {
                    Destroy(item);
                }
                Destroy(gameObject, lifeAfterImpact);
            }
        }
    }
}
