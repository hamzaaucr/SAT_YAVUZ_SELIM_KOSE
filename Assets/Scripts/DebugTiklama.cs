using UnityEngine;
using UnityEngine.EventSystems;

public class DebugTiklama : MonoBehaviour
{
    void Update()
    {
        // Sol tık yapıldığında
        if (Input.GetMouseButtonDown(0))
        {
            // EventSystem üzerinden o an işaretlenen objeyi bul
            GameObject tiklananObje = EventSystem.current.currentSelectedGameObject;

            // Eğer bir UI elemanına denk geliyorsa
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("FARE ŞU AN BUNUN ÜSTÜNDE: " + EventSystem.current.currentSelectedGameObject);
                // Detaylı tarama (Pointer verisi ile)
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };
                var results = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                if (results.Count > 0)
                {
                    Debug.Log("🛑 TIKLAMAYI ENGELLEYEN EN ÜSTTEKİ OBJE: " + results[0].gameObject.name);
                }
            }
        }
    }
}