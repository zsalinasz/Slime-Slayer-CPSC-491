using System.Collections.Generic;
using UnityEngine;

public class ItemListUI : MonoBehaviour
{
    [SerializeField] ItemEntryUI entryPrefab;
    [SerializeField] Transform contentParent;

    readonly List<GameObject> spawned = new List<GameObject>();
    int lastCount = -1;

    void OnEnable()
    {
        Refresh();
    }

    void Update()
    {
        if (PlayerDataManager.Instance == null) return;

        var items = PlayerDataManager.Instance.Data.inventoryItemIds;
        if (items != null && items.Count != lastCount)
            Refresh();
    }

    void Refresh()
    {
        if (PlayerDataManager.Instance == null) return;
        Render(PlayerDataManager.Instance.Data.inventoryItemIds);
    }

    public void Render(List<Item> items)
    {
        Debug.Log("Rendering " + (items == null ? 0 : items.Count) + " items");

        foreach (var go in spawned) Destroy(go);
        spawned.Clear();

        lastCount = items == null ? 0 : items.Count;
        if (items == null) return;

        foreach (Item item in items)
        {
            ItemEntryUI entry = Instantiate(entryPrefab, contentParent);
            entry.Render(item);
            spawned.Add(entry.gameObject);
        }
    }
}