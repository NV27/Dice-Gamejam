using UnityEngine;

public class DicePlayer : MonoBehaviour
{


    public GameObject[] dFourGun;
    public GameObject[] dSixGun;
    public GameObject[] dTwelveGun;
    
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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            int randomGun = Random.Range(0, dFourGun.Length);

            Destroy(currentGun);

            currentGun = Instantiate(dFourGun[randomGun], gunPoint.position, gunPoint.rotation, transform);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            int randomGun = Random.Range(0, dSixGun.Length);

            Destroy(currentGun);

            currentGun = Instantiate(dSixGun[randomGun], gunPoint.position, gunPoint.rotation, transform);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            int randomGun = Random.Range(0, dTwelveGun.Length);

            Destroy(currentGun);

            currentGun = Instantiate(dTwelveGun[randomGun], gunPoint.position, gunPoint.rotation, transform);
        }
    }
}
