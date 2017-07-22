using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Random = UnityEngine.Random;

[System.Serializable]
public class WaypointSubPath
{
	public WaypointNode[] Map = null;
	public WaypointNode start;
	public WaypointNode finish;
	public List<QueuePath> queue = new List<QueuePath>();
	public WaypointListNode[] openList;
	public WaypointListNode[] closedList;
    public GameObject end;
    public int pathOrder;

}

[ExecuteInEditMode]
public class WaypointMultiPathfinder : MonoBehaviour 
{
    public enum WaypointMethod
    {
        ExactPosition,
        ClosestWaypoint
    }

    //Singleton
	private static WaypointMultiPathfinder instance;
	public static WaypointMultiPathfinder Instance { get { return instance; } private set { } }

    //Variables
    public List<string> DisallowedTags;//not really used for waypoint?
    
    public List<WaypointSubPath> paths;
    public WaypointMethod SearchMethod;
    public bool sortPaths = false;
    //Can i walk to that spot
    private bool freeSpot = true;

    //Queue path finding to not bottleneck it + timers
    
    //FPS
    private float updateinterval = 1F;
    private int frames = 0;
    private float timeleft = 1F;
    private int FPS = 60;

    //Set singleton!
    void Awake()
    {
        instance = this;
    }

	void Start () 
    {
        if (sortPaths)
        {
            paths.Sort((a, b) => a.pathOrder.CompareTo(b.pathOrder));
        }

   
		foreach (WaypointSubPath s in paths){
			s.openList = new WaypointListNode[s.Map.Length];
			s.closedList = new WaypointListNode[s.Map.Length];
			for (int i = 0; i < s.Map.Length; i++)
			{
				s.Map[i].ID = i; // why do we need this
                s.Map[i].order = i;
            }
		}
	}

    void Update() 
    {
    
        timeleft -= Time.deltaTime;
        frames++;

        if (timeleft <= 0F)
        {
            FPS = frames;
            timeleft = updateinterval;
            frames = 0;
        }

        float timer = 0F;
        float maxtime = 1000 / FPS;
        //Bottleneck prevention
		for (int i = 0; i < paths.Count;i++){
			while (paths[i].queue.Count > 0 && timer < maxtime)
			{
				Stopwatch sw = new Stopwatch();
				sw.Start();
				paths[i].queue[0].storeRef.Invoke(FindPath(i, paths[i].queue[0].startPos, paths[i].queue[0].endPos,-1));
				paths[i].queue.RemoveAt(0);
				sw.Stop();
				//print("Timer: " + sw.ElapsedMilliseconds);
				timer += sw.ElapsedMilliseconds;
			}
		}
	}


    //---------------------------------------SETUP PATH QUEUE---------------------------------------//

    public void InsertInQueue(int p, Vector3 startPos, Vector3 endPos, Action<List<WaypointNodelet>> listMethod)
    {
        QueuePath q = new QueuePath(startPos, endPos, listMethod);
        paths[p].queue.Add(q);
    }


    #region astar
    //---------------------------------------FIND PATH: A*------------------------------------------//


    private WaypointListNode startNode;
    private WaypointListNode endNode;
    private WaypointListNode currentNode;
    //Use it with KEY: F-value, VALUE: ID. ID's might be looked up in open and closed list then
    private List<NodeSearch> sortedOpenList = new List<NodeSearch>();

