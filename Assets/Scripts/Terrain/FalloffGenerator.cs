using UnityEngine;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

public static class FalloffGenerator {

    public static Dictionary<bool[,], float[,]> borderTypes = new Dictionary<bool[,], float[,]>();

    public static float[,] GenerateFalloffMap(int size, bool[,] borders)
    {

        RemoveRedundantBorders(borders);

        foreach (var borderType in borderTypes)
        {
            if (CompareBorders(borderType.Key, borders))
            {
                return borderType.Value;
            }
        }

        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = 0;

                for (int borderY = 0; borderY < 3; borderY++)
                {
                    for (int borderX = 0; borderX < 3; borderX++)
                    {
                        if (!borders[borderX, borderY])
                        {
                            if ((borderX + borderY) % 2 == 0)
                                value = Mathf.Max(value, CornerValue(new Vector2(x, y), new Vector2(borderX - 1, borderY - 1)));
                            else
                                value = Mathf.Max(value, EdgeValue(new Vector2(x, y), new Vector2(borderX - 1, borderY - 1)));
                        }
                    }
                }

                map[i, j] = value; //Evaluate(value);
            }
        }

        borderTypes.Add(borders, map);

        return map;

    }

    static float EdgeValue(Vector2 coords, Vector2 edgeDir)
    {
        return Mathf.Clamp01((Mathf.Abs(edgeDir.x) > 0) ? coords.x * edgeDir.x : coords.y * edgeDir.y);
    }

    static float CornerValue(Vector2 coords, Vector2 cornerDir)
    {
        return Mathf.Clamp01(Mathf.Min(coords.x * cornerDir.x, coords.y * cornerDir.y));
    }

    static void RemoveRedundantBorders(bool[,] borders)
    {

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if ((x + y) % 2 != 0 && !borders[x, y])
                {
                    if (x == 1)
                        borders[x - 1, y] = borders[x + 1, y] = true;
                    else if (y == 1)
                        borders[x, y - 1] = borders[x, y + 1] = true;
                }
            }
        }

        //if (borders[1, 0])
        //    borders[0, 0] = borders[2, 0] = false;

        //if (borders[0, 1])
        //    borders[0, 0] = borders[0, 2] = false;

        //if (borders[2, 1])
        //    borders[2, 0] = borders[2, 2] = false;

        //if (borders[1, 2])
        //    borders[0, 2] = borders[2, 2] = false;


    }

    static float Evaluate(float value) {
		float a = 3;
		float b = 2.2f;

		return Mathf.Pow (value, a) / (Mathf.Pow (value, a) + Mathf.Pow (b - b * value, a));
	}

    static bool CompareBorders(bool[,] borderOne, bool[,] borderTwo)
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (borderOne[x, y] != borderTwo[x, y])
                    return false;
            }
        }

        return true;
    }

}