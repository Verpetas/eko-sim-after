using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid {

	Node[,] grid;
	int nodeCountX, nodeCountY;
	float terrainSizeX, terrainSizeY;

	public Grid(Node[,] grid, (int, int) gridSize, (float, float) terrainSize)
	{
		this.grid = grid;
		this.nodeCountX = gridSize.Item1;
        this.nodeCountY = gridSize.Item2;
        this.terrainSizeX = terrainSize.Item1;
        this.terrainSizeY = terrainSize.Item2;
    }

	public int MaxSize
	{
		get
		{
			return nodeCountX * nodeCountY;
		}
	}

	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < nodeCountX && checkY >= 0 && checkY < nodeCountY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}
	

	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		float gridOffset = 2f;

		float percentX = (worldPosition.x - gridOffset) / (terrainSizeX - (gridOffset * 2));
		float percentY = (worldPosition.z - gridOffset) / (terrainSizeY - (gridOffset * 2));
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);
		int x = Mathf.RoundToInt((nodeCountX - 1) * percentX);
		int y = Mathf.RoundToInt((nodeCountY - 1) * percentY);

		return grid[x, y];
    }
	
}