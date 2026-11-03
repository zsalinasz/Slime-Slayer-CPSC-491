using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LootBoxUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject lootPanel;
    [SerializeField] private Transform contentParent;
    [SerializeField] private Button takeButtonPrefab;

    private LootBox activeLootBox;

    public bool IsOpen => lootPanel != null &&
                          lootPanel.activeSelf;

    private void Awake()
    {
        if (lootPanel != null)
            lootPanel.SetActive(false);
    }

    public void Open(LootBox lootBox)
    {
        if (lootBox == null || lootPanel == null ||
            contentParent == null || takeButtonPrefab == null)
        {
            Debug.LogWarning("LootBoxUI: Missing references.");
            return;
        }

        Close();

        activeLootBox = lootBox;
        activeLootBox.OnLootChanged += Refresh;

        lootPanel.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        if (activeLootBox != null)
            activeLootBox.OnLootChanged -= Refresh;

        activeLootBox = null;

        if (lootPanel != null)
            lootPanel.SetActive(false);
    }

    private void Refresh()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        if (activeLootBox == null)
            return;

        for (int i = 0; i < activeLootBox.Items.Count; i++)
        {
            int index = i;
            LootBoxEntry entry = activeLootBox.Items[i];

            Button button = Instantiate(
                takeButtonPrefab, contentParent
            );

            TMP_Text label =
                button.GetComponentInChildren<TMP_Text>();

            if (label != null && entry.item != null)
            {
                label.text = entry.item.displayName +
                             " x" + entry.quantity +
                             " — Take";
            }

            button.onClick.AddListener(() =>
            {
                if (activeLootBox != null)
                    activeLootBox.TakeItem(index);
            });
        }
    }
}
