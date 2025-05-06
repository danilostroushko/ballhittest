using DG.Tweening;
using Lean.Pool;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadController : MonoBehaviour
{
    public static RoadController Instance;

    public LevelsData levelsData;

    [SerializeField] private PlayerController player;
    [SerializeField] private TargetItem targetItem;

    [SerializeField] private Transform roadItemsParent;
    [SerializeField] private GameObject obstacleItemPrefab;
    [SerializeField] private Transform decorLeft;
    [SerializeField] private Transform decorRight;

    [Title("Level settings")]
    [SerializeField] private LevelData levelData;

    private float totalWidth;
    private float totalHeight;
    private RoadCell[,] roadPoints;

    private RaycastHit gizmoHit;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
#if UNITY_EDITOR
        Physics.SphereCast(player.transform.position, player.GetRadius, player.transform.forward, out gizmoHit);
#endif
    }

    public void Init(int levelIndex)
    {
        levelData = levelsData.levels[levelIndex];

        CreateGrid();

        decorLeft.position = decorLeft.position.WithX(-(totalWidth * 0.5f));
        decorRight.position = decorRight.position.WithX(totalWidth * 0.5f);

        player.Init(totalWidth, totalHeight);

        player.OnHit += OnPlayerHit;
        player.OnMinFill += OnMinFill;
    }

    private void OnMinFill()
    {
        UIManager.Instance.Open<LosePopup>();
    }

    private void OnPlayerHit(float radius)
    {
        if (Physics.SphereCast(player.transform.position, radius, player.transform.forward, out RaycastHit hit))
        {
            var overlap = Physics.OverlapSphere(hit.point.WithY(0), radius);
            StartCoroutine(PlayHit(hit.point.WithY(0), overlap));
        }
    }

    private IEnumerator PlayHit(Vector3 target, Collider[] overlap)
    {
        yield return new WaitForSeconds(player.MoveHitSphere(target));

        if (overlap.Length > 0)
        {
            foreach (var item in overlap)
            {
                if (item.TryGetComponent<ObstacleItem>(out var comp))
                {
                    comp.DestroyItem();
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        bool isTarget = false;

        if (Physics.SphereCast(player.transform.position, player.GetRadius, player.transform.forward, out RaycastHit hit))
        {
            isTarget = hit.collider.TryGetComponent<TargetItem>(out var comp);
            
            yield return new WaitForSeconds(player.JumpTo(hit.point.z, isTarget));

            if (isTarget) { targetItem.CloseDoors(); }
        }

        player.isMoveHit = isTarget ? true : false;

        if (isTarget)
        {
            UIManager.Instance.Open<WinPopup>();
        }
    }

    private void CreateGrid()
    {
        LeanPool.DespawnAll();

        roadPoints = new RoadCell[levelData.rows, levelData.cols];

        totalWidth = levelData.cols * levelData.cellSize;
        totalHeight = levelData.rows * levelData.cellSize;

        float startX = -((totalWidth / 2f) - levelData.cellHalfSize);
        float startZ = levelData.cellHalfSize;

        float x = startX;
        float z = startZ;

        for (int r = 0; r < levelData.rows; r++)
        {
            for (int c = 0; c < levelData.cols; c++)
            {
                Vector3 pos = new Vector3(x, 0, z);
                Vector2 rp = Random.insideUnitCircle * (levelData.cellHalfSize - levelData.cellInnerOffset);
                Vector3 rPos = new Vector3(pos.x + rp.x, 0, pos.z + rp.y);

                roadPoints[r, c] = new RoadCell() { position = pos, randomPos = rPos };
                x += levelData.cellSize;
            }

            x = startX;
            z += levelData.cellSize;
        }

        PlaceObstacles();
    }

    private void PlaceObstacles()
    {
        int totalRows = roadPoints.GetLength(0);
        int totalCols = roadPoints.GetLength(1);
        System.Random rnd = new System.Random();

        for (int r = 0; r < roadPoints.GetLength(0); r++)
        {
            float curPercent = r.RangeToPercent(0, totalRows - 1);
            float levelFill = levelData.levelCurve.Evaluate(curPercent);
            int colsToShow = Mathf.Clamp(levelFill.PercentToCount(totalCols), 0, totalCols);
            
            List<int> indexes = new List<int>();
            for (int c = 0; c < colsToShow; c++)
            {
                int cIndex = c;
                if (colsToShow < totalCols - 1)
                {
                    do
                    {
                        cIndex = rnd.Next(totalCols);
                    } while (indexes.Contains(cIndex));
                    indexes.Add(cIndex);
                }
                
                var item = LeanPool.Spawn(obstacleItemPrefab, roadItemsParent, false);
                item.transform.position = roadPoints[r, cIndex].randomPos;
                item.transform.Rotate(Vector3.up, Random.Range(0, 359));
            }
        }
    }

    private void OnDrawGizmos()
    {
        /*GizmosExtend.DrawCylinder(player.transform.position, gizmoHit.point.WithX(0), Color.magenta, player.GetRadius);

        Gizmos.color = Color.yellow;
        if (roadPoints != null && roadPoints.Length > 0)
        {
            for (int r = 0; r < roadPoints.GetLength(0); r++)
            {
                for (int c = 0; c < roadPoints.GetLength(1); c++)
                {
                    Gizmos.color = r % 2 == 0 ? Color.red : c % 2 == 0 ? Color.green : Color.blue;

                    Gizmos.DrawWireCube(roadPoints[r, c].position, new Vector3(levelData.cellSize, 0.1f, levelData.cellSize));
                    Gizmos.DrawCube(roadPoints[r, c].randomPos, new Vector3(0.1f, 0.1f, 0.1f));
                }
            }
        }*/
    }
}

public struct RoadCell
{
    public Vector3 position;
    public Vector3 randomPos;
}

[Serializable]
public class LevelData
{
    public int index;
    public int cols;
    public int rows;
    public float cellSize;
    public float cellHalfSize => cellSize * 0.5f;
    public float cellInnerOffset;
    public AnimationCurve levelCurve;
}