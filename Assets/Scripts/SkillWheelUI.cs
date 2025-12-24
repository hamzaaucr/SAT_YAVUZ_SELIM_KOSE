using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillWheelUI : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform container;   // SkillWheelContainer
    public GameObject buttonPrefab;   // SkillButtonPrefab (Button + Image)
    public Canvas canvas;             // Main Canvas

    [Header("Layout")]
    public float radius = 120f;

    private Action<Skill, int> onClick; // (skill, slotIndex)
    private readonly List<GameObject> spawned = new();

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Open(Vector2 screenPos, List<Skill> skills, Action<Skill, int> callback)
    {
        if (canvas == null || container == null || buttonPrefab == null)
        {
            Debug.LogError("SkillWheelUI: canvas/container/buttonPrefab eksik!");
            return;
        }

        if (skills == null || skills.Count == 0)
        {
            Debug.Log("SkillWheelUI: gösterilecek skill yok.");
            return;
        }

        gameObject.SetActive(true);
        onClick = callback;
        Clear();

        // Paneli mouse pozisyonuna taþý (UI local)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPos
        );

        var panelRt = (RectTransform)transform;
        panelRt.anchorMin = panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.anchoredPosition = localPos;

        float step = 360f / skills.Count;
        float angle = 90f;

        for (int i = 0; i < skills.Count; i++)
        {
            Skill s = skills[i];
            int slotIndex = i + 1; // 1-based slot

            GameObject obj = Instantiate(buttonPrefab, container);
            spawned.Add(obj);

            // Daire dizilim
            RectTransform rt = obj.GetComponent<RectTransform>();
            if (rt != null)
            {
                float rad = angle * Mathf.Deg2Rad;
                rt.anchoredPosition = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
            }

            // Icon
            Image img = obj.GetComponentInChildren<Image>();
            if (img != null) img.sprite = s.skillIcon;

            // Click
            Button btn = obj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    onClick?.Invoke(s, slotIndex);
                    Close();
                });
            }

            angle -= step;
        }
    }

    public void Close()
    {
        Clear();
        gameObject.SetActive(false);
    }

    private void Clear()
    {
        for (int i = 0; i < spawned.Count; i++)
            if (spawned[i] != null) Destroy(spawned[i]);

        spawned.Clear();
    }
}
