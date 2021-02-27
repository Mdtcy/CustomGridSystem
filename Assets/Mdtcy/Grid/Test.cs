using UnityEngine;

namespace Mdtcy.Grid
{
    public class Test : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            // Grid<GridPrefabVisual.GridPrefabVisualObject> grid =
            //     new Grid<GridPrefabVisual.GridPrefabVisualObject>(10, 10, 10,
            //                                                       Vector3.zero, (g, x, y) => new
            //                                                           GridPrefabVisual.GridPrefabVisualObject(g, x, y));

            // GridPrefabVisual.Instance.Setup(grid);
            // GridPrefabVisual.Instance.UpdateVisual(grid);
            Grid<GridGradientVisual.GridGradientVisualObject> grid1 =
                new Grid<GridGradientVisual.GridGradientVisualObject>(10, 10, 5,
                                                   Vector3.zero, (g, x, y) => new
                                                       GridGradientVisual.GridGradientVisualObject(g, x, y));

            GridGradientVisual d = GetComponent<GridGradientVisual>();
            d.SetGrid(grid1);

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
