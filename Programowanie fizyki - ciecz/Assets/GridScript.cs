using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridScript : MonoBehaviour
{

    public GameObject CellObject;
    public int width = 128;
    public int height = 128;

    public Cell[,] cells;

    void Start()
    {
        cells = new Cell[width, height];
        GenerateGrid();
        GetComponent<Fluid>().SetCells(cells);
        GetComponent<Fluid>().enabled = true;
        UpdateDisplayText(Cell.displayMode);
    }

    void GenerateGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                GameObject tmp = Instantiate(CellObject, this.transform);
                tmp.transform.position = new Vector3(x, y, 0.0f);
                tmp.transform.GetChild(0).position = new Vector3(0.0f, 0.0f, -1.0f);
                cells[x, y] = tmp.GetComponent<Cell>();

            }
        }
    }

    void UpdateAllCells()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[x, y].UpdateCellLook();
            }
        }
    }


    public Slider slider;

    public void ChangeBrushPower()
    {
        Cell.ChangeBrushPower(slider.value);
    }

    public void ChangeTool()
    {
        Cell.SwapTools();
    }

    public Text displayText;

    public void ChangeDisplayMode()
    {
        DisplayMode newDisplay = Cell.displayMode;
        if (newDisplay == DisplayMode.WATER_LEVEL) newDisplay = DisplayMode.FLOW_POWER;
        else if (newDisplay == DisplayMode.FLOW_POWER) newDisplay = DisplayMode.WATER_LEVEL;

        UpdateDisplayText(newDisplay);

        Cell.ChangeDisplayMode(newDisplay);
        UpdateAllCells();
    }

    public void UpdateDisplayText(DisplayMode mode)
    {
        displayText.text = "Display Mode: " + DisplayMode.GetName(typeof(DisplayMode), mode);
    }

  
    
}
