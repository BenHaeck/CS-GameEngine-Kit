using System.Numerics;

public class CollisionPartition {
	List<Node>[,] sectors;
	public readonly float sectorSize = 0;

	public CollisionPartition ((int, int) sectorNums, float sectorSize) {
		sectors = new List<Node>[sectorNums.Item2, sectorNums.Item1];
		this.sectorSize = sectorSize;
	}

	public void AddToSector (Node node) {
		var sect = GetSector(node.Body.position);
		
		if (sect.y >= sectors.GetLength(0) || sect.x >= sectors.GetLength(1))
			return;

		if (sectors[sect.y, sect.x] == null)
			sectors[sect.y, sect.x] = new List<Node>();
		
		sectors[sect.y, sect.x].Add(node);
	}

	(int x, int y) GetSector (Vector2 pos) {
		Vector2 gridPos = pos / sectorSize;
		return ((int)gridPos.X, (int)gridPos.Y);
	}
}
