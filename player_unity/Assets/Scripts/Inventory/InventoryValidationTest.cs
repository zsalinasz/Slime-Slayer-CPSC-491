using UnityEngine;
using UnityEngine;

public class InventoryValidationTest : MonoBehaviour
{
    [SerializeField] private ItemData testItem;

    private void Start()
    {
        PlayerDataManager manager = PlayerDataManager.Instance;

        if (manager == null || testItem == null)
        {
            Debug.LogError(
                "Inventory test setup is incomplete."
            );
            return;
        }

        // Test 1: Valid addition
        bool addValid = manager.AddItem(testItem, 2);
        Debug.Log("Valid addition: " + addValid);

        // Test 2: Negative quantity
        bool addNegative = manager.AddItem(testItem, -5);
        Debug.Log("Negative addition rejected: " + !addNegative);

        // Test 3: Excessive removal
        bool removeTooMany = manager.RemoveItem(testItem, 999);
        Debug.Log("Excessive removal rejected: " + !removeTooMany);

        // Test 4: Valid removal
        bool removeValid = manager.RemoveItem(testItem, 1);
        Debug.Log("Valid removal: " + removeValid);

        // Test 5: Check ownership
        bool ownsItem = manager.HasItem(testItem, 1);
        Debug.Log("Owns at least one item: " + ownsItem);
    }
}
