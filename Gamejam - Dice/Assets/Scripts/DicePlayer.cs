using UnityEngine;

public class DicePlayer : MonoBehaviour
{


    public GameObject[] dFourGun;
    
    [SerializeField]
    GameObject currentGun;

    [SerializeField]
    Transform gunPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            int randomGun = Random.Range(0, dFourGun.Length);

            Destroy(currentGun);

            currentGun = Instantiate(dFourGun[randomGun], gunPoint.position, gunPoint.rotation, transform);
        }
    }
}
