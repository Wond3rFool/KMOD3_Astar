using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    /// 
    List<Node> openList = new List<Node>();
    List<Vector2Int> closedList = new List<Vector2Int>();
    List<Node> nodesInGrid = new List<Node>();
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        openList.Clear();
        closedList.Clear();
        nodesInGrid.Clear();
        Cell currentCell = grid[startPos.y, startPos.x];
        Node startNode = new Node();
        Node endNode = new Node();

        startNode.position = startPos;
        endNode.position = endPos;
        openList.Add(startNode);

        foreach (Cell cell in grid)
        {
            Node node = new Node(cell.gridPosition, null, int.MaxValue, 0);
            if (!nodesInGrid.Contains(node)) nodesInGrid.Add(node);
        }

        startNode.GScore = 0;
        startNode.HScore = startNode.SetDistance(startNode, endNode);


        while (openList.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openList);

            if (currentNode.position == endNode.position)
            {
                Debug.Log("hey");
                return CalculatePath(currentNode);
            }
            currentCell = grid[currentNode.position.x, currentNode.position.y];

            openList.Remove(currentNode);
            closedList.Add(currentNode.position);

            foreach (Node neighbourNode in GetNeighbourList(currentNode, grid, currentCell))
            {
                if (closedList.Contains(neighbourNode.position)) continue;

                int tentativeGCost = currentNode.GScore + currentNode.SetDistance(currentNode, neighbourNode);

                if (tentativeGCost < neighbourNode.GScore)
                {
                    neighbourNode.parent = currentNode;
                    neighbourNode.GScore = tentativeGCost;
                    neighbourNode.HScore = neighbourNode.SetDistance(neighbourNode, endNode);

                    Node nodeIndex = nodesInGrid.Find(x => x.position == neighbourNode.position);
                    int index = nodesInGrid.IndexOf(nodeIndex); 
                    if (index != -1)
                    {
                        nodesInGrid[index] = neighbourNode;
                    }

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }

        }
        return null;
    }

    private List<Vector2Int> CalculatePath(Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(endNode.position);
        Node currentNode = endNode;
        while (currentNode.parent != null)
        {
            path.Add(currentNode.parent.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private Node GetLowestFCostNode(List<Node> nodeList)
    {
        Node lowestFCostNode = nodeList[0];
        for (int i = 0; i < nodeList.Count; i++)
        {
            if (nodeList[i].FScore < lowestFCostNode.FScore)
            {
                lowestFCostNode = nodeList[i];
            }
        }
        return lowestFCostNode;
    }

    private List<Node> GetNeighbourList(Node currentNode, Cell[,] grid, Cell currentCell)
    {
        List<Node> neighbourList = new List<Node>();

        if (!currentCell.HasWall(Wall.LEFT))
        {
            if (currentNode.position.x - 1 >= 0)
            {
                neighbourList.Add(GetNode(nodesInGrid, currentNode.position.x - 1, currentNode.position.y));
            }
        }
        if (!currentCell.HasWall(Wall.RIGHT))
        {
            if (currentNode.position.x + 1 < grid.GetLength(0))
            {
                neighbourList.Add(GetNode(nodesInGrid, currentNode.position.x + 1, currentNode.position.y));
            }
        }
        if (!currentCell.HasWall(Wall.DOWN))
        {
            if (currentNode.position.y - 1 >= 0)
            {
                neighbourList.Add(GetNode(nodesInGrid, currentNode.position.x, currentNode.position.y - 1));
            }
        }
        if (!currentCell.HasWall(Wall.UP))
        {
            if (currentNode.position.y + 1 < grid.GetLength(1))
            {
                neighbourList.Add(GetNode(nodesInGrid, currentNode.position.x, currentNode.position.y + 1));
            }
        }
        return neighbourList;
    }


    private Node GetNode(List<Node> nodes, int nodePositionX, int nodePositionY)
    {
        Node node = nodes.Find(x => x.position.x == nodePositionX && x.position.y == nodePositionY);
        return node;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public int FScore
        { //GScore + HScore
            get { return GScore + HScore; }
        }
        public int GScore; //Current Travelled Distance
        public int HScore; //Distance estimated based on Heuristic

        public Node() { }

        public Node(Vector2Int position)
        {
            this.position = position;

        }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
        public int SetDistance(Node targetX, Node targetY)
        {
            int xDistance = Mathf.Abs(targetX.position.x - targetY.position.x);
            int yDistance = Mathf.Abs(targetX.position.y - targetY.position.y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * Math.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }
    }
}