    public List<WaypointNodelet> FindPath(int p, Vector3 startPos, Vector3 endPos, int previousPath)
	{//print ("findpath");
        //The list we returns when path is found
        List<WaypointNodelet> returnPath = new List<WaypointNodelet>();

        //Find start and end nodes, if we cant return null and stop!
        SetStartAndEndNode(p, startPos, endPos, previousPath);
       // CheckEndPosition(endPos);

        if (startNode != null && endNode != null)
		{
			Array.Clear(paths[p].openList, 0, paths[p].openList.Length);
			Array.Clear(paths[p].closedList, 0, paths[p].openList.Length);
            if (sortedOpenList.Count > 0) { sortedOpenList.Clear(); }            
			paths[p].openList[startNode.ID] = startNode;            
            BHInsertNode(p, new NodeSearch(startNode.ID, startNode.F));

            bool endLoop = false;

            while (!endLoop)
            {
                //If we have no nodes on the open list AND we are not at the end, then we got stucked! return empty list then.
                if (sortedOpenList.Count == 0)
                {
            //      print("Empty Openlist, closedList");
                    return new List<WaypointNodelet>();
                }

                //Get lowest node and insert it into the closed list
                
                int id = BHGetLowest(p);
                
                //sortedOpenList.Sort(sort);
                //int id = sortedOpenList[0].ID;
                currentNode = paths[p].openList[id];
				paths[p].closedList[currentNode.ID] = currentNode;
				paths[p].openList[id] = null;
                //sortedOpenList.RemoveAt(0);

                if (currentNode.ID == endNode.ID)
                {
                    endLoop = true;
                    continue;
                }
                //Now look at neighbours, check for unwalkable tiles, bounderies, open and closed listed nodes.

                NeighbourCheck(p);
            }


            while (true)
            {
                returnPath.Add(new WaypointNodelet(currentNode));
                if (currentNode.parent != null)
                {
                    currentNode = currentNode.parent;
                }
                else
                {
                    break;
                }
            }

            returnPath.Reverse();

            //Now make sure we do not go backwards or go to long
            if (freeSpot && SearchMethod == WaypointMethod.ExactPosition)
            {
                returnPath.Add(new WaypointNodelet(endNode));

                if (returnPath.Count > 2)
                {
                    if (Vector3.Distance(returnPath[returnPath.Count - 1].position, returnPath[returnPath.Count - 3].position) < 
                        Vector3.Distance(returnPath[returnPath.Count - 3].position, returnPath[returnPath.Count - 2].position) && freeSpot)
                    {
                        returnPath.RemoveAt(returnPath.Count - 2);
                    }
                }
            }

            //Check if start pos i closer to second waypoint
            if (returnPath.Count > 1)
            {
                if (Vector3.Distance(returnPath[1].position, startPos) < Vector3.Distance(returnPath[0].position, returnPath[1].position))
                {
                    returnPath.RemoveAt(0);
                }
            }

            return returnPath;

        }
        else
        {
            return null;
        }
    }

    private void CheckEndPosition(Vector3 end)
    {
        RaycastHit[] hit = Physics.SphereCastAll(end + Vector3.up, 0.5F, Vector3.down, 1.5F);

        foreach (RaycastHit h in hit)
        {
            foreach (String s in DisallowedTags)
            {
                if (h.transform.tag == s)
                {
                    freeSpot = false;
                    return;
                }
            }
        }
        freeSpot = true;
    }

    // Find start and end Node
    private void SetStartAndEndNode(int p, Vector3 start, Vector3 end, int previous)
    {
        startNode = FindClosestNode(p, start, previous);
       // endNode = FindClosestNode(end);
	//	print ("setstartandendnode " + end);
		endNode = FindClosestNode(p, end, -1);
    }

    private WaypointListNode FindClosestNode(int p, Vector3 pos, int previous)
    {
        bool check_previous = !(previous < 0);
        Stopwatch a = new Stopwatch();
        a.Start();
        
        int ID = -1;
        float lowestDist = Mathf.Infinity;
        
        foreach (WaypointNode m in paths[p].Map)
        {
            if (check_previous && previous <= m.ID && Random.RandomRange(0, 1f) > 0.25f) continue;
            
            float d = Vector3.Distance(m.position, pos);
            if (d < lowestDist)
            {
                ID = m.ID;
                lowestDist = d;
            }

        }

        if (ID > -1)
        {
			//UnityEngine.Debug.Log(paths[p].ToString());
			//UnityEngine.Debug.Log(paths[p].Map.Length);
			WaypointListNode wp = new WaypointListNode(paths[p].Map[ID].position, paths[p].Map[ID].ID, paths[p].Map[ID].Iii, null, paths[p].Map[ID].neighbors);
            return wp;
			//print("Closest waypoint is " + ID);
        }
        else
        {
            return null;
        }
    }

