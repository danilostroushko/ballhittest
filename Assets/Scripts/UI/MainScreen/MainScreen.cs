using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MainScreen : UIView
{
    [Title("Main screen")]
    [SerializeField] private Button exitButton;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private ListLevelItem levelItemPrefab;
    [Space]
    [SerializeField] private LevelsData levelsData;

    public override void Start()
    {
        base.Start();

        exitButton.onClick.AddListener(() => { Application.Quit(); });

        for (int i = 0; i < levelsData.levels.Count; i++) 
        {
            var item = Instantiate(levelItemPrefab, scrollRect.content, false);
            item.SetLevelData(levelsData.levels[i]);
            item.OnClickEvent += OnClickLevel;
        }
    }

    private void OnClickLevel(int levelIndex)
    {
        canvasGroup.interactable = false;
        GameController.Instance.OpenGameScene(levelIndex);
    }
}
