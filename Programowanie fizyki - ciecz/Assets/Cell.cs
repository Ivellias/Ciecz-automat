using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DisplayMode {WATER_LEVEL, FLOW_POWER}

public class Cell : MonoBehaviour
{
    static float brushPower = 5.0f;

    public Transform mask;
    public SpriteRenderer renderer;
    public SpriteRenderer maskRenderer;
    public float waterLevel = 0.0f;

    public float flowPower = 0.0f;

    public Color wallColor;
    public Color airColor;

    public bool solidBlock = false;

    public static DisplayMode displayMode = DisplayMode.WATER_LEVEL;
    

    void Start()
    {
        mask = transform.GetChild(0);
        maskRenderer = mask.gameObject.GetComponent<SpriteRenderer>();
        UpdateCellLook();
    }

    public void UpdateCellLook()
    {
        if (waterLevel < 0.005f) waterLevel = 0.0f;
        float tmp = waterLevel;
        if (tmp > 1.0f) tmp = 1.0f;
        if (waterLevel >= 1.0f)
        {
            maskRenderer.color = airColor;

            switch (displayMode)
            {
                case DisplayMode.WATER_LEVEL:
                    {
                        renderer.color = UnderwaterLevelColor();
                        break;
                    }
                case DisplayMode.FLOW_POWER:
                    {
                        renderer.color = FlowPowerColor();
                        break;
                    }
                default:
                    {
                        renderer.color = UnderwaterLevelColor();
                        break;
                    }
            } 

        }
        else 
        {
            if(!solidBlock) maskRenderer.color = new Color(airColor.r, airColor.g, airColor.b, (3.0f - waterLevel)/3.0f);

            switch (displayMode)
            {
                case DisplayMode.WATER_LEVEL:
                    {
                        renderer.color = WaterLevelColor();
                        break;
                    }
                case DisplayMode.FLOW_POWER:
                    {
                        renderer.color = FlowPowerColor();
                        break;
                    }
                default:
                    {
                        renderer.color = WaterLevelColor();
                        break;
                    }
            }

            

        }


        mask.position = new Vector3(this.transform.position.x, this.transform.position.y + tmp/2.0f, -1.0f);
        mask.localScale = new Vector3(1.0f, 1.0f-tmp, 1.0f);
    }

    public void AddWater(float amount)
    {
        waterLevel += amount;
        if (solidBlock)
        {
            solidBlock = false;
            maskRenderer.color = airColor;
        }

        if(waterLevel < 0.005f)
        {
            waterLevel = 0.0f;
        }
        UpdateCellLook();
    }

    public void ChangeToSolidBlock()
    {
        solidBlock = true;
        if (mask != null) mask.gameObject.GetComponent<SpriteRenderer>().color = wallColor;
        else StartCoroutine(BlackIn());

        waterLevel = 0.0f;
    }

    public IEnumerator BlackIn()
    {
        bool done = false;
        while (!done)
        {
            yield return new WaitForFixedUpdate();
            if (mask != null)
            {
                mask.gameObject.GetComponent<SpriteRenderer>().color = wallColor;
                done = true;
            }
        }
    }

    public void ResetCell()
    {
        waterLevel = 0.0f;
        solidBlock = false;
        maskRenderer.color = airColor;
        UpdateCellLook();
    }



    static bool pressing = false;
    static bool staticWallTool = false;
    private void OnMouseDown()
    {
        pressing = true;
    }

    private void OnMouseUp()
    {
        pressing = false;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) ChangeToSolidBlock();
        if (pressing)
        {
            if (staticWallTool) ChangeToSolidBlock();
            else AddWater(brushPower * Time.deltaTime);
        }
    }

    public static void SwapTools()
    {
        staticWallTool = !staticWallTool;
    }

    public static void ChangeBrushPower(float newPower)
    {
        brushPower = newPower;
    }

    public static void ChangeDisplayMode(DisplayMode newDisplay)
    {
        displayMode = newDisplay;
    }


    Color WaterLevelColor()
    {
        return new Color((1.0f / (waterLevel / 2.0f)) , (1.0f / (waterLevel / 2.0f)), (1.0f / (waterLevel / 2.0f)), 1.0f);
    }

    Color UnderwaterLevelColor()
    {
        return new Color((0.7f / (waterLevel - 0.6f)) , (0.7f / (waterLevel - 0.6f)), (0.7f / (waterLevel - 0.6f)), 1.0f);
    }

    Color FlowPowerColor()
    {
        return new Color(flowPower*10.0f, flowPower * 2.0f, flowPower/2.0f, 1.0f);
    }



}
