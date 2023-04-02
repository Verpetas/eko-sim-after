using UnityEngine;
using System.Collections;
using System.Drawing;

public static class FalloffGenerator {

    public static float[,] GenerateFalloffMap(int size, Vector2 chunkBorderPos)
    {

        float[,] map = new float[size, size];
        int borderPosIndex = (int)(Mathf.Abs(chunkBorderPos.x) + Mathf.Abs(chunkBorderPos.y));

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = -chunkBorderPos.x * (i / (float)size * 2 - 1);
                float y = -chunkBorderPos.y * (j / (float)size * 2 - 1);

                //float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

                float value = 0;
                if (borderPosIndex == 2)
                {
                    value = Mathf.Clamp01(Mathf.Max(x, y));
                }
                else if (borderPosIndex == 1)
                {
                    value = x + y;
                }
                map[i, j] = Evaluate(value);

            }
        }

        return map;

    }

    static float Evaluate(float value) {
		float a = 3;
		float b = 2.2f;

		return Mathf.Pow (value, a) / (Mathf.Pow (value, a) + Mathf.Pow (b - b * value, a));
	}

}
