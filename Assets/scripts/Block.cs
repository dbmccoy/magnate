using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Block: MonoBehaviour {

    List<Segment> boundingSegments = new List<Segment>();
    List<Intersection> intersections = new List<Intersection>();
    public List<Road> boundingRoads;
    public List<Vector3> verts = new List<Vector3>();
    Mesh mesh;
    
    public void InitBlock(List<Segment> _boundingSegments) {
        foreach (var item in _boundingSegments) {
            if (!boundingSegments.Contains(item)) {
                boundingSegments.Add(item);
                item.intersections.ForEach(i => Debug.Log(i.transform.name));              
            }
        }
        //Debug.Log(boundingSegments.Count);
        //boundingSegments.Where(i => !verts.Contains(i.start())).ToList().ForEach(j => verts.Add(j.intersections[0].transform.position));
        foreach (var seg in boundingSegments) {
            foreach (var inter in seg.intersections) {
                bool match = true;
                foreach (var inter_seg in inter.segments) {
                    if (!boundingSegments.Contains(inter_seg)) {
                        match = false;
                    }
                }
                if (match) {
                    intersections.Add(inter);
                }
            }
        }
        //cull segment pairs that are outside the block bounds
        List<Intersection> adjIntersections = new List<Intersection>();
        Dictionary<Intersection, Segment[]> intersectionToSegments = new Dictionary<Intersection, Segment[]>();
        foreach (var item in intersections) {
            intersectionToSegments.Add(item, item.segments.ToArray());
        }
        foreach (var item in intersectionToSegments) {
            Dictionary<Intersection, Segment[]> temp = intersectionToSegments;
            temp.Remove(item.Key);
            if (temp.ContainsValue(item.Value)) {
                if (Vector3.Distance(item.Key.transform.position, transform.position) > Vector3.Distance(temp.FirstOrDefault(x => x.Value == item.Value).Key.transform.position, transform.position)) {
                    intersections.Remove(item.Key);
                }
                //segment = road.segmentToObj.FirstOrDefault(x => x.Value == GetComponent<SegmentCollider>()).Key; //reverse lookup on dictionary
            }
        }
        //boundingSegments.ForEach(i => i.intersections.Where(j => !verts.Contains(j.transform.position)).ToList().ForEach(k => verts.Add(k.transform.position)));
        //intersections.Where(i => !verts.Contains(i.transform.position)).ToList().ForEach(j => verts.Add(j.transform.position));
        foreach (var item in intersections) {
            if (!verts.Contains(item.transform.position)) {
                verts.Add(item.transform.position);
            }
        }
        boundingSegments.ForEach(i => boundingRoads.Add(i.road));
        verts.ForEach(i => Instantiate(Resources.Load("boundingPoint"), i, Quaternion.identity));
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        renderer.material = (Material)Resources.Load("materials/lot");
        filter.mesh = new Mesh();
        filter.mesh.SetVertices(verts);
        filter.mesh.uv = new Vector2[] {
            new Vector2 (0, 0),
            new Vector2 (0, 1),
            new Vector2(1, 1),
            new Vector2 (1, 0)
        };
        filter.mesh.triangles = (new int[] { 0, 1, 2, 0, 2, 3 });
        filter.mesh.RecalculateNormals();
        Debug.Log("all done");
    }
}
