using System;
using System.Collections.Generic;
using UnityEngine;

internal class TickHandler
{
    private int m_BiggestTick = -1;
    private float m_MaxValue = 1f;
    private float m_MinValue;
    private float m_PixelRange = 1f;
    private int m_SmallestTick;
    private float[] m_TickModulos = new float[0];
    private float[] m_TickStrengths = new float[0];

    internal int GetLevelWithMinSeparation(float pixelSeparation)
    {
        for (int i = 0; i < this.m_TickModulos.Length; i++)
        {
            if (((this.m_TickModulos[i] * this.m_PixelRange) / (this.m_MaxValue - this.m_MinValue)) >= pixelSeparation)
            {
                return (i - this.m_SmallestTick);
            }
        }
        return -1;
    }

    internal float GetPeriodOfLevel(int level)
    {
        return this.m_TickModulos[Mathf.Clamp(this.m_SmallestTick + level, 0, this.m_TickModulos.Length - 1)];
    }

    internal float GetStrengthOfLevel(int level)
    {
        return this.m_TickStrengths[this.m_SmallestTick + level];
    }

    internal float[] GetTicksAtLevel(int level, bool excludeTicksFromHigherlevels)
    {
        int index = Mathf.Clamp(this.m_SmallestTick + level, 0, this.m_TickModulos.Length - 1);
        List<float> list = new List<float>();
        int num2 = Mathf.CeilToInt(this.m_MaxValue / this.m_TickModulos[index]);
        for (int i = Mathf.FloorToInt(this.m_MinValue / this.m_TickModulos[index]); i <= num2; i++)
        {
            if ((!excludeTicksFromHigherlevels || (index >= this.m_BiggestTick)) || ((i % Mathf.RoundToInt(this.m_TickModulos[index + 1] / this.m_TickModulos[index])) != 0))
            {
                list.Add(i * this.m_TickModulos[index]);
            }
        }
        return list.ToArray();
    }

    internal void SetRanges(float minValue, float maxValue, float minPixel, float maxPixel)
    {
        this.m_MinValue = minValue;
        this.m_MaxValue = maxValue;
        this.m_PixelRange = maxPixel - minPixel;
    }

    internal void SetTickModulos(float[] tickModulos)
    {
        this.m_TickModulos = tickModulos;
    }

    internal void SetTickModulosForFrameRate(float frameRate)
    {
        if (frameRate != Mathf.Round(frameRate))
        {
            float[] tickModulos = new float[] { 1f / frameRate, 5f / frameRate, 10f / frameRate, 50f / frameRate, 100f / frameRate, 500f / frameRate, 1000f / frameRate, 5000f / frameRate, 10000f / frameRate, 50000f / frameRate, 100000f / frameRate, 500000f / frameRate };
            this.SetTickModulos(tickModulos);
        }
        else
        {
            List<int> list = new List<int>();
            int item = 1;
            while (item < frameRate)
            {
                if (item == frameRate)
                {
                    break;
                }
                int num2 = Mathf.RoundToInt(frameRate / ((float) item));
                if ((num2 % 60) == 0)
                {
                    item *= 2;
                    list.Add(item);
                }
                else
                {
                    if ((num2 % 30) == 0)
                    {
                        item *= 3;
                        list.Add(item);
                        continue;
                    }
                    if ((num2 % 20) == 0)
                    {
                        item *= 2;
                        list.Add(item);
                        continue;
                    }
                    if ((num2 % 10) == 0)
                    {
                        item *= 2;
                        list.Add(item);
                        continue;
                    }
                    if ((num2 % 5) == 0)
                    {
                        item *= 5;
                        list.Add(item);
                        continue;
                    }
                    if ((num2 % 2) == 0)
                    {
                        item *= 2;
                        list.Add(item);
                        continue;
                    }
                    if ((num2 % 3) == 0)
                    {
                        item *= 3;
                        list.Add(item);
                        continue;
                    }
                    item = Mathf.RoundToInt(frameRate);
                }
            }
            float[] numArray2 = new float[9 + list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                numArray2[i] = 1f / ((float) list[(list.Count - i) - 1]);
            }
            numArray2[numArray2.Length - 1] = 3600f;
            numArray2[numArray2.Length - 2] = 1800f;
            numArray2[numArray2.Length - 3] = 600f;
            numArray2[numArray2.Length - 4] = 300f;
            numArray2[numArray2.Length - 5] = 60f;
            numArray2[numArray2.Length - 6] = 30f;
            numArray2[numArray2.Length - 7] = 10f;
            numArray2[numArray2.Length - 8] = 5f;
            numArray2[numArray2.Length - 9] = 1f;
            this.SetTickModulos(numArray2);
        }
    }

    internal void SetTickStrengths(float tickMinSpacing, float tickMaxSpacing, bool sqrt)
    {
        this.m_TickStrengths = new float[this.m_TickModulos.Length];
        this.m_SmallestTick = 0;
        this.m_BiggestTick = this.m_TickModulos.Length - 1;
        for (int i = this.m_TickModulos.Length - 1; i >= 0; i--)
        {
            float num2 = (this.m_TickModulos[i] * this.m_PixelRange) / (this.m_MaxValue - this.m_MinValue);
            this.m_TickStrengths[i] = (num2 - tickMinSpacing) / (tickMaxSpacing - tickMinSpacing);
            if (this.m_TickStrengths[i] >= 1f)
            {
                this.m_BiggestTick = i;
            }
            if (num2 <= tickMinSpacing)
            {
                this.m_SmallestTick = i;
                break;
            }
        }
        for (int j = this.m_SmallestTick; j <= this.m_BiggestTick; j++)
        {
            this.m_TickStrengths[j] = Mathf.Clamp01(this.m_TickStrengths[j]);
            if (sqrt)
            {
                this.m_TickStrengths[j] = Mathf.Sqrt(this.m_TickStrengths[j]);
            }
        }
    }

    internal int tickLevels
    {
        get
        {
            return ((this.m_BiggestTick - this.m_SmallestTick) + 1);
        }
    }
}

