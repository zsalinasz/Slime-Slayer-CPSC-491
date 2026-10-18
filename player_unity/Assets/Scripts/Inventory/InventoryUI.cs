using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] Transform slotParent;       // drag ItemGrid here
    [SerializeField] ItemData[] startingItems;   // temporary, for testing
    [SerializeField] int startingAmount = 1;

    public Inventory Inventory { get; private set; }

    SlotUI[] slotUIs;

    void Awake()
    {
        slotUIs = slotParent.GetComponentsInChildren<SlotUI>(true);
        Inventory = new Inventory(slotUIs.Length);
        Inventory.OnSlotChanged += RefreshSlot;

        foreach (var item in startingItems)
            if (item != null) Inventory.TryAdd(item, startingAmount);

        for (int i = 0; i < slotUIs.Length; i++)
            RefreshSlot(i);
    }

    void OnDestroy()
    {
        if (Inventory != null) Inventory.OnSlotChanged -= RefreshSlot;
    }

    void RefreshSlot(int index)
    {
        slotUIs[index].Render(Inventory.slots[index]);
    }
}