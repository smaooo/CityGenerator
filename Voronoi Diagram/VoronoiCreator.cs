using System.Collections.Generic;
using UnityEngine;
using Voronoi;
using System.Linq;
using Structures;

public class AngleComparer : IComparer<Vector3>
{
    public int Compare(Vector3 a, Vector3 b)
    {
        Vector3 center = VoronoiCreator.polygonCenter;
        double a1 = Mathf.Atan2(a.z - center.z, a.x - center.x);
        double a2 = Mathf.Atan2(b.z - center.z, b.x - center.x);

        int comp = a1.CompareTo(a2);
        return comp;
    }
}

public class VoronoiCreator : MonoBehaviour
{
    public struct Points
    {
        public Vector3 position;
        public Vector3 trianglePoint1;
        public Vector3 trianglePoint2;
        public List<List<Vector3>> edges;
    }

    struct Sortable<T> : System.IComparable<Sortable<T>>
    {
        readonly public T value;
        readonly public float key;

        public Sortable(T value, float key)
        {
            this.value = value;
            this.key = key;
        }

        public int CompareTo(Sortable<T> other)
        {
            return key.CompareTo(other.key);
        }
    }
    [SerializeField]
    private bool showPoints = false;
    [Range(0, 100)]
    public float dotRadius = 0.5f;
    public Color dotColor = Color.red;
    [Range(0,1000)]
    public int seed = 0;
    [Range(0,1000)]
    public float mapSize = 100f;
    [Range(4,150)]
    public int numberOfPoints = 5;

    [SerializeField]
    private bool showMesh = false;
    private float outterSize = 5f;
    [Space(10)]
    [HideInInspector]
    public static List<Points> finalPoints = new List<Points>();
    private List<Vector3> outSites = new List<Vector3>();
    [SerializeField]
    private bool showLines = false;

    [Header("Create Child Sites")]
    [SerializeField]
    private bool showChildSites = false;
    [SerializeField, Range(0,2)]
    private int childIterations = 0;
    [SerializeField, Range(0, 10)]
    private float childPointRadius = 1f;
    [SerializeField, Range(0,100)]
    private int childCountPerSite = 10;
    [Space(10)]
    [HideInInspector]
    public static Vector3 polygonCenter = Vector3.zero;
    private int iterations = 0;
   
    [HideInInspector]
    private static List<VoronoiCell> points = new List<VoronoiCell>();

    private void OnDrawGizmos()
    {
        points.Clear();
        iterations = childIterations;
        CreateVoronoi();
        print(CellPoints.Count);
        
    }
    
    private void Start()
    {
        CreateVoronoi();
        GetComponent<SimpleVisualizer>().Visualize();
    }
    public List<VoronoiToSystem> CellPoints
    {
        get
        {
            List<VoronoiToSystem> cells = new List<VoronoiToSystem>();

            foreach(var p in points)
            {
                List<GrowthDirection> directions = new List<GrowthDirection>();
                int index = 0;
                for (int i = 0; i < p.edges.Count; i++)
                {
                    
                    VoronoiEdge e = p.edges[i];
                    Vector3 mid = e.v1 + ((e.v2 - e.v1) / 2.0f);
                    Vector3 direction = mid - p.sitePos;
                    float magnitude = direction.magnitude;
                    float angle = 0;
                    if (i == 0)
                    {
                        angle = Quaternion.Angle(Quaternion.LookRotation(Vector3.right), Quaternion.LookRotation(direction));

                    }
                    else
                    {
                        VoronoiEdge pe = p.edges[i - 1];
                        Vector3 prevMid = pe.v1 + ((pe.v2 - pe.v1) / 2.0f);
                        Vector3 pd = prevMid - p.sitePos;
                        angle = Vector3.Distance(direction, pd);
                    }

                    directions.Add(new GrowthDirection(direction, angle, magnitude));
                   
                }
                cells.Add(new VoronoiToSystem(p.sitePos, directions));
                
            }

            return cells;
        }
    }
  
