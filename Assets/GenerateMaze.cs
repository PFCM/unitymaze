using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateMaze : MonoBehaviour {
	private enum Direction : byte {
		NONE  = 0,
		NORTH = 1,
		EAST  = 2,
		WEST  = 4,
		SOUTH = 8
	};

	// actual maze
	byte[,] maze = new byte[10,10];

	private class MazePoint {
		public int x;
		public int y;

		public MazePoint(int x, int y) {
			this.x = x;
			this.y = y;
		}
	}

	// Use this for initialization
	void Start () {
		Queue<MazePoint> fringe = new Queue<MazePoint>();
		fringe.Enqueue (new MazePoint (0, 0));

		while (fringe.Count > 0) {
			// grab a square from the queue
			MazePoint current = fringe.Dequeue();
			byte b = maze[current.x,current.y];
			//if ((b & (b-1)) != 0) // wizardry -- check if power of two or 0 (only one or 0 directions set)
			//	continue;
			Direction d = randomDirection(current);
			if (d == Direction.NONE) // if there's nowhere to go, bail
				continue;
			MazePoint next = nextPoint(current, d);
			// set this transition into the maze
			maze[current.x, current.y] &= (byte)d;
			maze[next.x,next.y] &= (byte)flip(d);
			// add the possible next places to go
			List<MazePoint> children = childrenOf(next, d);
		}
	}

	// gets children (adjacent indices) excluding from
	private List<MazePoint> childrenOf(MazePoint mp, Direction from) {
		List<MazePoint> c = new List<MazePoint> ();
		for (int i = 0; i < 4; i++) { // for each direction
			MazePoint child = nextPoint(mp, (Direction)(1 << i));
			if ((Direction)(1 << i) != from && child.x >= 0 && child.x < 10 && child.y >= 0 && child.y < 10)
				c.Add(child);
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
		int r = Random.Range (1, 5);
		Direction d = (Direction)(1 << r);

		MazePoint next = nextPoint (from, (Direction)d);

		int count = 0;
		// make sure it is valid
		while ((next.x < 0 || next.x >= 10 || next.y < 0 || next.y >= 10 ||
	           maze[next.x, next.y] != (byte)Direction.NONE) && count < 4) {
			d = cycleDirection (d);
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
