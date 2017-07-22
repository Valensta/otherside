using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MultiPathfinding : MonoBehaviour 
{
    public List<WaypointNodelet> Path = new List<WaypointNodelet>();
    public PathfinderType PathType = PathfinderType.GridBased;
	

    public void FindPath(int path, Vector2 startPosition, Vector2 endPosition, WaypointNodelet previousPoint)
    {
        if (PathType == PathfinderType.GridBased)
		{//Debug.Log("gridbased findpath");
          //  Pathfinder2D.Instance.InsertInQueue(startPosition, endPosition, SetList);
        }
        else if (PathType == PathfinderType.WaypointBased)
        {
            WaypointMultiPathfinder.Instance.InsertInQueue(path, startPosition, endPosition, SetList);          
        }
    }


    //A test move function, can easily be replaced
    public void Move()
    {
        if (Path.Count > 0)
		{//Debug.Log("IN HERE");
            transform.position = Vector2.MoveTowards(transform.position, Path[0].position, Time.deltaTime * 30F);
            
            if (Vector2.Distance(transform.position, Path[0].position) < 0.4F)
            {
                Path.RemoveAt(0);
            }
        }
    }

    protected virtual void SetList(List<WaypointNodelet> path)
    {
        if (path == null)
        {
            return;
        }


        Path.Clear();
        Path = path;
        if (Path.Count > 0)
        {
            //Path[0] = new Vector3(Path[0].x, Path[0].y - 1, Path[0].z);
            //Path[Path.Count - 1] = new Vector3(Path[Path.Count - 1].x, Path[Path.Count - 1].y - 1, Path[Path.Count - 1].z);
        }

    }
}
	
