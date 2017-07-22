using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DrawLine : MonoBehaviour
{
	private LineRenderer line;
	private bool isMousePressed;
	private List<Vector2> pointsList;
    [SerializeField]
    private List<myLine> lineList;
    private Vector2 mousePos;
    private float total_length;
    private float min_distance = 0.5f;
    private bool am_active;
    private bool blah = false;

    public float time_to_draw_line;


    public List<Vector2> getLine()
    {
        return pointsList;
    }

    // Structure for line points
    [System.Serializable]
    struct myLine
	{		
		public Vector2 Point;
        public float length;

        public myLine (Vector2 p, float l)
        {
            Point = p;
            length = l; 
        }
	};


	//	-----------------------------------	
	void Awake()
	{
		// Create line renderer component and set its property
		line = gameObject.AddComponent<LineRenderer>();
		line.material =  new Material(Shader.Find("Particles/Additive"));

        clearLine();
		line.SetWidth(0.1f,0.1f);
		line.SetColors(Color.green, Color.green);
		line.useWorldSpace = true;	
		isMousePressed = false;
		
        //		renderer.material.SetTextureOffset(
    }

    public List<Vector3> getFractions(int num)
    {
      //  Debug.Log("need " + num + " fractions, " + lineList.Count + " contenders\n");
        List<Vector3> fractions = new List<Vector3>();
        
        float delta = 1f / (float)num;
        float total_delta = 0f;

        while (fractions.Count < num)
        {
            fractions.Add(getFraction(total_delta));
            total_delta += delta;
            if (total_delta > 1f + delta / 2f) total_delta = 0f;
        }

        return fractions;
    }

    public Vector3 getFraction(float f)
    {
        Vector2 pickme = Vector2.zero;
        pickme = lineList[lineList.Count - 1].Point; // default

        if (f <= 0) pickme = lineList[0].Point;
        else if (f >= 1) pickme = lineList[lineList.Count-1].Point;    
        else
        {
            float need_length = f * total_length;
            float current_length = 0f;
         
            for (int i = 0; i < lineList.Count; i++)
            {             
                current_length += lineList[i].length;

                if (current_length >= need_length)
                {             
                    pickme = lineList[i].Point;                    
                    return new Vector3(pickme.x, pickme.y, 1f);
                }
            }
        }
        return new Vector3(pickme.x, pickme.y, 1f);

    }

    public void EndLine()
    {
        Debug.Log("END " + Sun.Instance.current_time_of_day + "\n");
        am_active = false;
        
    }

    public void BeginLine()
    {
        
     //   Debug.Log("drawline onbegindrag\n");
        am_active = true;
        time_to_draw_line = 0f;
        Debug.Log("START " + Sun.Instance.current_time_of_day + "\n");
        blah = true;        
        clearLine();
        line.SetColors(Color.green, Color.green);
    }

    public void clearLine()
    {
    //    Debug.Log("Clearing line\n");
        line.SetVertexCount(0);
        pointsList = new List<Vector2>();
        lineList = new List<myLine>();     
        total_length = 0f;
    }

    public void SimulateAttack(List<Vector2> positions)
    {
        clearLine();
        foreach (Vector2 pos in positions)
        {
            AddMousePos(pos);
        }

    }

    public void Update() {
        if (!am_active) return;
        if (Time.timeScale == 0) return;

        time_to_draw_line += Time.deltaTime;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      //  if (blah) Debug.Log("mousePos " + mousePos + "\n");                        
        blah = !blah;

        AddMousePos(mousePos);
    }
 
    void AddMousePos(Vector2 mousePos)
    {
        if (!pointsList.Contains(mousePos))
        {

            float dist = 0f;
            if (lineList.Count > 0) dist = Vector2.Distance(mousePos, lineList[lineList.Count - 1].Point);
            //   Debug.Log("Dist " + dist + "\n");
            if (dist >= min_distance || lineList.Count == 0)
            {
                //     Debug.Log("Added points list\n");
                pointsList.Add(mousePos);
                lineList.Add(new myLine(mousePos, dist));
                total_length += dist;



                line.SetVertexCount(pointsList.Count);
                Vector3 p = new Vector3(pointsList[pointsList.Count - 1].x, pointsList[pointsList.Count - 1].y, 0f);
                line.SetPosition(pointsList.Count - 1, p);
            }
        }
    }
}

