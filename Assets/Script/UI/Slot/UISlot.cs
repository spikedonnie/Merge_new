using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UISlot
{
    private int slotIndex;
    public Image iconImage;
    public string iconName;
    public string description;
    public string info1;
    public string info2;
    public Sprite iconSprite;
    public Button runButton;

    public int SlotIndex
    {
        get
        {
            return slotIndex;
        }
        set
        {
            slotIndex = value;
        }
    }
}

