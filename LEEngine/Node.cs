using System.Numerics;
using System.Collections.Generic;


public class Node {
	EntityBody? body;
	public EntityBody Body => GetBody();
	public bool ShouldRemove => GetBody().ShouldRemove;

	Node[]? nodes = null;

	public Vector2 offset = Vector2.Zero;
	public Vector2 LocalPosition => offset + GetBody().position;

	public virtual void Setup () {}
	public virtual void Execute (float dt) {}
	protected virtual bool OnMessage (Message message) {return false;}

	static List<Node> allNodes = new List<Node>();
	static Dictionary<char, List<Node>> globalNodeLists = new Dictionary<char, List<Node>>();

	public bool Message (Message message) {
		bool result = false;

		if (nodes != null)
			for (int i = 0; i <= nodes.Length; i++) {
				result = nodes[i].Message(message) || result;
			}

		return OnMessage(message) || result;
	}

	public void Destroy () {
		GetBody().Destroy();
	}

	public void Register (Node[] nodes) {
		this.nodes = nodes;

		for (int i = 0; i < nodes.Length; i++) {
			nodes[i].body = GetBody();
			nodes[i].Setup();
		}
	}

	public static void DestroyAll (List<Node> nodes) {
		for (int i = 0; i < nodes.Count; i++) {
			nodes[i].Destroy();
		}
		nodes.Clear();
	}

	EntityBody GetBody () {
		if (body == null)
			body = new EntityBody();
		
		return body;
	}

	public static T SetupNode <T> (T node, Vector2 position) where T : Node {
		node.Setup();
		node.Body.position = position;
		var ni = new NodeIter(allNodes);
		while (ni.Next()) {}
		allNodes.Add(node);
		return node;
	}

	public static void DestroyAll () {
		var ni = new NodeIter(allNodes);
		while (ni.Next()) {
			ni.Get().Destroy();
		}

		globalNodeLists.Clear();
		allNodes.Clear();
	}

	public static List<Node> GetNodes(char k) {
		if (!globalNodeLists.ContainsKey(k)) {
			globalNodeLists.Add(k, new List<Node>());
		}

		return globalNodeLists[k];
	}
}

public class EntityBody : Collider {
	bool shouldRemove = false;
	public bool ShouldRemove => shouldRemove;

	public void Destroy() {
		shouldRemove = true;
	}
}

public struct Message {
	public readonly string key = "untitled";
	public readonly (float, float) fvs = (0, 0);
	public readonly int am = 0;

	public Vector2 Dir => new Vector2(fvs.Item1, fvs.Item2);

	public Message () {}
	public Message (string key, (float, float)? fvs = null, int am = 0) {
		this.key = key;
		this.am = am;

		if (fvs.HasValue)
			this.fvs = fvs.Value;
	}

	public Message (string key, Vector2? dir = null, int am = 0) {
		this.key = key;
		this.am = am;

		if (dir.HasValue)
			this.fvs = (dir.Value.X, dir.Value.Y);
	}

	public Message (string key, int am = 0) {
		this.key = key;
		this.am = am;
	}
}
