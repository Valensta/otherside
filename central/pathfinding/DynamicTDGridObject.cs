using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class DynamicTDGridObject : MonoBehaviour 
{
    private List<Vector2> IDs = new List<Vector2>();

    
    public float timer = 0F;
    public bool SetTimer = false;

    private Vector2 lastPos = Vector2.zero;
    private Quaternion lastRot = Quaternion.identity;

    void Start()
    {
        StartCoroutine(DelayStart()); 
    }

    void Update()
    {
        if (!SetTimer)
        {
            if (new Vector2(transform.position.x, transform.position.y) != lastPos || transform.rotation != lastRot)
            {
                lastPos = transform.position;
                lastRot = transform.rotation;
                RemoveFromMap();
                UpdateMap();
            }
        }
    }

    void OnDestroy()
    {
        RemoveFromMap();
    }

    public void UpdateMap()
    {
        List<Vector2> checkList = new List<Vector2>();
        Bounds bR = GetComponent<Renderer>().bounds;
        Bounds bM = gameObject.GetComponent<MeshFilter>().mesh.bounds;
        checkList = DynamicSetupList(bR.min.x, bR.max.x, bR.min.y, bR.max.y, bR, bM);
  
        Pathfinder.Instance.DynamicMapEdit(checkList, UpdateList);
    }

    public void RemoveFromMap()
    {
        if (IDs != null)
        {
            Pathfinder.Instance.DynamicRedoMapEdit(IDs);
        }
    }

    private void UpdateList(List<Vector2> ids)
    {
        IDs = ids;
    }

    private List<Vector2> DynamicSetupList(float minX, float maxX, float minY, float maxY, Bounds bR, Bounds bM)
    {      
        List<Vector2> checkList = new List<Vector2>();
        float Tilesize = Pathfinder.Instance.Tilesize;

        for (float i = minY; i < maxY; i += Tilesize / 2)
        {
            for (float j = minX; j < maxX; j += Tilesize / 2)
            {
                
                    
                        Vector2 local = transform.InverseTransformPoint(new Vector2(j, i));

                        if (bM.Contains(local))
                        {
                            checkList.Add(new Vector2(j, i));
                        }
                    
                
            }
        }
        return checkList;
    }

    IEnumerator CoroutineUpdate(float _timer)
    {
        if (new Vector2(transform.position.x, transform.position.y) != lastPos || transform.rotation != lastRot)
        {
            lastPos = transform.position;
            lastRot = transform.rotation;
            RemoveFromMap();
            UpdateMap();
        }
        
        //Wait amount of time and call its self recursively
        yield return new WaitForSeconds(_timer);
        StartCoroutine(CoroutineUpdate(_timer));
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForEndOfFrame();

        lastPos = transform.position;
        lastRot = transform.rotation;
        UpdateMap();

        if (SetTimer)
        {
            StartCoroutine(CoroutineUpdate(0.2f)); //Calls it 5 times per second
        }
    }
}
