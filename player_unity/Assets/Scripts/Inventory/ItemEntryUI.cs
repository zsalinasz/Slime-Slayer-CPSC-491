using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemEntryUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;

    public void Render(Item item)
    {
        nameText.text = item.itemName;
        descriptionText.text = ItemDataManager.GetDescription(item.descriptionKey);

        Sprite sprite = ItemIconLookup.GetIcon(item);
        icon.sprite = sprite;
        icon.enabled = sprite != null;
    }
}