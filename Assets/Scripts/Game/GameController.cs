using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void OpenMainScene()
    {
        StartCoroutine(InnerOpenMainScene());
    }

    private IEnumerator InnerOpenMainScene()
    {
        UIManager.Instance.ShowFade();
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.HideAll(true);

        var op = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
        op.completed += (v) =>
        {
            UIManager.Instance.Open<MainScreen>();
            UIManager.Instance.HideFade();
        };
    }

    public void OpenGameScene(int levelIndex)
    {
        StartCoroutine(InnerOpenGameScene(levelIndex));
    }

    public IEnumerator InnerOpenGameScene(int levelIndex)
    {
        UIManager.Instance.ShowFade();
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.HideAll(true);

        var op = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);

        while (!op.isDone)
        {
            yield return null;
        }

        RoadController.Instance.Init(levelIndex);
        yield return new WaitForSeconds(0.5f);
        
        UIManager.Instance.Open<GameScreen>().SetLevel(levelIndex);
        UIManager.Instance.HideFade();
    }
}