    private void DisplayChildCells(List<VoronoiCell> cells, Dictionary<VoronoiCell, Mesh> meshes, VoronoiCell parentCell)
    {
        if (showPoints)
        {
            if (!Application.isPlaying)
            Gizmos.DrawSphere(parentCell.sitePos, childPointRadius);

        }
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        Gizmos.color = color;
        foreach (VoronoiCell cell in cells)
        {
            if (showMesh)
            {
                if (!Application.isPlaying)
                    Gizmos.DrawMesh(meshes[cell]);

            }
            if (showPoints)
            {

                if (!Application.isPlaying)
                    Gizmos.DrawSphere(cell.sitePos, childPointRadius);
            }
            foreach(VoronoiEdge edge in cell.edges)
            {
                if (showLines)
                {
                    if (!Application.isPlaying)
                    {

                        Gizmos.DrawLine(edge.v1, edge.v2);
                    }
                    else
                    {
                        Debug.DrawLine(edge.v1, edge.v2, color, 100);
                    }

                }
                //Gizmos.DrawWireCube(edge.v2, Vector3.one * (cells.IndexOf(cell) + 1)  * 2);
                //Gizmos.color = Color.black;
                //Gizmos.DrawWireCube(edge.v1, Vector3.one * (cells.IndexOf(cell) + 1) );
            }
        }

       
    }
    private void DisplayVoronoiCells(Dictionary<VoronoiCell, Mesh> meshes, List<VoronoiCell> cells)
    {
        Random.InitState(seed);
      

        for (int i = 0; i < cells.Count; i++)
        {
            Gizmos.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            if (showMesh)
            {
                Gizmos.DrawMesh(meshes[cells[i]]);
            }

            foreach (var edge in cells[i].edges)
            {
                if (showLines)
                    if (!Application.isPlaying)
                    Gizmos.DrawLine(edge.v1, edge.v2);
                    else
                        Debug.DrawLine(edge.v1, edge.v2);
            }
        }

       
    }

    private void CreateVoronoi()
    {
        
        finalPoints = new List<Points>();

        List<Vector3> sites = new List<Vector3>();

        Random.InitState(seed);

        float max = mapSize / 2;
        float min = -mapSize / 2;

        for (int i = 0; i < numberOfPoints; i++)
        {
            float x = Random.Range(min, max);
            float z = Random.Range(min, max);

            sites.Add(new Vector3(x, 0, z));
        }
        float outSize = mapSize / 2 * outterSize;
        sites.Add(new Vector3(0, 0, outSize));
        sites.Add(new Vector3(0, 0, -outSize));
        sites.Add(new Vector3(outSize, 0, 0));
        sites.Add(new Vector3(-outSize, 0, 0));

        outSites.Add(new Vector3(0, 0, outSize));
        outSites.Add(new Vector3(0, 0, -outSize));
        outSites.Add(new Vector3(outSize, 0, 0));
        outSites.Add(new Vector3(-outSize, 0, 0));

        
        List<VoronoiCell> cells = DelaunayToVoronoi.GenerateVoronoiDiagram(sites);
        var meshes = CreateMeshes(cells);
        DisplayVoronoiCells(meshes, cells);
        points.AddRange(cells);
        Gizmos.color = dotColor;

        if (showPoints)
        {
            for (int i = 0; i < sites.Count; i++)
            {
                if (!Application.isPlaying)
                    Gizmos.DrawSphere(sites[i], dotRadius);
            }

        }


        //if (showConnections)
        //{

        //    Gizmos.color = connectionColor;

        //    var tris = Delaunay.TriangulateByFlippingEdges(sites);
        //    foreach (var t in tris)
        //    {

        //        Gizmos.DrawLine(t.v1.position, t.v2.position);
        //        Gizmos.DrawLine(t.v2.position, t.v3.position);
        //        Gizmos.DrawLine(t.v1.position, t.v3.position);

        //    }
        //}
        

        if (showChildSites)
        {
            CreateChildCells(meshes);
           
        }
    }

