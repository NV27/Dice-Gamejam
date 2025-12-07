using RatKing;
using UnityEngine;
using TMPro;

public class DicePlayer : MonoBehaviour
{


    public GameObject[] dFourGun;
    public GameObject[] dSixGun;
    public GameObject[] dTwelveGun;
    
    [SerializeField]
    GameObject currentGun;

    [SerializeField]
    Transform gunPoint;

    public TextMeshProUGUI gunType;
    private string pistolName = "Pistol";
    private string shotGunName = "ShotGun";
    private string machineGunName = "Rapid";

    private int gunSwapTimer = 0;
    private int gunSwapAmount = 1500;
    private int gunSwapNumber = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        ++gunSwapTimer;

        if (gunSwapTimer >= gunSwapAmount)
        {
            //var r = Random.Range(-1,1);
            
            //if (gunSwapNumber == 0)
            //{
           //     gunSwapNumber = 1;
            //}

            //gunSwapNumber += r;
            gunSwapNumber++;
            gunSwapTimer = 0;

            if (gunSwapNumber > 3)
            {
                gunSwapNumber = 1;
            }

            if (gunSwapNumber < 0)
            {
                gunSwapNumber = 3;
            }

            SwapGun();
        }

        currentGun.transform.position = gunPoint.position;
        currentGun.transform.rotation = gunPoint.rotation;
    }

    void SwapGun()
    {
        if (gunSwapNumber == 1)
        {
            int randomGun = Random.Range(0, dFourGun.Length);

            Destroy(currentGun);

            currentGun = Instantiate(dFourGun[randomGun], gunPoint.position, gunPoint.rotation, transform);

            gunType.text = pistolName;

        }
        else if (gunSwapNumber == 2)
        {
            int randomGun = Random.Range(0, dSixGun.Length);

            Destroy(currentGun);

            currentGun = Instantiate(dSixGun[randomGun], gunPoint.position, gunPoint.rotation, transform);

            gunType.text = shotGunName;
        }
        else if (gunSwapNumber == 3)
        {
            int randomGun = Random.Range(0, dTwelveGun.Length);

            Destroy(currentGun);

            currentGun = Instantiate(dTwelveGun[randomGun], gunPoint.position, gunPoint.rotation, transform);

            gunType.text = machineGunName;

        }
    }
}

//else if (Input.GetKeyDown(KeyCode.Alpha3))