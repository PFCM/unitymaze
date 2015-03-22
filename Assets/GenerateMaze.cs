using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateMaze : MonoBehaviour {
	private enum Direction : byte {
		NONE  = 0,
		NORTH = 1,
		EAST  = 2,
		WEST  = 4,
		SOUTH = 8,
		ALL   = 1|2|4|8
	};

	// actual maze
	int[,] maze;// = new byte[10,10];

	private class MazePoint {
		public int x;
		public int y;

		public MazePoint(int x, int y) {
			this.x = x;
			this.y = y;
		}
	}

	private class StackFrame
	{
		public MazePoint to;
		public MazePoint from;
		public Direction d;

		public StackFrame(MazePoint f, MazePoint t, Direction d) {
			this.to = t;
			this.from = f;
			this.d = d;
		}
	}

	private class RSStackFrame
	{
		public MazePoint topLeft;
		public MazePoint bottomRight;

		public RSStackFrame(MazePoint a, MazePoint b) {
			this.topLeft = a;
			this.bottomRight = b;
		}
	}

	// maze walls
	public Transform eastInnerWall;
	public Transform southInnerWall;

	// method to generate a maze via random depth first search
	private int[,] GenerateDFS(int width, int height) {
		int[,] data = new int[width, height];
		Stack<StackFrame> fringe = new Stack<StackFrame> ();
		fringe.Push (new StackFrame (null, new MazePoint (0, 0), Direction.NONE));
		// and actually sarch around
		while (fringe.Count > 0) {
			StackFrame frame = fringe.Pop(); // grab one
			MazePoint current = frame.to;
			int b = data[current.x, current.y];
			if (b != 0) continue; // if we have visited it then don't visit again
			List<StackFrame> children = childrenOf(current, Direction.NONE, data); // all the places we could go
			if (frame.d != Direction.NONE) {
				data[frame.from.x, frame.from.y] |= (int)frame.d;
				data[current.x, current.y] |= (int)flip (frame.d);
			}
			// shuffle children list
			for (int i = 0; i < children.Count; i++) {
				StackFrame temp = children[i];
				int randomIndex = Random.Range(i, children.Count);
				children[i] = children[randomIndex];
				children[randomIndex] = temp;
			}
			
			foreach(StackFrame c in children) { // put them on the stack
				fringe.Push(c);
			}
		}
		return data;
	}

	private int[,] GenerateRecursiveSubdivision(int width, int height, bool randomSize = true) {
		int[,] data = new int[width, height]; // we actuall want to set them all to have no walls

		for (int x = 0; x < width; x++) 
			for (int y = 0; y < height; y++) {
				data[x,y] = (int)Direction.ALL; // all directions are permissible = no walls
			}

		Stack<RSStackFrame> stack = new Stack<RSStackFrame> ();
		stack.Push (new RSStackFrame (new MazePoint (0, 0), new MazePoint (width, height)));
		//int count = 0;
		while (stack.Count > 0) { // stack frame here is a rectangle
			RSStackFrame current = stack.Pop();

			// make sure it is big enough
			// calculate some helpers
			int xrange = current.bottomRight.x - current.topLeft.x;
			int yrange = current.bottomRight.y - current.topLeft.y;

			if (yrange < 2 || xrange < 2) // bail if it is too small
				continue;

			// first we will make a wall down the middle
			// either vertical or horizontal, we choose at random
			int vertical = Random.Range(0,2);
			if (xrange > yrange){//vertical == 1) {
				// choose an index
				int split = (randomSize)? Random.Range(current.topLeft.x,current.bottomRight.x-2) :
					                      (xrange+1)/2 + current.topLeft.x;
				// choose a random door
				int door = Random.Range(0,yrange)+current.topLeft.y;
				// now put in the walls
				for (int y = current.topLeft.y; y < current.bottomRight.y; y++) {
					if (y != door) {
						data[split,y]   &= ~(int)Direction.EAST;
						data[split+1,y] &= ~(int)Direction.WEST;
					}
				}
				// now split the current rect and push each half onto the stack
				stack.Push(new RSStackFrame(current.topLeft, 
				                            new MazePoint(split+1, current.bottomRight.y)));
				stack.Push(new RSStackFrame(new MazePoint(split+1, current.topLeft.y), 
				                            current.bottomRight));
			} else { // horizontal
				// choose an index
				int split = (randomSize)? Random.Range(current.topLeft.y,current.bottomRight.y-2) :
					                      (yrange+1)/2 + current.topLeft.y;
				// choose a random door
				int door = Random.Range(0,xrange)+current.topLeft.x;
				// now put in the walls
				for (int x = current.topLeft.x; x < current.bottomRight.x; x++) {
					if (x != door) {
						data[x,split]   &= ~(int)Direction.SOUTH;
						data[x,split+1] &= ~(int)Direction.NORTH;
					}
				}
				// now split the current rect and push each half onto the stack
				stack.Push(new RSStackFrame(current.topLeft, 
				                            new MazePoint(current.bottomRight.x, split+1)));
				stack.Push(new RSStackFrame(new MazePoint(current.topLeft.x, split+1), 
				                            current.bottomRight));
			}
		}
		return data;
	}

	// Use this for initialization
	void Start () {
		//maze = this.GenerateDFS (10, 10);
		int which = Random.Range (0, 3);
		if (which == 0)
			maze = this.GenerateRecursiveSubdivision (10, 10, true);
		else if (which == 1)
			maze = this.GenerateRecursiveSubdivision (10, 10, true);
		else 
			maze = this.GenerateDFS (10, 10);


		// now add actual gameobjects
		// loop through each chap and add the west and south walls if necessary
		for (int x = 0; x < 10; x++) {
			for (int y = 0; y < 10; y++) {

				/*GameObject lightGameobject = new GameObject("Light (" + x +","+y+")");
				Light pointLight = lightGameobject.AddComponent<Light>();
				pointLight.color = Color.cyan;

				lightGameobject.transform.localPosition = new Vector3((x-4.5f)*5,1,(y-4.5f)*5);
				pointLight.type = LightType.Point;
				pointLight.intensity = 0.5f;*/

				if ((maze[x,y] & (byte)Direction.SOUTH) == 0 &&
				    y < 9){
					Instantiate (southInnerWall,this.transform.position +  new Vector3 ((x-4.5f)*5, 0, (y-4.0f)*5), Quaternion.identity);
				} 

				if ((maze[x,y] & (byte)Direction.EAST) == 0 &&
				    x < 9) {
					Instantiate (eastInnerWall, this.transform.position + new Vector3 ((x-4f)*5.0f, 0, (y-4.5f)*5), Quaternion.identity);
				}
			}
		}
	}

	// gets children (adjacent indices) excluding from
	private List<StackFrame> childrenOf(MazePoint mp, Direction from, int[,] data) {
		List<StackFrame> c = new List<StackFrame> ();
		for (int i = 0; i < 4; i++) { // for each direction
			MazePoint child = nextPoint(mp, (Direction)(1 << i));
			if ((Direction)(1 << i) != from && 
			    child.x >= 0 && child.x < 10 && 
			    child.y >= 0 && child.y < 10 && 
			    data[child.x, child.y] == (int)Direction.NONE)
				c.Add(new StackFrame(mp, child, (Direction)(1<<i)));
		}
		return c;
	}

	// reverses a direction
	private Direction flip(Direction d) {
		switch (d) {
		case Direction.WEST:
			return Direction.EAST;
		case Direction.NORTH:
			return Direction.SOUTH;
		case Direction.SOUTH:
			return Direction.NORTH;
		case Direction.EAST:
			return Direction.WEST;
		case Direction.NONE:
			return Direction.NONE;
		default:
			return Direction.NONE;
		}
	}

	// chooses a random direction from the available directions
	private Direction randomDirection(MazePoint from) {
		int r = Random.Range (0, 4);
		byte d = (byte)(1 << r);

		MazePoint next = nextPoint (from, (Direction)d);

		int count = 0;
		// make sure it is valid
		while ((next.x < 0 || next.x >= 10 || next.y < 0 || next.y >= 10 ||
	           maze[next.x, next.y] != (byte)Direction.NONE) && count < 4) {
			d = (byte)cycleDirection ((Direction)d);
			next = nextPoint (from, (Direction)d);
			count++;
		}

		if (count == 4)
			return Direction.NONE;

		return (Direction)d;
	}

	// cycles the direction around the compass points clockwise one step
	private Direction cycleDirection( Direction d) {
		switch (d) {
		case Direction.WEST:
			return Direction.NORTH;
		case Direction.NORTH:
			return Direction.EAST;
		case Direction.SOUTH:
			return Direction.WEST;
		case Direction.EAST:
			return Direction.SOUTH;
		case Direction.NONE:
			return Direction.NONE;
		default:
			return Direction.NONE;
		}
	}

	private MazePoint nextPoint (MazePoint from, Direction d) {
		switch (d) {
		case Direction.EAST:
			return new MazePoint(from.x + 1, from.y);
		
		case Direction.NORTH:
			return new MazePoint(from.x, from.y-1);

		case Direction.SOUTH:
			return new MazePoint(from.x, from.y +1);

		case Direction.WEST:
			return new MazePoint(from.x-1, from.y);

		default:
			Debug.Log("Invalid direction: " + d);
			return null;
		}
	}
}
