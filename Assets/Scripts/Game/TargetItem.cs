using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class TargetItem : MonoBehaviour
{
    [SerializeField] private Transform door_1;
    [SerializeField] private Transform door_2;

    private void Start()
    {
        door_1.localEulerAngles = Vector3.zero;
        door_2.localEulerAngles = Vector3.zero;
    }

    [Button]
    public void OpenDoors()
    {
        door_1.DOLocalRotate(Vector3.up * 110f, 1f, RotateMode.Fast).SetEase(Ease.OutBack);
        door_2.DOLocalRotate(Vector3.up * -110f, 1f, RotateMode.Fast).SetEase(Ease.OutBack);
    }

    [Button]
    public void CloseDoors()
    {
        door_1.DOLocalRotate(Vector3.zero, 0.7f, RotateMode.Fast).SetEase(Ease.Linear);
        door_2.DOLocalRotate(Vector3.zero, 0.7f, RotateMode.Fast).SetEase(Ease.Linear);
    }
}