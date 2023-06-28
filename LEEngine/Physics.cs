using System.Numerics;
using System;
using static System.MathF;

public class Collider {
	// values
	public Vector2 position = Vector2.Zero;
	protected Vector2 halfSize = Vector2.One;

	// getters
	public Vector2 HalfSize => halfSize;
	public Vector2 Size => halfSize * 2;

	public Collider () {}

	public Collider (Vector2 position, Vector2 size) {
		this.position = position;
		halfSize = size * 0.5f;
	}

	// aligns the edges of 2 colliders
	private static float AlignEdge (float pos1, float pos2, float cSize) {
		float dir = pos1 < pos2? 1 : -1;
		return pos2 - cSize * dir;
	}

	public void AlignEdgeX (Collider collider) {
		position.X = AlignEdge(position.X, collider.position.X, halfSize.X + collider.halfSize.X);
	}

	public void AlignEdgeY (Collider collider) {
		position.Y = AlignEdge(position.Y, collider.position.Y, halfSize.Y + collider.halfSize.Y);
	}

	public void SetSize (Vector2 size) {
		this.halfSize = size * 0.5f;
	}

	// converts it to a string
	public override string ToString(){
		return $"(pos: ({position.X}, {position.Y}), size: ({Size.X}, {Size.Y}))";
	}
}


public static class Physics {
	// calculates whether the colliders overlap
	public static bool CheckOverlap (Collider collider, Collider collider2) {
		Vector2 dist = Vector2.Abs(collider.position - collider2.position);
		Vector2 cSize = collider.HalfSize + collider2.HalfSize;
		return dist.X < cSize.X && dist.Y < cSize.Y;
	}

	// exponentially aproaches a target value
	public static float MoveTo (float val, float target, float dt) {
		return target + (val - target) * Exp(-dt);
	}

	public static float MoveToIntegral (float val, float target, float dt) {
		return target * dt - (val - target) * Exp(-dt) + val - target;
	}

	// exponentially aproaches a target value, but with Vectors
	public static Vector2 MoveTo (Vector2 val, Vector2 target, float dt) {
		return target + (val - target) * Exp(-dt);
	}

	public static Vector2 MoveToIntegral (Vector2 val, Vector2 target, float dt, float dtMult = 1) {
		return (target * dt * dtMult - (val - target) * Exp(-dt * dtMult) + val - target) / dtMult;
	}
}
