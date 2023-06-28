using System.Numerics;
using System.Collections.Generic;

public static class Engine {
	// callback things
	public static Editor editor = new Editor();
	public static GameWorld? world;

	// Sets the game mode
	public static GameMode gameMode = GameMode.Editor;

	// Level loading
	public static bool queueLevelChange = true;
	public static string levelFileName = "Level";
	public static int currentIndex = 0;
	public static List<(int x, int y, char t)> levelBP = new List<(int, int, char)>();

	public static void Update (float dt) {
		if (gameMode == GameMode.Editor){ // updates the level editor
			editor.Update(dt);
			queueLevelChange = true;
		}
		else if (gameMode == GameMode.Game){
			if (world == null)
				gameMode = GameMode.Editor;
			else{
				if (queueLevelChange){ // loads a new level
					world.ChangeLevel(levelBP);
					queueLevelChange = false;
				}
				world.Update(dt); // updates the game world
			}
		}
	}

	public static void Draw () {
		if (gameMode == GameMode.Editor)
			editor.Draw();// draws the editor
		else if (gameMode == GameMode.Game){
			if (world != null)
				world.Draw(); // draws the world
		}
	}

	public static string LevelToString (List<(int x, int y, char t)> levelBP) {
		string res = "";

		for (int i = 0; i < levelBP.Count; i++) {
			var ent = levelBP[i]; // gets the entity
			res += $"{ent.t}|{ent.x}|{ent.y}\n"; // adds it to the string
		}

		return res;
	}

	public static void StringToLevel (List<(int, int, char)> level, string levelBP) {
		level.Clear(); // clears the level
		string[] levelEnts = levelBP.Split('\n'); // splits the entities

		for (int i = 0; i < levelEnts.Length; i++) {
			if (levelEnts[i].Length <= 4) break; // stops if there isn't an entity on that line
			
			char c = levelEnts[i][0]; // gets the entity type
			int ind = 2, x, y; // defines the index (at 2 after the entity and the |) and the x and y position

			(x, ind) = TextHelper.GetNum(levelEnts[i], ind); //gets the x value
			(y, ind) = TextHelper.GetNum(levelEnts[i], ind+1); // gets the y value
			level.Add((x, y, c)); // adds it to the list
		}
	}

	public static void ReadLevel (int index) {
		currentIndex = index;
		var path = FilePath; // gets the file path
		if (System.IO.File.Exists(path)){ // makes sure the file exists
			var str = System.IO.File.ReadAllText(path); // reads the level from file
			StringToLevel(levelBP, str); // loads the level
		}
		else
			levelBP.Clear();
	}

	public static void SaveLevel () {
		var path = FilePath;
		System.IO.File.WriteAllText(path, LevelToString(Engine.levelBP));
	}

	static string FilePath => TextHelper.GetRootPath + levelFileName + currentIndex + ".txt";
}

public class Editor {
	
	public float placeCoolDown = 0.5f;
	public Vector2 cursorPosition;

	public Vector2 GridPosition => new Vector2(
		MathF.Round((cursorPosition.X) / grid) * grid,
		MathF.Round((cursorPosition.Y) / grid) * grid
		);


	public bool CanPlace => placeCoolDown <= 0;

	public int grid = 32;

	public void Update (float dt) {
		if (placeCoolDown < 0)
			placeCoolDown -= dt;

		OnUpdate();

		
	}

	public void PlaceEntityPoint ((int, int, char) e) {
		Engine.levelBP.Add(e);
	}


	public void Draw () {
		for (int i = 0; i < Engine.levelBP.Count; i++) {
			DrawEntities(Engine.levelBP[i]);
		}

		DrawCursor(cursorPosition);
	}

	public void RemoveEntity (Vector2 pos) {
		for (int i = Engine.levelBP.Count - 1; i >= 0; i--) {
			var ent = Engine.levelBP[i];
			var dist = pos - new Vector2(ent.x, ent.y);
			dist *= dist;
			if (dist.X + dist.Y < grid / 4) {
				Engine.levelBP.RemoveAt(i);
			}
		}
	}

	public void PlaceEntity (Vector2 pos, char c) {
		RemoveEntity(pos);
		Engine.levelBP.Add(((int)pos.X,(int)pos.Y, c));
	}

	protected virtual void OnUpdate() {}
	protected virtual void DrawEntities ((int x, int y, char c) e) {}
	protected virtual void DrawCursor (Vector2 pos) {}
}

public class GameWorld {
	
	public virtual object MakeEntity (Vector2 pos, char t) {return null;}

	public virtual void Clear () {}

	public virtual void Update (float dt) {}
	
	public virtual void Draw () {}

	public void ChangeLevel (List<(int x, int y, char t)> levelBP) {
		Clear();
		for (int i = 0; i < levelBP.Count; i++) {
			var ent = levelBP[i];
			MakeEntity(new Vector2(ent.x, ent.y), ent.t);
		}
	}
	

	
}

public enum GameMode {
	Editor, Game
}

