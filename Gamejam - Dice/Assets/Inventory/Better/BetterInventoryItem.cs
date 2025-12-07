using UnityEngine;

public class BetterInventoryItem : MonoBehaviour
{
    public string itemName = "Key";

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponent<BetterInventory>();

        if (inventory != null)
        {
            inventory.AddItem(itemName);
            Destroy(gameObject);
        }
    }
}
