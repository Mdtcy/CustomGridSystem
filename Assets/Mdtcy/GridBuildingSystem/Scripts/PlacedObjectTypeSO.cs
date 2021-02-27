using System.Collections.Generic;
using UnityEngine;

namespace Mdtcy.GridBuildingSystem.Scripts
{
    [CreateAssetMenu(fileName = "PlacedObjectTypeSO", menuName = "MyPlacedObjectTypeSO", order = 1)]
    public class PlacedObjectTypeSO : ScriptableObject
    {
        /// <summary>
        /// 获取下一个方向
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Dir GetNextDir(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Down:      return Dir.Left;
                case Dir.Left:      return Dir.Up;
                case Dir.Up:        return Dir.Right;
                case Dir.Right:     return Dir.Down;
            }
        }

        /// <summary>
        /// 放置的物体的方向
        /// </summary>
        public enum Dir
        {
            Down,
            Left,
            Up,
            Right,
        }

        public string    nameString;
        public Transform prefab;
        public Transform visual;
        public int       width;
        public int       height;


        /// <summary>
        /// 不同方向对应的要旋转的角度
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public int GetRotationAngle(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Down:  return 0;
                case Dir.Left:  return 90;
                case Dir.Up:    return 180;
                case Dir.Right: return 270;
            }
        }

        /// <summary>
        /// 不同方向对应的偏移量
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public Vector2Int GetRotationOffset(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Down:  return new Vector2Int(0, 0);
                case Dir.Left:  return new Vector2Int(0, width);
                case Dir.Up:    return new Vector2Int(width, height);
                case Dir.Right: return new Vector2Int(height, 0);
            }
        }

        public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir)
        {
            List<Vector2Int> gridPositionList = new List<Vector2Int>();
            switch (dir)
            {
                default:
                case Dir.Down:
                case Dir.Up:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }
                    break;
                case Dir.Left:
                case Dir.Right:
                    for (int x = 0; x < height; x++)
                    {
                        for (int y = 0; y < width; y++)
                        {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }
                    break;
            }
            return gridPositionList;
        }

    }
}
