using UnityEngine;

public class BasicBattery : MonoBehaviour
{
    public BasicInventory inventory;

    private void OnTriggerEnter(Collider other)
    {
        inventory.hasBattery = true;
        Destroy(gameObject);
    }
}
