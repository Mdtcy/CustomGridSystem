using System.Collections.Generic;
using UnityEngine;

namespace Mdtcy.Grid
{
    /// <summary>
    /// 显示Prefab的Grid
    /// </summary>
    public class GridPrefabVisual : MonoBehaviour
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static GridPrefabVisual Instance
        {
            get;
            private set;
        }

        #region Field

        // VisualNode 预制体
        [SerializeField]
        private Transform pfGridPrefabVisualNode = null;

        private List<Transform>              visualNodeList;
        private Transform[,]                 visualNodeArray;
        private Grid<GridPrefabVisualObject> grid;
        private bool                         updateVisual;

        #endregion


        #region Public Method

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="grid"></param>
        public void Setup(Grid<GridPrefabVisualObject> grid)
        {
            this.grid       = grid;
            visualNodeArray = new Transform[grid.GetWidth(), grid.GetHeight()];

            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    Vector3 gridPosition =
                        new Vector3(x, y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f;
                    Transform visualNode = CreateVisualNode(gridPosition);
                    visualNodeArray[x, y] = visualNode;
                    visualNodeList.Add(visualNode);
                }
            }

            HideNodeVisuals();

            grid.OnGridObjectChanged += Grid_OnGridObjectChanged;
        }

        /// <summary>
        /// 更新GridObject显示，通过SetupVisualNode
        /// </summary>
        /// <param name="grid"></param>
        public void UpdateVisual(Grid<GridPrefabVisualObject> grid)
        {
            HideNodeVisuals();

            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    GridPrefabVisualObject gridObject = grid.GetGridObject(x, y);

                    Transform visualNode = visualNodeArray[x, y];
                    visualNode.gameObject.SetActive(true);
                    SetupVisualNode(visualNode);
                }
            }
        }

        #endregion

        #region Private Method

        private void Awake()
        {
            Instance       = this;
            visualNodeList = new List<Transform>();
        }

        private void Update()
        {
            // todo 这里不直接更改而是在Update检查
            if (updateVisual)
            {
                updateVisual = false;
                UpdateVisual(grid);
            }
        }

        // GridObject改变回调

        private void Grid_OnGridObjectChanged(object sender,
                                              Grid<GridPrefabVisualObject>.OnGridObjectChangedEventArgs e)
        {
            updateVisual = true;
        }

        // 隐藏所有的GridObject
        private void HideNodeVisuals()
        {
            foreach (Transform visualNodeTransform in visualNodeList)
            {
                visualNodeTransform.gameObject.SetActive(false);
            }
        }

        // 创建prefab的方法 TODO 这里可以增加一个创建fun,使用这个来解决注入，对象池之类的问题
        private Transform CreateVisualNode(Vector3 position)
        {
            Transform visualNodeTransform = Instantiate(pfGridPrefabVisualNode, position, Quaternion.identity);

            return visualNodeTransform;
        }

        // 更新VisualNode
        private void SetupVisualNode(Transform visualNodeTransform)
        {
        }
        #endregion

        /// <summary>
        /// Represents a single Grid Object
        /// </summary>
        public class GridPrefabVisualObject
        {

            private Grid<GridPrefabVisualObject> grid;
            private int                          x;
            private int                          y;
            private int                          value;

            public GridPrefabVisualObject(Grid<GridPrefabVisualObject> grid, int x, int y)
            {
                this.grid = grid;
                this.x    = x;
                this.y    = y;
            }

            public void SetValue(int value)
            {
                this.value = value;
                grid.TriggerGridObjectChanged(x, y);
            }

            public override string ToString()
            {
                return x + "," + y + "\n" + value.ToString();
            }

        }
    }
}

