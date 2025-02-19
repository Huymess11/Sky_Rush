using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Sirenix.Utilities;

public class GridLevelController : SerializedMonoBehaviour
{
    [TabGroup("CREATE GRID")]
    [Title("Grid Settings",titleAlignment: TitleAlignments.Centered)]
    [TabGroup("CREATE GRID")]
    public int width = 5;
    [TabGroup("CREATE GRID")]
    public int height = 5;
    [TabGroup("CREATE GRID")]
    public GridCell panelPrefab;

   [HideInInspector]
    public List<GridCell> cellList;

    #region CREATE GRID
    [TabGroup("CREATE GRID")]
    [GUIColor(0.3f, 0.8f, 0.3f)]
    [Button("CREATE GRID",ButtonSizes.Large)]
    private void GenerateGrid()
    {
        SpawnGridCell();
    }

    [TabGroup("CREATE GRID")]
    [GUIColor(1f, 0.4f, 0.4f)]
    [Button("CLEAR GRID",ButtonSizes.Large)]
    private void ClearGrid()
    {
        if (cellList != null)
        {
            foreach (var child in cellList)
            {
                if (child != null)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
            cellList.Clear();
        }
    }
    private Vector3 CalculateGridCenter()
    {
        float centerX = (width - 1) * 0.5f;
        float centerZ = (height - 1) * 0.5f;
        return new Vector3(centerX, 0, centerZ);
    }
    private void SpawnGridCell()
    {
        ClearGrid();
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        Vector3 gridCenter = CalculateGridCenter();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 position = new Vector3(i + 0.5f, 0, j + 0.5f) - gridCenter;
                GridCell obj = Instantiate(panelPrefab, position, Quaternion.identity, transform);
                obj.name = $"Cell[{i},{j}]";
                obj.transform.SetParent(transform);
                obj.transform.localRotation = Quaternion.Euler(0f,0f,180f);
                obj.SetGridPosition(i,j);
                cellList.Add(obj);
            }
        }
        transform.localRotation = Quaternion.Euler(0f,180f, 180f);
    }
    private void CheckNeibor()
    {
        for (int i = 0; i < cellList.Count; i++)
        {
                cellList[i].CheckNeighbor(cellList);
        }
    }
    #endregion
    #region EDIT GRID
    [TabGroup("EDIT GRID")]
#if UNITY_EDITOR
    [ShowInInspector]
    [TableMatrix(HorizontalTitle = "Custom Cell Drawing", DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = false, RowHeight = 16)]
    public bool[,] CustomCellDrawing;
    private static bool DrawColoredEnumElement(Rect rect, bool value)
    {
      
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            value = !value;
            GUI.changed = true;
            Event.current.Use();
        }
        UnityEditor.EditorGUI.DrawRect(rect.Padding(1), value ? new Color(0.1f, 0.8f, 0.2f) : new Color(0, 0, 0, 0.5f));
        return value;
    }
    [TabGroup("EDIT GRID")]
    [GUIColor(0.3f, 0.8f, 0.3f)]
    [Button("CREATE GRID MATRIX", ButtonSizes.Large)]
    private void CreateGridMatrix()
    {
        CustomCellDrawing = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CustomCellDrawing[x, y] = true;
            }
        }
    }

    [TabGroup("EDIT GRID")]
    [GUIColor(0.3f, 0.8f, 0.3f)]
    [Button("FORCE GRID", ButtonSizes.Large)]
    private void GridComplete()
    {
        SpawnGridCell();
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(!CustomCellDrawing[i, j])
                {
                    foreach(var cell in cellList)
                    {
                        if(cell.gridPosX == i && cell.gridPosY == j)
                        {
                            DestroyImmediate(cell.gameObject);
                            cellList.Remove(cell); 
                            cellList.Remove(cell); 
                            break;
                        }
                    }
                }
            }
        }
        CheckNeibor();
    }

#endif
    #endregion
}


