﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator {

	public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre, Vector2 chunkBorderPos) {
		float[,] noiseValues = Noise.GenerateNoiseMap (width, height, settings.noiseSettings, sampleCentre);
		float[,] falloffValues = FalloffGenerator.GenerateFalloffMap(width, chunkBorderPos);

        AnimationCurve heightCurve = new AnimationCurve (settings.heightCurve.keys);

		float minValue = float.MaxValue;
		float maxValue = float.MinValue;

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				noiseValues[i, j] = Mathf.Clamp(noiseValues[i, j] - falloffValues[i, j], 0, float.MaxValue); 
                noiseValues[i, j] *= heightCurve.Evaluate (noiseValues[i, j]) * settings.heightMultiplier;

				if (noiseValues[i, j] > maxValue) {
					maxValue = noiseValues[i, j];
				}
				if (noiseValues[i, j] < minValue) {
					minValue = noiseValues[i, j];
				}
			}
		}

		return new HeightMap (noiseValues, minValue, maxValue);
	}

}

public struct HeightMap {
	public readonly float[,] values;
	public readonly float minValue;
	public readonly float maxValue;

	public HeightMap (float[,] values, float minValue, float maxValue)
	{
		this.values = values;
		this.minValue = minValue;
		this.maxValue = maxValue;
	}
}

