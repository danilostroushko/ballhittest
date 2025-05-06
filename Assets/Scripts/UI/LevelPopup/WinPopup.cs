using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class WinPopup : UIView
{
    [Title("Win popup")]
    [SerializeField] private Button backButton;

    public override void Start()
    {
        base.Start();

        backButton.onClick.AddListener(() => GameController.Instance.OpenMainScene());
    }
}