    private List<VoronoiCell> CutEdges(List<VoronoiCell> cells, VoronoiCell parentCell, Dictionary<VoronoiCell, Mesh> meshes)
    {
        List<VoronoiCell> opCells = new List<VoronoiCell>(cells);
        Dictionary<Vector3, List<Vector3>> edgeVerts = new Dictionary<Vector3, List<Vector3>>();

        // find intersection points of line segments that go out of parent cell bound
        for (int i = 0; i < opCells.Count; i++)
        {
            VoronoiCell c = opCells[i];
            for (int j = 0; j < c.edges.Count; j++)
            {
                VoronoiEdge ec = c.edges[j];

                bool changed = false;
                foreach (var pc in parentCell.edges)
                {
                    if (changed)
                    {
                        continue;
                    }
                    Vector3 sitepos = ec.sitePos;

                    Vector3 p1 = ec.v1;
                    Vector3 p2 = ec.v2;
                    Vector3 p3 = pc.v1;
                    Vector3 p4 = pc.v2;
                    
                    Vector3 intersection = Vector3.zero;

                    if (FindIntersection(p1, p2, p3, p4, out intersection))
                    {
                        changed = true;
                        c.edges[j].v2 = intersection;

                        var tmp = c.edges[j].v1;
                        c.edges[j].v1 = c.edges[j].v2;
                        c.edges[j].v2 = tmp;
                        if (edgeVerts.ContainsKey(c.sitePos))
                        {
                            edgeVerts[c.sitePos].Add(intersection);
                        }
                        else
                        {
                            edgeVerts.Add(c.sitePos, new List<Vector3>() { intersection });
                        }
                        

                    }
                    
                }
            }
        }
        

        
        // remove edges that are out of bound
        for (int i = 0; i < opCells.Count; i++)
        {
            VoronoiCell cell = opCells[i];
            List<VoronoiEdge> toRemove = new List<VoronoiEdge>();
            Mesh mesh = meshes[parentCell];


            foreach(var edge in cell.edges)
            {
                foreach (var pe in parentCell.edges)
                {
                    //Vector3 p1 = edge.v1 + (edge.v2 - edge.v1) / 2;
                    Vector3 p1 = edge.v2;
                    Vector3 p2 = CalculateCenter(mesh.vertices.ToList());

                    Vector3 p3 = pe.v1;
                    Vector3 p4 = pe.v2;
                    Vector3 intersection = Vector3.zero;
                    if (FindIntersection(p1, p2, p3, p4, out intersection))
                    {
                        toRemove.Add(edge);
                    }
                    //if (!mesh.bounds.Contains(edge.v1) || !mesh.bounds.Contains(edge.v2))
                    //{
                    //    toRemove.Add(edge);
                    //}
                }
            }

            opCells[i].edges.RemoveAll(v => toRemove.Contains(v));
            
            //foreach (var edge in cell.edges)
            //{
            //    if (!mesh.bounds.Contains(edge.v1) || !mesh.bounds.Contains(edge.v2))
            //    {

            //    }
            //}
        }

        //// remove cells that don't have any edges
        //for (int i = 0; i < opCells.Count; i++)
        //{
        //    if (opCells[i].edges.Count == 0)
        //    {
        //        opCells.RemoveAt(i);
        //    }
        //}


        // add parent cell corners to adjacent child cells
        foreach (var pe in parentCell.edges)
        {
            float distance = float.PositiveInfinity;
            Vector3 pos = Vector3.zero;

            foreach (var kvp in edgeVerts)
            {
                HashSet<Vector3> verts = new HashSet<Vector3>();

                foreach (var e in kvp.Value)
                {
                    verts.Add(e);
                }

                Vector3 center = CalculateCenter(verts.ToList());

                if (Vector3.Distance(kvp.Key, pe.v1) < distance)
                {
                    distance = Vector3.Distance(center, pe.v1);
                    pos = kvp.Key;
                }
            }
            VoronoiEdge edge1 = new VoronoiEdge(pe.v1, edgeVerts[pos][0], pos);
            var c = opCells.Find(c => c.sitePos == pos);
            if (c != null) c.edges.Add(edge1);
        }

        // reorder the vertices in every cell and create the edges from scratch 
        for (int i = 0; i < opCells.Count; i++)
        {
            VoronoiCell c = opCells[i];
            HashSet<Vector3> verts = new HashSet<Vector3>();

            foreach (var e in c.edges)
            {
                verts.Add(e.v1);
                verts.Add(e.v2);
            }
            var vertices = new List<Vector3>(verts);

            polygonCenter = CalculateCenter(vertices);
            AngleComparer ac = new AngleComparer();
            vertices.Sort(ac);

            List<VoronoiEdge> edges = new List<VoronoiEdge>();
            for (int j = 0; j < vertices.Count; j++)
            {
                int index = j + 1 < vertices.Count ? j + 1 : 0;
                VoronoiEdge edge = new VoronoiEdge(vertices[j], vertices[index], c.sitePos);
                //print(i + " : " + vertices[j] + " " + vertices[index] + "PoS: " + c.sitePos);
                edges.Add(edge);
            }
            opCells[i].edges = new List<VoronoiEdge>(edges);

        }

        //for (int i = 0; i < opCells.Count; i++)
        //{
        //    VoronoiCell c = opCells[i];
        //    Vector3 p1 = c.sitePos;
        //    Vertex v1 = new Vertex(p1);
        //    List<Triangle> tris = new List<Triangle>();
        //    foreach (var e in c.edges)
        //    {
        //        Vertex v2 = new Vertex(e.v2);
        //        Vertex v3 = new Vertex(e.v1);

        //        v1.prevVertex = v3;
        //        v1.nextVertex = v2;

        //        v2.prevVertex = v1;
        //        v2.nextVertex = v3;

        //        v3.prevVertex = v2;
        //        v3.nextVertex = v1;

        //        HalfEdge he1 = new HalfEdge(v1);
        //        HalfEdge he2 = new HalfEdge(v2);
        //        HalfEdge he3 = new HalfEdge(v3);

        //        Triangle triangle = new Triangle(v1, v2, v3);
        //        he1.t = triangle;
        //        he2.t = triangle;
        //        he3.t = triangle;
        //        triangle.halfEdge = he3;
        //        he1.prevEdge = he3;
        //        he1.nextEdge = he2;
        //        he2.prevEdge = he1;
        //        he2.nextEdge = he3;
        //        he3.prevEdge = he2;
        //        he3.nextEdge = he1;
        //        tris.Add(triangle);

        //    }
        //    Delaunay.OrientTrianglesClockwise(tris);
        //    List<VoronoiEdge> edges = new List<VoronoiEdge>();
        //    foreach (var t in tris)
        //    {
        //        edges.Add(new VoronoiEdge(t.v3.position, t.v2.position, t.v1.position));

        //    }
        //    opCells[i].edges = new List<VoronoiEdge>(edges);
        //}
        return opCells;
    }

