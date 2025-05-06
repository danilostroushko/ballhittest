using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : UIView
{
    [Title("Game screen")]
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI levelLabel;

    public override void Start()
    {
        base.Start();

        backButton.onClick.AddListener(() => GameController.Instance.OpenMainScene());
    }

    public void SetLevel(int level)
    {
        levelLabel.text = $"Level {level + 1}";
    }
}