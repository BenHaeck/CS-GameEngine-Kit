using System.Numerics;


public class CollisionPartition {
	List<Node>[,] sectors;
	public readonly float sectorSize = 0;

	public CollisionPartition ((int, int) sectorNums, float sectorSize) {
		sectors = new List<Node>[sectorNums.Item2, sectorNums.Item1];
		this.sectorSize = sectorSize;
	}

	public void Reset () {
		sectors = new List<Node>[sectors.GetLength(0), sectors.GetLength(1)];
	}

	public void AddToSector (Node node) {
		var sect = GetSectorIndex(node.Body.position);
		
		if (sect.y >= sectors.GetLength(0) || sect.x >= sectors.GetLength(1))
			return;

		if (sectors[sect.y, sect.x] == null)
			sectors[sect.y, sect.x] = new List<Node>();
		
		sectors[sect.y, sect.x].Add(node);
	}

	public (int x, int y) GetSectorIndex (Vector2 pos) {
		Vector2 gridPos = pos / sectorSize;
		return ((int)gridPos.X, (int)gridPos.Y);
	}

	public bool IsValidIndex ((int c, int r) indexes) {
		return indexes.c < 0 || indexes.r < 0 ||
		indexes.r >= sectors.GetLength(0) || indexes.c >= sectors.GetLength(1);
	}

	public List<Node>? GetSector ((int c, int r) indexes) {
		var sector = sectors[indexes.r, indexes.c];
		if (sector == null){
			sector = new List<Node>();
			sectors[indexes.r, indexes.c] = sector;
		}
		return sector;
	}

	public PartIter GetIter (Node node) {
		return new PartIter (node, this);
	}

	public struct PartIter : INodeIterator {
		CollisionPartition partition;
		CollideIter iter;
		Node mainNode;
		int startX, startY, cX = 0, cY = 0;

		int EndX => Math.Min(partition.sectors.GetLength(1) - 1, startX + 2);
		int EndY => Math.Min(partition.sectors.GetLength(0) - 1, startY + 2); 

		public PartIter (Node mainNode, CollisionPartition partition) {
			this.mainNode = mainNode;
			this.partition = partition;
			var pos = partition.GetSectorIndex(mainNode.Body.position);
			startX = pos.x - 1;
			startY = pos.y - 1;
			cX = Math.Max(startX, 0);
			cY = Math.Max(startY, 0);
			iter = new CollideIter(partition.GetSector((cX, cY)), mainNode);
			Reset();
		}

		public void Reset () {
			cX = Math.Max(0, startX);
			cY = Math.Max(0, startY);
			iter = new CollideIter(partition.GetSector((cX, cY)), mainNode);
		}

		public bool Next() {
			while (!iter.Next() && cY <= EndY) {
				cX += 1;
				if (cX > EndX) {
					cX = Math.Max(0, startX);
					cY++;
				}
				iter = new CollideIter(partition.GetSector((cX, cY)), mainNode);
			}

			return cY <= EndY;
		}

		public Node Get(){
			return iter.Get();
		}
	}
}
