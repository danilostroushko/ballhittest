using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Initialization : MonoBehaviour
{

    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject UIManagerPrefab;

    private void Start()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;

        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        GameObject gc = new GameObject("[GameController]", typeof(GameController));
        DontDestroyOnLoad(gc);

        UIManager uiManager = Instantiate(UIManagerPrefab).GetComponent<UIManager>();
        yield return uiManager.Initialize();
        yield return new WaitForEndOfFrame();

        yield return progressSlider.DOValue(1f, 2f).SetEase(Ease.Linear).WaitForCompletion();
        yield return new WaitForSeconds(1f);

        
        yield return new WaitForSeconds(0.5f);
        GameController.Instance.OpenMainScene();
    }
}