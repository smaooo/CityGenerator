using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structures;

public class SimpleVisualizer : MonoBehaviour
{
    public int vertexCount = 40; // 4 vertices == square
    public float lineWidth = 0.2f;
    private float radius;

    private LineRenderer lineRenderer;

    public GameObject sphereScaler;
    /*
     attempts 
    [F][F]--F++[F][F]++F looks almost circular, not even close but the closest i've found so far
    
    angel of 90 is spread in every direction

    angle of 45 leaves 1/4 to a 1/3 of a side open, rotation towards that may make conecting spots easier

    angle of 60 comes out with things that look more like population centres

    angle 15 makes a tree

    angle 120, i'm not mad at the ressults

    150 is crap
    180 is a straight line, should have expected that really
    270 is pretty good

    varying the length and length decrement varies where the population densities are
    higher the length, the longer the lines
    higher the decrement value the quicker those lines shorten with each iteration
     */
    public LSystemGenerator lsystem;
    private GameObject systemHolder;
    List<Vector3> positions = new List<Vector3>();
    public GameObject prefab;
    public Material lineMaterial;

    [SerializeField]
    private float length = 8f;
    public bool lengthRandomizer = false;
    [SerializeField]
    private int lengthDecrement = 2;

    public bool AngleRandomizer = false;
    public float angle = 90;

