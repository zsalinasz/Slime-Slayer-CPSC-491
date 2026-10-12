using System;

[Serializable]
public class ItemStack
{
    public ItemData item;
    public int count;
}

public class Inventory
{
    public ItemStack[] slots;
    public event Action<int> OnSlotChanged;

    public Inventory(int size)
    {
        slots = new ItemStack[size];
        for (int i = 0; i < size; i++) slots[i] = new ItemStack();
    }

    // Returns the amount that did NOT fit
    public int TryAdd(ItemData item, int amount)
    {
        // 1) top up existing stacks
        for (int i = 0; i < slots.Length && amount > 0; i++)
        {
            var s = slots[i];
            if (s.item == item && s.count < item.maxStack)
            {
                int add = Math.Min(amount, item.maxStack - s.count);
                s.count += add;
                amount -= add;
                OnSlotChanged?.Invoke(i);
            }
        }
        // 2) fill empty slots
        for (int i = 0; i < slots.Length && amount > 0; i++)
        {
            var s = slots[i];
            if (s.item == null)
            {
                int add = Math.Min(amount, item.maxStack);
                s.item = item;
                s.count = add;
                amount -= add;
                OnSlotChanged?.Invoke(i);
            }
        }
        return amount;
    }

    public void Swap(int a, int b)
    {
        (slots[a], slots[b]) = (slots[b], slots[a]);
        OnSlotChanged?.Invoke(a);
        OnSlotChanged?.Invoke(b);
    }
}