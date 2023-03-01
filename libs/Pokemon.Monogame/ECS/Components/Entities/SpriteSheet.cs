using Microsoft.Xna.Framework;

namespace Pokemon.Monogame.ECS.Components.Entities;

public struct SpriteSheet
{
	public Vector2 GridSize { get; set; }
	
	public Vector2 TileSize { get; set; }
	
	public int TileIndexX { get; set; }
	
	public int TileIndexY { get; set; }

	public Rectangle[,] TilePositions { get; set; }
	
	public SpriteSheet(Vector2 gridSize, Vector2 tileSize)
	{
		GridSize = gridSize;
		TileSize = tileSize;
		
		TileIndexX = 0;
		TileIndexY = 0;
		
		var tilesCountX = (int) (gridSize.X / tileSize.X);
		var tilesCountY = (int) (gridSize.Y / tileSize.Y);

		TilePositions = new Rectangle[tilesCountX,tilesCountY];
		
		for (var x = 0; x < tilesCountX; x++)
		{
			for (var y = 0; y < tilesCountY; y++)
			{
				TilePositions[x,y] = new Rectangle((int) (x * tileSize.X), (int) (y * tileSize.Y), (int) tileSize.X, (int) tileSize.Y);
			}
		}
	}
}