    public float circleDiameter;

    
    public float Length 
    {
        get
        {
            if (length > 0) 
            {
                return length;
            }
            else
            {
            return 1;
            }
        }
        
        set => length = value; 
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        //Debug.Log(length);
        RandomizeAngle();
        RandomizeLength();
    }
    public void Start()
    {

        //var sequence = lsystem.GenerateSentence();
        //systemHolder = GameObject.FindGameObjectWithTag("System");
        //points = VoronoiCreator.CellPoints;
        //foreach (var point in points)
        //{
        //    print(point);
        //    var sequence = systemHolder.GetComponent<LSystemGenerator>().GenerateSentence(point.directions);
        //    VisualizeSequence(sequence, point);

        //}
        ////Debug.Log(length);
        //GetChildTransforms();


    }
    public  void Visualize()
    {
        RandomizeAngle();
        RandomizeLength();
        //systemHolder = GameObject.FindGameObjectWithTag("System");
        List<VoronoiToSystem> points = GetComponent<VoronoiCreator>().CellPoints;
        //var sequence = GetComponent<LSystemGenerator>().GenerateSentence(points[0].directions);
        //VisualizeSequence(sequence.Item1, sequence.Item2, points[0]);
        foreach (var point in points)
        {
            var sequence = GetComponent<LSystemGenerator>().GenerateSentence(point.directions);
            VisualizeSequence(sequence.Item1, sequence.Item2, point);

        }
        //Debug.Log(length);
        //GetChildTransforms();
    }

    
    private void SetupCircle()
    {
        lineRenderer.widthMultiplier = lineWidth;

        float deltaTheta = (2f * Mathf.PI) / vertexCount;
        float theta = 0f;

        lineRenderer.positionCount = vertexCount;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0f);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
        Instantiate(sphereScaler, transform.position, Quaternion.identity, gameObject.transform);
        sphereScaler.GetComponent<Transform>().localScale = new Vector3(radius * 2, radius * 2, radius * 2);


    }
    private void GetChildTransforms() 
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        float minX =children[0].position.x, maxX = children[0].position.x, minY = children[0].position.z, maxY = children[0].position.z;
        
        foreach (Transform child in children) 
        {
            if (child.transform.position.x >= maxX)
            {
                maxX = child.transform.position.x;
            }
            else if (child.transform.position.x <= minX)
            {
                minX = child.transform.position.x;
            }
            else if (child.transform.position.z >= maxY)
            {
                maxY = child.transform.position.z;
            }
            else if (child.transform.position.z <= minY) 
            {
                minY = child.transform.position.z;
            }
            
        }
        float xVal = Mathf.Abs(maxX - minX);
        float yVal = Mathf.Abs(maxY - minY);
        //Debug.Log("x:" + xVal);
        //Debug.Log("y: " + yVal);
        radius = (Mathf.Max(xVal, yVal))/2;
        //Debug.Log("Max val is: " + circleDiameter);
        //SetupCircle();
        
        
    }
    private void RandomizeLength() 
    {
        if (lengthRandomizer) 
        {
            length = Random.Range(4, 36);
        }
        
    }
    private void RandomizeAngle() 
    {
        if (AngleRandomizer) 
        {
            angle = Random.Range(1f, 270f);
        
        }
    }
    private void VisualizeSequence(string sequence, List<int> fCount, VoronoiToSystem cell) 
    {
        Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
        //var currentPosition = Vector3.zero;
        //var currentPosition = gameObject.transform.position;
        var currentPosition = cell.cellPosition;
        Vector3 direction = Vector3.forward;
        Vector3 tempPosition = Vector3.zero;
        int index = 0;
        
        positions.Add(currentPosition);

        foreach (var letter in sequence) 
        {
            if (savePoints.Count == 0 && letter == '|')
            {
                index++;
            }
            if (AngleRandomizer)
            {
                angle = Random.Range(index - 1 > -1 ? cell.directions[index - 1].angle + 10 : 0 + 10, index + 1 < cell.directions.Count ? cell.directions[index+1].angle - 10: 350);
                print(angle);
            }
            

            EncodingLetters encoding = (EncodingLetters)letter;
            switch (encoding)
            {
                case EncodingLetters.save:
                    savePoints.Push(new AgentParameters
                    {
                        position = currentPosition,
                        direction = direction,
                        length = Length
                    }); 
                    break;
                case EncodingLetters.load:
                    if (savePoints.Count > 0)
                    {
                        var agentParameter = savePoints.Pop();
                        currentPosition = agentParameter.position;
                        direction = agentParameter.direction;
                        Length = agentParameter.length;
                    }
                    else 
                    {
                        throw new System.Exception("Doesn't have saved point in our stack");
                    }
                    break;
                case EncodingLetters.draw:
                    tempPosition = currentPosition;
                    length = cell.directions[index].magnitude / (10*fCount[index]);
                    currentPosition += direction * length;
                    DrawLine(tempPosition, currentPosition, Color.red);
                    Length = Length - lengthDecrement; ; //makes successive iterations have smaller lines line[0] is 8, [1] is 6, [2] is 4 and so on 
                    positions.Add(currentPosition);
                    break;
                case EncodingLetters.turnRight:
                    if (savePoints.Count == 0)
                    {
                        direction = Quaternion.AngleAxis(cell.directions[index].angle, Vector3.up) * direction;
                        
                    }
                    else
                    {
                        direction = Quaternion.AngleAxis(angle, Vector3.up)*direction;

                    }
                    break;
                case EncodingLetters.turnLeft:
                    if (savePoints.Count == 0)
                    {
                        //direction = Quaternion.AngleAxis(-cell.directions[index].angle, Vector3.up) * direction;
                    }
                    else
                    {

                    }
                        direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                    break;
            }
        }

        //foreach (var position in positions) 
        //{
        //    Instantiate(prefab, position, Quaternion.identity, gameObject.transform);
            
        //}
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        //GameObject line = new GameObject("line");
        //line.transform.position = start;
        //var lineRenderer = line.AddComponent<LineRenderer>();
        //lineRenderer.material = lineMaterial;
        //lineRenderer.startColor = color;
        //lineRenderer.endColor = color;
        //lineRenderer.startWidth = 0.1f;
        //lineRenderer.endWidth = 0.1f;
        //lineRenderer.SetPosition(0, start);
        //lineRenderer.SetPosition(1, end);
        Debug.DrawLine(start, end, color, 100f);

    }

    public enum EncodingLetters 
    {
        unknown = '1',
        save = '[',
        load = ']',
        draw = 'F',
        turnRight = '+',
        turnLeft = '-'
    }
}