    private float GetI(WaypointListNode n1, WaypointNode n2)
    {
        if (n1.ID + 1 == n2.ID && n1.I != 0)
        {
            UnityEngine.Debug.Log("WOoo special " + n1.ID + "\n");
             return n1.I;
            //return 1;
            
        }
        return 1;
    }

    private void NeighbourCheck(int p)
    {
        foreach (WaypointNode wp in currentNode.neighbors)
        {
            if (wp != null)
            {
                if (!OnClosedList(p, wp.ID))
                {
                    //If it is not on the open list then add it to
                    if (!OnOpenList(p, wp.ID))
                    {
                        
                        WaypointListNode addNode = new WaypointListNode(wp.position, wp.ID, wp.Iii, currentNode, wp.neighbors);
                        addNode.H = GetHeuristics(endNode.position, wp.position); // vertex to goal
                        
                        addNode.G = GetMovementCost(currentNode, wp) + currentNode.G; //start to vertex
                        addNode.F = addNode.H + addNode.G;
                        //Insert on open list
// UnityEngine.Debug.Log("CurrentNode " + currentNode.ID + " For " + addNode.ID + " H " + addNode.H + " G " + addNode.G + " F " + addNode.F + "\n");
                        paths[p].openList[addNode.ID] = addNode;
                        //Insert on sorted list
                        BHInsertNode(p, new NodeSearch(addNode.ID, addNode.F));
                        //sortedOpenList.Add(new NodeSearch(addNode.ID, addNode.F));
                        
                    }
                    else
                    {
                        ///If it is on openlist then check if the new paths movement cost is lower
                        WaypointListNode n = GetNodeFromOpenList(p, wp.ID);

						if (currentNode.G + GetMovementCost(currentNode, wp) < paths[p].openList[wp.ID].G)
                        {
                            n.parent = currentNode;
                            n.G = currentNode.G + GetMovementCost(currentNode, wp);
                            n.F = n.G + (n.H);
                            BHSortNode(p, n.ID, n.F);
                            //ChangeFValue(n.ID, n.F);
                        }
                    }
                }
            }
        }
     }

    private void ChangeFValue(int id, int F)
    {
        foreach (NodeSearch ns in sortedOpenList)
        {
            if (ns.ID == id)
            {
                ns.F = F;
            }
        }
    }

    //Check if a Node is already on the openList
    private bool OnOpenList(int p, int id)
    {
		return (paths[p].openList[id] != null) ? true : false;
    }

    //Check if a Node is already on the closedList
    private bool OnClosedList(int p, int id)
    {
		return (paths[p].closedList[id] != null) ? true : false;
    }

    private float GetHeuristics(Vector3 p1, Vector3 p2)
    {
        return Vector3.Distance(p1, p2);
    }

    private float GetMovementCost(Vector3 p1, Vector3 p2)
    {
        return Vector3.Distance(p1, p2);
    }

    private float GetMovementCost(WaypointListNode p1, WaypointNode p2)
    {
        float weight = 1;
        if (isNeighbor(p1, p2)) weight = p1.I;
        return weight * Vector3.Distance(p1.position, p2.position);
    }

    private bool isNeighbor(WaypointListNode p1, WaypointNode p2)
    {
        foreach (WaypointNode n in p1.neighbors)
        {
            if (n.ID == p2.ID) return true;
        }
        return false;
    }

    private WaypointListNode GetNodeFromOpenList(int p, int id)
    {
		return (paths[p].openList[id] != null) ? paths[p].openList[id] : null;
    }

    #region Binary_Heap (min)

