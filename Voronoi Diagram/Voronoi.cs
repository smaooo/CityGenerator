using System.Collections.Generic;
using UnityEngine;

namespace Voronoi
{
    
    public class VoronoiEdge
    {
        //These are the voronoi vertices
        public Vector3 v1;
        public Vector3 v2;

        //All positions within a voronoi cell is closer to this position than any other position in the diagram
        public Vector3 sitePos;

        public VoronoiEdge(Vector3 v1, Vector3 v2, Vector3 sitePos)
        {
            this.v1 = v1;
            this.v2 = v2;

            this.sitePos = sitePos;
        }
    }
    public class VoronoiCell
    {
        //All positions within a voronoi cell is closer to this position than any other position in the diagram
        public Vector3 sitePos;

        public List<VoronoiEdge> edges = new List<VoronoiEdge>();

        public VoronoiCell(Vector3 sitePos)
        {
            this.sitePos = sitePos;
        }
    }
}
