using NUnit.Framework.Internal.Commands;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    float bulletSpeed = 0.0f;

    int damage = 0;

    public float bulletLife = 2.0f;

    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, bulletLife);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rb.AddForce(transform.forward * bulletSpeed, ForceMode.Force);
    }

    public void SetSpeed(float speed)
    {
        bulletSpeed = speed;
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    public int GetDamage()
    {
        return damage;
    }

    public void SetLife(float life)
    {
        bulletLife = life;
    }
}