    private void BHInsertNode(int p, NodeSearch ns)
    {
        //We use index 0 as the root!
        if (sortedOpenList.Count == 0)
        {
            sortedOpenList.Add(ns);
			paths[p].openList[ns.ID].sortedIndex = 0;
            return;
        }

        sortedOpenList.Add(ns);
        bool canMoveFurther = true;
        int index = sortedOpenList.Count - 1;
		paths[p].openList[ns.ID].sortedIndex = index;

        while (canMoveFurther)
        {
            int parent = Mathf.FloorToInt((index - 1) / 2);

            if (index == 0) //We are the root
            {
				paths[p].openList[sortedOpenList[index].ID].sortedIndex = 0;
                canMoveFurther = false;
            }
            else
            {
                if (sortedOpenList[index].Fv < sortedOpenList[parent].Fv)
                {
                    NodeSearch s = sortedOpenList[parent];
                    sortedOpenList[parent] = sortedOpenList[index];
                    sortedOpenList[index] = s;

                    //Save sortedlist index's for faster look up
					paths[p].openList[sortedOpenList[index].ID].sortedIndex = index;
					paths[p].openList[sortedOpenList[parent].ID].sortedIndex = parent;

                    //Reset index to parent ID
                    index = parent;
                }
                else
                {
                    canMoveFurther = false;
                }
            }
        }
    }

    private void BHSortNode(int p, int id, float F)
    {
        bool canMoveFurther = true;
        int index = paths[p].openList[id].sortedIndex;
        sortedOpenList[index].Fv = F;

        while (canMoveFurther)
        {
            int parent = Mathf.FloorToInt((index - 1) / 2);

            if (index == 0) //We are the root
            {
                canMoveFurther = false;
				paths[p].openList[sortedOpenList[index].ID].sortedIndex = 0;
            }
            else
            {
                if (sortedOpenList[index].Fv < sortedOpenList[parent].Fv)
                {
                    NodeSearch s = sortedOpenList[parent];
                    sortedOpenList[parent] = sortedOpenList[index];
                    sortedOpenList[index] = s;

                    //Save sortedlist index's for faster look up
					paths[p].openList[sortedOpenList[index].ID].sortedIndex = index;
					paths[p].openList[sortedOpenList[parent].ID].sortedIndex = parent;

                    //Reset index to parent ID
                    index = parent;
                }
                else
                {
                    canMoveFurther = false;
                }
            }
        }
    }

    private int BHGetLowest(int p)
    {

        if (sortedOpenList.Count == 1) //Remember 1 is our root
        {
            int ID = sortedOpenList[0].ID;
            sortedOpenList.RemoveAt(0);
            return ID;
        }
        else if (sortedOpenList.Count > 1)
        {
            //save lowest not, take our leaf as root, and remove it! Then switch through children to find right place.
            int ID = sortedOpenList[0].ID;
            sortedOpenList[0] = sortedOpenList[sortedOpenList.Count - 1];
            sortedOpenList.RemoveAt(sortedOpenList.Count - 1);
			paths[p].openList[sortedOpenList[0].ID].sortedIndex = 0;

            int index = 0;
            bool canMoveFurther = true;
            //Sort the list before returning the ID
            while (canMoveFurther)
            {
                int child1 = (index * 2) + 1;
                int child2 = (index * 2) + 2;
                int switchIndex = -1;

                if (child1 < sortedOpenList.Count)
                {
                    switchIndex = child1;
                }
                else
                {
                    break;
                }
                if (child2 < sortedOpenList.Count)
                {
                    if (sortedOpenList[child2].Fv < sortedOpenList[child1].Fv)
                    {
                        switchIndex = child2;
                    }
                }
                if (sortedOpenList[index].Fv > sortedOpenList[switchIndex].Fv)
                {
                    NodeSearch ns = sortedOpenList[index];
                    sortedOpenList[index] = sortedOpenList[switchIndex];
                    sortedOpenList[switchIndex] = ns;

                    //Save sortedlist index's for faster look up
					paths[p].openList[sortedOpenList[index].ID].sortedIndex = index;
					paths[p].openList[sortedOpenList[switchIndex].ID].sortedIndex = switchIndex;

                    //Switch around idnex
                    index = switchIndex;
                }
                else
                {
                    break;
                }
            }

            return ID;

        }
        else
        {
            return -1;
        }
    }
    #endregion //end BH sort
    #endregion //End astar region!
}