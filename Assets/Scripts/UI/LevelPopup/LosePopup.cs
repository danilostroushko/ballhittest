using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class LosePopup : UIView
{
    [Title("Lose popup")]
    [SerializeField] private Button backButton;

    public override void Start()
    {
        base.Start();

        backButton.onClick.AddListener(() => GameController.Instance.OpenMainScene());
    }
}
