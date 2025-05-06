using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ListLevelItem : MonoBehaviour
{
    public event Action<int> OnClickEvent;

    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI label;

    public void SetLevelData(LevelData levelData)
    {
        label.text = $"Level {levelData.index + 1}";
        button.onClick.AddListener(() => OnClickEvent?.Invoke(levelData.index));
    }
}