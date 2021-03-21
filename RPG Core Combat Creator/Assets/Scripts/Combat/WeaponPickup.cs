using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon weapon = null;
        [SerializeField] float respawnTime = 5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                StartCoroutine(HideForSeconds(respawnTime));
            }
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (var child in GetAllChildren())
            {
                child.SetActive(shouldShow);
            }
        }

        private GameObject [] GetAllChildren()
        {
            GameObject[] allChildren = new GameObject[transform.childCount];
            for(int i=0; i<transform.childCount; i++)
            {
                allChildren[i] = transform.GetChild(i).gameObject;
            }
            return allChildren;
        }

    }
}
