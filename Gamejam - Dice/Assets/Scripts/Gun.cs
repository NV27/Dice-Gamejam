using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    enum FireType
    {
        SingleShot,
        Rapid
    }

    [SerializeField]
    FireType fireType;


    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    Transform barrel;

    [SerializeField]
    float bulletSpeed = 30.0f;

    float singleFireDelay = 60.0f;
    float singleFireTimer;

    float rapidFireDelay = 5.0f;
    float rapidFireTimer;

    [SerializeField]
    int bulletDamage = 100;

    




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        singleFireTimer = singleFireDelay;
        rapidFireTimer = rapidFireDelay;
    }

    // Update is called once per frame
    void Update()
    {
        switch (fireType)
        {
            case FireType.SingleShot:
                SingleFire();
                break;

            case FireType.Rapid: 
                RapidFire();
                break;
        }
    }

    private void FixedUpdate()
    {
        // increment using fixedupdates consideration of timing
        if (singleFireTimer < singleFireDelay && fireType == FireType.SingleShot)
        {
            ++singleFireTimer;
        }
        
        if (rapidFireTimer < rapidFireDelay && fireType == FireType.Rapid)
        { 
            ++rapidFireTimer; 
        }
    }

    void SingleFire()
    {
        
        // fire on press, to a delay
        if (Input.GetMouseButtonDown(0) && singleFireTimer >= singleFireDelay)
        {
            SpawnBullet();
            singleFireTimer = 0.0f;
        }
    }

    void RapidFire()
    {
        // Fire when held
        if (Input.GetMouseButton(0))
        {
            if (rapidFireTimer >= rapidFireDelay)
            {
                SpawnBullet();
                rapidFireTimer = 0.0f;
            }
            
        }
    }

    void SpawnBullet()
    {
        GameObject go = Instantiate(bulletPrefab, barrel.position, barrel.rotation);
        Bullet bulletInstance = go.GetComponent<Bullet>();

        bulletInstance.SetSpeed(bulletSpeed);
        bulletInstance.SetDamage(bulletDamage);
    }
}
