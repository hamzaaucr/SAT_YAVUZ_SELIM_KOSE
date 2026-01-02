using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public string slotType;
    public InventoryManager manager;

    public void OnDrop(PointerEventData eventData)
    {
        if (manager == null) manager = FindObjectOfType<InventoryManager>();
        if (manager == null) return;

        if (eventData.pointerDrag == null) return;

        DraggableItem droppedItem = eventData.pointerDrag.GetComponent<DraggableItem>();

        if (droppedItem != null)
        {
            manager.TryEquipFromDrag(droppedItem.myItemName, slotType);
            Destroy(droppedItem.gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager == null) manager = FindObjectOfType<InventoryManager>();

        if (manager != null)
        {
            manager.SlotTiklandi(slotType);
        }
    }
}