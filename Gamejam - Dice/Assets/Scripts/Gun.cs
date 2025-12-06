using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    Transform barrel;

    [SerializeField]
    float bulletSpeed = 30.0f;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject go = Instantiate(bulletPrefab, barrel.position, barrel.rotation);
            go.GetComponent<Bullet>().SetSpeed(bulletSpeed);
        }
    }
}
