using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class ClickDetector : MonoBehaviour
{
    public BattleManager manager;
    public SpriteRenderer mySprite;
    public bool isPlayer;

    private Color hoverColor;

    void Start()
    {
        hoverColor = isPlayer
            ? new Color(0.6f, 1f, 0.6f, 1f)
            : new Color(1f, 0.6f, 0.6f, 1f);
    }

    bool IsPointerOverUIButton()
    {
        if (EventSystem.current == null) return false;

        PointerEventData ped = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);

        foreach (var r in results)
            if (r.gameObject.GetComponent<Button>() != null) return true;

        return false;
    }

    void OnMouseEnter()
    {
        if (mySprite != null && !IsPointerOverUIButton())
            mySprite.color = hoverColor;
    }

    void OnMouseExit()
    {
        if (mySprite != null)
            mySprite.color = Color.white;
    }

    void OnMouseDown()
    {
        if (manager == null) return;
        if (!IsPointerOverUIButton())
            manager.KarakterTiklandi(isPlayer);
    }
}
