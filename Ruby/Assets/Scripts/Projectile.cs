using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    [SerializeField] private float bulletDistance =20;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // xoá đạn bay quá xa.
        if (transform.position.magnitude > bulletDistance)
        {
            DestroyProjectile();
        }
    }
    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController enemy = other.collider.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.Fix();
            DestroyProjectile();
        }
        if (other != null)
        {
            DestroyProjectile();
        }
             
    }
    private void DestroyProjectile()
    {
        Destroy(gameObject);
        RubyController.instance.addBullet();
    }
}