    private static bool FindIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 intersection)
    {
        intersection = Vector3.zero;
        var d = (p2.x - p1.x) * (p4.z - p3.z) - (p2.z - p1.z) * (p4.x - p3.x);
        if (d != 0)
        {
            var u = ((p3.x - p1.x) * (p4.z - p3.z) - (p3.z - p1.z) * (p4.x - p3.x)) / d;
            var v = ((p3.x - p1.x) * (p2.z - p1.z) - (p3.z - p1.z) * (p2.x - p1.x)) / d;
             intersection = Vector3.zero;
            if (u >= 0.0f || u <= 1.0f || v >= 0.0f || v <= 1.0f)
            {
                intersection.x = p1.x + u * (p2.x - p1.x);
                intersection.z = p1.z + u * (p2.z - p1.z);
                float dot1 = Vector3.Dot((p4 - p3).normalized, (intersection - p4).normalized);
                float dot2 = Vector3.Dot((p3 - p4).normalized, (intersection - p3).normalized);
                float dot3 = Vector3.Dot((p2 - p1).normalized, (intersection - p2).normalized);
                float dot4 = Vector3.Dot((p1 - p2).normalized, (intersection - p1).normalized);

                if (dot1 < 0f && dot2 < 0f && dot3 < 0f && dot4 < 0f)
                {
                    
                    return true;
                }
            }
        }

        return false;
    }
    private static Vector3 CalculateCenter(List<Vector3> vertices)
    {
        Vector3 center = Vector3.zero;
        float x = 0;
        float y = 0;
        foreach (var v in vertices)
        {
            x += v.x;
            y += v.z;
        }
        center.x = x / vertices.Count;
        center.z = y / vertices.Count;
        return center;
    }
    private Dictionary<VoronoiCell, Mesh> CreateMeshes(List<VoronoiCell> cells)
    {
        Dictionary<VoronoiCell, Mesh> meshes = new Dictionary<VoronoiCell, Mesh>();

        for (int i = 0; i < cells.Count; i++)
        {
            VoronoiCell c = cells[i];

            Vector3 p1 = c.sitePos;

            Gizmos.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);

            List<Vector3> vertices = new List<Vector3>();

            List<int> triangles = new List<int>();
            vertices.Add(p1);
            
            for (int j = 0; j < c.edges.Count; j++)
            {
                Vector3 p3 = c.edges[j].v1;
                Vector3 p2 = c.edges[j].v2;


                vertices.Add(p2);
                vertices.Add(p3);
                
                triangles.Add(0);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 1);
            }

            //foreach (var t in tris)
            //{
            //    //triangles.Add(0);
            //    //var vt1 = vertices.Find(v => v == t.v2.position);
            //    //triangles.Add(vertices.IndexOf(vt1));
            //    //var vt2 = vertices.Find(v => v == t.v3.position);
            //    //triangles.Add(vertices.IndexOf(vt2));

        
            //}

            Mesh triangleMesh = new Mesh();
            triangleMesh.vertices = vertices.ToArray();
            triangleMesh.triangles = triangles.ToArray();

            triangleMesh.RecalculateNormals();
            meshes.Add(c, triangleMesh);
            
        }
        return meshes;
    }
    private void CreateChildCells(Dictionary<VoronoiCell, Mesh> meshes)
    {

        Random.InitState(seed);

        foreach (KeyValuePair<VoronoiCell, Mesh> mesh in meshes)
        {
            if (outSites.Contains(mesh.Key.sitePos))
            {
                continue;
            }
            Color siteColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            Gizmos.color = siteColor;
            List<Vector3> childSites = new List<Vector3>();

            Vector3 maxB = mesh.Value.bounds.max;
            Vector3 minB = mesh.Value.bounds.min;
            Vector3 center = mesh.Value.bounds.center;
            float dist = Vector3.Distance(minB, maxB);
            childSites.Add(center + new Vector3(0, 0, dist));
            childSites.Add(center + new Vector3(0, 0, -dist));
            childSites.Add(center + new Vector3(dist, 0, 0));
            childSites.Add(center + new Vector3(-dist, 0, 0));


            for (int i = 0; i < mesh.Value.triangles.Length; i+=3)
            {
                for (int j = 0; j < childCountPerSite; j++)
                {

                    Mesh m = mesh.Value;
                    var vertex = new Vector3[m.vertexCount];
                    for (int v = 0; v < m.vertexCount; v++)
                    {
                        vertex[v] = m.bounds.center + (m.vertices[v] - m.bounds.center) * 0.95f;
                    }
                  
                    float r1 = Random.value;
                    float r2 = Random.value;

                    int v1 = m.triangles[i];
                    int v2 = m.triangles[i+1];
                    int v3 = m.triangles[i+2];
                   
                    Vector3 p = (1 - Mathf.Sqrt(r1)) * vertex[v1] +
                        (Mathf.Sqrt(r1) * (1 - r2)) * vertex[v2] +
                        (r2 * Mathf.Sqrt(r1)) * vertex[v3];
                    
                    childSites.Add(p);
                    


                }


            }
            
            List<VoronoiCell> cells = DelaunayToVoronoi.GenerateVoronoiDiagram(childSites);
            cells = new List<VoronoiCell>(CutEdges(cells, mesh.Key, meshes));
            var childMeshes = CreateMeshes(cells);
            points.AddRange(cells);
            DisplayChildCells(cells, childMeshes, mesh.Key);
            if (iterations > 0)
            {
                iterations--;
                CreateChildCells(childMeshes);
            }

        }


    }
    
}
