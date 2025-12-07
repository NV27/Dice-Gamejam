using UnityEngine;

public class BetterInventoryDoor : MonoBehaviour
{
    public string requiredItem1 = "Key1";
    public string requiredItem2 = "Key2";
    public string requiredItem3 = "Key3";
    public string requiredItem4 = "Key4";

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponent<BetterInventory>();

        if (
        inventory != null 
        && inventory.HasItem(requiredItem1)
        && inventory.HasItem(requiredItem2)
        && inventory.HasItem(requiredItem3)
        && inventory.HasItem(requiredItem4)
        )
        {
            Destroy(gameObject);
        }
    }
}
