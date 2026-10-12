using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text countText;

    public void Render(ItemStack stack)
    {
        bool has = stack != null && stack.item != null;
        icon.enabled = has;
        icon.sprite = has ? stack.item.icon : null;
        countText.text = has && stack.count > 1 ? stack.count.ToString() : "";
    }
}