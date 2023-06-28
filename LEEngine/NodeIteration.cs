using System.Numerics;
using System.Collections.Generic;

public class NodeEvent {
	List<Node> listeners = new List<Node>();
	
	public void Add (Node node) {
		listeners.Add(node);
	}

	public void Execute (float dt) {
		NodeIter iter = new NodeIter(listeners);
		while (iter.Next()) {
			iter.Get().Execute(dt);
		}
	}

	public int GetSize (){
		return listeners.Count;
	}
}


public interface INodeIterator {
	void Reset(); // reset the iterator
	bool Next(); // moves to the next node (and returns if there is a next)
	Node Get(); // gets the current node
}

public struct NodeIter: INodeIterator {
	public List<Node> nodes;
	int i;

	// constructor
	public NodeIter (List<Node> nodes) {
		this.nodes = nodes;
		i = nodes.Count;
	}

	// resets the iterator
	public void Reset() {
		i = nodes.Count;
	}

	// moves to next node
	public bool Next(){
		if (nodes == null) return false;
		i--;
		while (i >= 0) {
			if (nodes[i].ShouldRemove) {
				nodes.RemoveAt(i); // if a node is marked for removal, remove the node
				i--;
			} else
				break;// if not that is the next node
		}
		return i >= 0;
	}

	public Node Get (){
		return nodes[Math.Clamp(i, 0, nodes.Count - 1)]; // returns the next node
	}
}

public struct CollideIter: INodeIterator {
	List<Node> nodes; // the nodes to check
	Node mainNode; // the node that is checked against the other nodes
	int i;
	public CollideIter (List<Node> nodes, Node mainNode) {
		this.nodes = nodes;
		this.mainNode = mainNode;
		i = nodes.Count;
	}
	public void Reset() {
		i = nodes.Count;
	}

	public bool Next(){
		if (nodes == null) return false;
		i--;
		while (i >= 0) {
			if (nodes[i].ShouldRemove) {// if marked for removal, remove it
				nodes.RemoveAt(i);
			} else if (Physics.CheckOverlap(nodes[i].Body, mainNode.Body)){ // if not collided, skip
				break;
			}

			i--;
		}

		return i >= 0; // returns if there is a next node
	}

	public Node Get() {
		return nodes[Math.Clamp(i, 0, nodes.Count - 1)]; // returns the next node
	}
}
