using System;
using System.Collections.Generic;
using Mdtcy.Utils;
using UnityEngine;

namespace Mdtcy.GridBuildingSystem.Scripts._
{
    /// <summary>
    /// GridBuildingSystem2D 单例
    /// </summary>
    public class GridBuildingSystem2D : MonoBehaviour
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static GridBuildingSystem2D Instance { get; private set; }

        public event EventHandler OnSelectedChanged;
        public event EventHandler OnObjectPlaced;


        private                  Grid<GridObject>         grid;

        // 可以被建造的建筑列表
        [SerializeField]
        private List<PlacedObjectTypeSO> placedObjectTypeSOList = new List<PlacedObjectTypeSO>();


        [SerializeField]
        private int gridWidth = 10;

        [SerializeField]
        private int gridHeight = 10;

        [SerializeField]
        private float cellSize = 10f;

        // 当前选中的要建造的建筑类型
        private                  PlacedObjectTypeSO       selectedPlacedObjectTypeSO;
        private                  PlacedObjectTypeSO.Dir   dir;

        private void Awake()
        {
            Instance = this;
            grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

            selectedPlacedObjectTypeSO = null;
        }

        public class GridObject
        {

            private Grid<GridObject>  grid;
            private int               x;
            private int               y;
            public  PlacedObject_Done placedObject;

            public GridObject(Grid<GridObject> grid, int x, int y)
            {
                this.grid    = grid;
                this.x       = x;
                this.y       = y;
                placedObject = null;
            }

            public override string ToString()
            {
                return x + ", " + y + "\n" + placedObject;
            }

            public void SetPlacedObject(PlacedObject_Done placedObject)
            {
                this.placedObject = placedObject;
                grid.TriggerGridObjectChanged(x, y);
            }

            public void ClearPlacedObject()
            {
                placedObject = null;
                grid.TriggerGridObjectChanged(x, y);
            }

            public PlacedObject_Done GetPlacedObject()
            {
                return placedObject;
            }

            public bool CanBuild()
            {
                return placedObject == null;
            }

        }

        private void Update()
        {
            // 如果有选中的建筑并且点击左键
            if (Input.GetMouseButtonDown(0) && selectedPlacedObjectTypeSO != null)
            {
                // 根据鼠标位置获取对应Grid的xz
                Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
                grid.GetXY(mousePosition, out int x, out int z);

                Vector2Int placedObjectOrigin = new Vector2Int(x, z);

                // 如果可以在鼠标对应的xz位置建造
                if (CanBuild(selectedPlacedObjectTypeSO, placedObjectOrigin))
                {
                    Build(selectedPlacedObjectTypeSO, placedObjectOrigin);
                    //DeselectObjectType();
                }
                else
                {
                    // Cannot build here
                    UtilsClass.CreateWorldTextPopup("Cannot Build Here!", mousePosition);
                }
            }


            // todo 替换成UI

            #region 建造，移除

            if (Input.GetKeyDown(KeyCode.R))
            {
                dir = PlacedObjectTypeSO.GetNextDir(dir);
            }

            // 切换当前选中的建筑类型
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedPlacedObjectTypeSO = placedObjectTypeSOList[0];
                RefreshSelectedObjectType();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                selectedPlacedObjectTypeSO = placedObjectTypeSOList[1];
                RefreshSelectedObjectType();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                selectedPlacedObjectTypeSO = placedObjectTypeSOList[2];
                RefreshSelectedObjectType();
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                selectedPlacedObjectTypeSO = placedObjectTypeSOList[3];
                RefreshSelectedObjectType();
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                selectedPlacedObjectTypeSO = placedObjectTypeSOList[4];
                RefreshSelectedObjectType();
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                selectedPlacedObjectTypeSO = placedObjectTypeSOList[5];
                RefreshSelectedObjectType();
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                DeselectObjectType();
            }


            if (Input.GetMouseButtonDown(1))
            {
                Vector3           mousePosition = UtilsClass.GetMouseWorldPosition();
                PlacedObject_Done placedObject  = grid.GetGridObject(mousePosition).GetPlacedObject();

                if (placedObject != null)
                {
                    // Demolish
                    placedObject.DestroySelf();

                    List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();

                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                    }
                }
            }

            #endregion
        }


        /// <summary>
        /// placedObjectTypeSo是否能在placedObjectOrigin这个位置建造
        /// </summary>
        /// <param name="placedObjectTypeSo"></param>
        /// <param name="placedObjectOrigin"></param>
        /// <returns></returns>
        private bool CanBuild(PlacedObjectTypeSO placedObjectTypeSo , Vector2Int placedObjectOrigin)
        {
            bool canBuild = true;

            foreach (Vector2Int gridPosition in placedObjectTypeSo.GetGridPositionList(placedObjectOrigin, dir))
            {
                if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {
                    canBuild = false;

                    break;
                }
            }

            return canBuild;
        }

        /// <summary>
        /// 在placedObjectOrigin建造placedObjectTypeSo类型的建筑
        /// </summary>
        /// <param name="placedObjectTypeSo"></param>
        /// <param name="placedObjectOrigin"></param>
        private void Build(PlacedObjectTypeSO placedObjectTypeSo, Vector2Int placedObjectOrigin)
        {
            Vector2Int       rotationOffset   = placedObjectTypeSo.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) +
                                                new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();

            PlacedObject_Done placedObject =
                PlacedObject_Done.Create(placedObjectWorldPosition, placedObjectOrigin, dir,
                                         placedObjectTypeSo);
            placedObject.transform.rotation = Quaternion.Euler(0, 0, -placedObjectTypeSo.GetRotationAngle(dir));

            foreach (Vector2Int gridPosition in placedObjectTypeSo.GetGridPositionList(placedObjectOrigin, dir))
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }

            OnObjectPlaced?.Invoke(this, EventArgs.Empty);
        }

        private void DeselectObjectType() {
            selectedPlacedObjectTypeSO = null; RefreshSelectedObjectType();
        }

        private void RefreshSelectedObjectType()
        {
            OnSelectedChanged?.Invoke(this, EventArgs.Empty);
        }


        public Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            grid.GetXY(worldPosition, out int x, out int z);
            return new Vector2Int(x, z);
        }

        public Vector3 GetMouseWorldSnappedPosition()
        {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            grid.GetXY(mousePosition, out int x, out int y);

            if (selectedPlacedObjectTypeSO != null) {
                Vector2Int rotationOffset = selectedPlacedObjectTypeSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();
                return placedObjectWorldPosition;
            } else {
                return mousePosition;
            }
        }

        public Quaternion GetPlacedObjectRotation() {
            if (selectedPlacedObjectTypeSO != null) {
                return Quaternion.Euler(0, 0, -selectedPlacedObjectTypeSO.GetRotationAngle(dir));
            } else {
                return Quaternion.identity;
            }
        }

        public PlacedObjectTypeSO GetPlacedObjectTypeSO()
        {
            return selectedPlacedObjectTypeSO;
        }

    }
}
