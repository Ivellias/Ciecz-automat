using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fluid : MonoBehaviour
{

    Cell[,] cells;
    float[,] Diff;
    int width = 0;
    int height = 0;

    float compression = 0.05f;

    float minFlow = 0.02f;
    float maxFlow = 4.0f;
    public float flowSpeed = 1.0f;

    public Text waterText;

    public float remainingLimit = 0.01f;

    public void SetCells(Cell[,] cells)
    {
        this.cells = cells;
        width = cells.GetLength(0);
        height = cells.GetLength(1);
        Debug.Log(width);
        Debug.Log(height);
    }

    void Start()
    {
        ResetAll();
    }


    public void ResetAll()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(cells[x, y].waterLevel != 0.0f || cells[x, y].solidBlock) cells[x, y].ResetCell();
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) cells[x, y].ChangeToSolidBlock();
            }
        }
        compressionSlider.value = 0.05f;
        UpdateCompression();
        flowSpeedSlider.value = 1.0f;
        UpdateFlowSpeed();

    }

    void FixedUpdate()
    {
        Diff = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (cells[x, y].solidBlock) continue;
                if (cells[x, y].waterLevel == 0.0f) continue;

                float startValue = cells[x, y].waterLevel;
                float remainingValue = cells[x, y].waterLevel;
                float flow = 0;


                //w dol
                if(y > 0) //czy istnieje w ogole
                {
                    if(!cells[x, y - 1].solidBlock) //nie moze byc sciana
                    {
                        flow = VerticalFlowAmount(cells[x, y].waterLevel, cells[x, y - 1].waterLevel) - cells[x, y - 1].waterLevel;
                        if (flow > minFlow)
                            flow *= flowSpeed;

                        if (flow < 0) flow = 0.0f;
                        if (flow > Mathf.Min(maxFlow, cells[x, y].waterLevel)) flow = Mathf.Min(maxFlow, cells[x, y].waterLevel);
                        if(flow != 0)
                        {
                            remainingValue -= flow;
                            Diff[x, y] -= flow;
                            Diff[x, y - 1] += flow;
                        }
                    }
                }

                if (remainingValue < remainingLimit) //jesli jest na niskim poziomie to nie liczmy dalej i oddajemy do komorki
                {
                    Diff[x, y] += remainingValue;
                    remainingValue = 0.0f;
                    continue;
                }


                                 
                //w lewo
                if(x > 0)//czy istnieje
                {
                    if (!cells[x - 1, y].solidBlock)//nie jest sciana
                    {
                        flow = (remainingValue - cells[x - 1, y].waterLevel) / 4.0f;
                        if (flow > minFlow)
                            flow *= flowSpeed;

                        if (flow < 0) flow = 0.0f;
                        if (flow > Mathf.Min(maxFlow, remainingValue)) flow = Mathf.Min(maxFlow, remainingValue);

                        if (flow != 0)
                        {
                            remainingValue -= flow;
                            Diff[x, y] -= flow;
                            Diff[x - 1, y] += flow;
                        }
                    }
                }



                if (remainingValue < remainingLimit) //jesli jest na niskim poziomie to nie liczmy dalej i oddajemy do komorki
                {
                    Diff[x, y] += remainingValue;
                    remainingValue = 0.0f;
                    continue;
                }

                    //w prawo
                    if (x < width - 1)//czy istnieje
                    {
                        if (!cells[x + 1, y].solidBlock)//nie jest sciana
                        {
                            flow = (remainingValue - cells[x + 1, y].waterLevel) / 3.0f;
                            if (flow > minFlow)
                                flow *= flowSpeed;

                            if (flow < 0) flow = 0.0f;
                            if (flow > Mathf.Min(maxFlow, remainingValue)) flow = Mathf.Min(maxFlow, remainingValue);

                            if (flow != 0)
                            {
                                remainingValue -= flow;
                                Diff[x, y] -= flow;
                                Diff[x + 1, y] += flow;
                            }
                        }
                    }
                    


                if (remainingValue < remainingLimit) //jesli jest na niskim poziomie to nie liczmy dalej i oddajemy do komorki
                {
                    Diff[x, y] += remainingValue;
                    remainingValue = 0.0f;
                    continue;
                }




                //w gore
                if (y < height - 1)//czy istnieje
                {
                    if (!cells[x, y + 1].solidBlock)//nie jest sciana
                    {
                        flow = remainingValue - VerticalFlowAmount(remainingValue, cells[x, y + 1].waterLevel);
                        if (flow > minFlow)
                            flow *= flowSpeed;

                        // constrain flow
                        flow = Mathf.Max(flow, 0);
                        if (flow > Mathf.Min(maxFlow, remainingValue)) flow = Mathf.Min(maxFlow, remainingValue);

                        // Adjust values
                        if (flow != 0)
                        {
                            remainingValue -= flow;
                            Diff[x, y] -= flow;
                            Diff[x, y + 1] += flow;
                        }

                    }
                }

                if (remainingValue < remainingLimit) //jesli jest na niskim poziomie to nie liczmy dalej i oddajemy do komorki
                {
                    Diff[x, y] += remainingValue;
                    remainingValue = 0.0f;
                    continue;
                }


            }
        }


        float waterSumLevel = 0.0f;
        //rysowanie
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y].waterLevel += Diff[x, y];
                cells[x, y].flowPower = Diff[x, y];
                if (cells[x, y].waterLevel < 0.001f) cells[x, y].waterLevel = 0.0f;

                waterSumLevel += cells[x, y].waterLevel;
                cells[x, y].UpdateCellLook();
            }
        }

        waterText.text = "Water level: " + waterSumLevel;

    }

    public float VerticalFlowAmount(float remainingWater, float destinationWater)
    {
        float amount = 0.0f;
        float sum = remainingWater + destinationWater;

        if (sum <= 1.0f) //jesli jest miejsce na calosc to calosc
        {
            amount = 1.0f;
        }
        else if (sum < 2.0f + compression)
        {
            amount = (1.0f + sum * compression) / (1.0f + compression);
        }
        else
        {
            amount = (sum + compression) / 2.0f;
        }


        return amount;
    }

    public Slider flowSpeedSlider;
    public Slider compressionSlider;


    public void UpdateFlowSpeed()
    {
        flowSpeed = flowSpeedSlider.value;
    }
    public void UpdateCompression()
    {
        compression = compressionSlider.value;
    }

}


