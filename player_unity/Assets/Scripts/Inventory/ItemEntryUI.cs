
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemEntryUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    public void Render(Item item)
    {
        if (item == null)
            return;

        // Prefer ItemData assets
        if (item.data != null)
        {
            nameText.text = item.data.displayName;
            descriptionText.text = item.data.description;

            icon.sprite = item.data.icon;
            icon.enabled = item.data.icon != null;

            return;
        }

        // Temporary legacy fallback
        nameText.text = item.itemName;

        descriptionText.text =
            ItemDataManager.GetDescription(item.descriptionKey);

        Sprite sprite = ItemIconLookup.GetIcon(item);

        icon.sprite = sprite;
        icon.enabled = sprite != null;
    }
}
