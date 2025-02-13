using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using Sirenix.Utilities;

public class GridMap : MonoBehaviour
{
    [TabGroup("CREATE GRID")]
    [Title("Grid Settings",titleAlignment: TitleAlignments.Centered)]
    [TabGroup("CREATE GRID")]
    public int width = 5;
    [TabGroup("CREATE GRID")]
    public int height = 5;
    [TabGroup("CREATE GRID")]
    public GameObject panelPrefab; 

    [HideInInspector]
    public Cell[,] grid;
    [HideInInspector]
    public List<GameObject> cellList;

    
    private void Awake()
    {
        GenerateGrid();
    }

    [TabGroup("CREATE GRID")]
    [GUIColor(0.3f, 0.8f, 0.3f)]
    [Button("CREATE GRID",ButtonSizes.Large)]
    private void GenerateGrid()
    {
        ClearGrid();
        
        grid = new Cell[width, height];
        Vector3 gridCenter = CalculateGridCenter();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 position = new Vector3(i, 0, j) - gridCenter;
                Transform obj = Instantiate(panelPrefab.transform, position, Quaternion.identity,transform);
                obj.name = $"Cell[{i},{j}]";
                obj.transform.SetParent(transform);
                grid[i, j] = new Cell(true, position, obj);
                cellList.Add(obj.gameObject);
            }
        }
        CustomCellDrawing = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CustomCellDrawing[x, y] = true;
            }
        }
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
                DestroyImmediate(child);
            }
            cellList.Clear();
        }
    }
    private Vector3 CalculateGridCenter()
    {
        float centerX = (width - 1) * 0.5f;
        float centerZ = (height - 1) * 0.5f/5;
        return new Vector3(centerX, 0, centerZ);
    }
    private void SetCameraSize()
    {

    }
    [TabGroup("EDIT GRID")]
#if UNITY_EDITOR
    [ShowInInspector]
    [TableMatrix(HorizontalTitle = "Custom Cell Drawing", DrawElementMethod = "DrawColoredEnumElement")]
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
    [Button("EDIT GRID COMPLETE", ButtonSizes.Large)]
    private void EditGridComplete()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {

            }
        }
    }

#endif
}


public class Cell
{
    public bool canFill;
    public Vector3 cellPosition;
    public Transform obj;

    public Cell(bool isFill, Vector3 cellPosition,Transform obj)
    {
        this.canFill = isFill;
        this.cellPosition = cellPosition;
        this.obj = obj;
    }
}
