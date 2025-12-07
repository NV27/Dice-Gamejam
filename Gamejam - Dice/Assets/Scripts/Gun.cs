using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;
using Unity.Mathematics;

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

    [SerializeField]
    float bulletLife = 2f;

    [SerializeField]
    float singleFireDelay = 60.0f;
    
    float singleFireTimer;

    float rapidFireDelay = 5.0f;
    float rapidFireTimer;

    [SerializeField]
    int bulletDamage = 100;

    [SerializeField]
    int bulletNumber = 1;

    public TextMeshProUGUI reloaded;

    private float offset = 0;
    

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

        if (singleFireTimer >= singleFireDelay)
        {
            reloaded.text = "Time To Die";
        }
        else
        {
            reloaded.text = "";
        }
    }

    private void FixedUpdate()
    {
        // increment using fixedupdates consideration of timing
        if (singleFireTimer < singleFireDelay)
        {
            ++singleFireTimer;
        }
        
        if (rapidFireTimer < rapidFireDelay)
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

        if (bulletNumber > 1){
            for(var i = 0; i < bulletNumber; i++)
            {
                var offsetX = UnityEngine.Random.Range(0f, 30f);
                var offsetY = UnityEngine.Random.Range(0f, 30f);
                var offsetZ = UnityEngine.Random.Range(0f, 30f);
                Quaternion spawnRot = barrel.rotation * Quaternion.Euler(offsetX-15f, offsetY-15f, offsetZ-15f);
                
                GameObject go = Instantiate(bulletPrefab, barrel.position, spawnRot);
                Bullet bulletInstance = go.GetComponent<Bullet>();

                bulletInstance.SetSpeed(bulletSpeed);
                bulletInstance.SetDamage(bulletDamage);
                bulletInstance.SetLife(bulletLife);
            }
        }
        else
        {
            GameObject go = Instantiate(bulletPrefab, barrel.position, barrel.rotation);
            Bullet bulletInstance = go.GetComponent<Bullet>();

            bulletInstance.SetSpeed(bulletSpeed);
            bulletInstance.SetDamage(bulletDamage);
            bulletInstance.SetLife(bulletLife);
        }
        
    }
}
