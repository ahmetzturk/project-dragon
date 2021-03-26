using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] TextMesh textMesh;
       public void DestroyText()
       {
            Destroy(gameObject);
       }
       
       public void SetValue(float amount)
       {
            textMesh.text = amount.ToString();
       }
    }
}
