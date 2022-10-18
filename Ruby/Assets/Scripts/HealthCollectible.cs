using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) //phát hiện các collider Trigger đưa vào other
    {
        RubyController controller = other.GetComponent<RubyController>(); //collider lấy liên kết tới component Ruby

        if (controller != null)
        {
            if (controller.health < controller.maxHealth)
            {
                controller.ChangeHealth(1);
                Destroy(gameObject);
            }
        }
    }    

}
