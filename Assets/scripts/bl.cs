using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProBuilder2.Common;
using ProBuilder2.MeshOperations;
using System.Linq;


public static class bl
{

    public static List<pb_Edge> AddEdgeLoop(pb_Object pb, pb_Face face, pb_Edge edge)
    {
        pb_Edge[] createdEdges;

        if (pb.Connect(pbMeshUtils.GetEdgeRing(pb, new pb_Edge[] { edge }), out createdEdges))
        {
            pb.ToMesh();
            pb.Refresh();
        }
        else
        {
            Debug.LogError("loop failed");
        }
        return createdEdges.ToList();
    }

    public static List<pb_Edge> AxisEdges(pb_Object pb, pb_Face face, bool horizontal = true)
    {
        List<pb_Edge> edges = new List<pb_Edge>();
        face.edges.ToList().ForEach(x =>
        {
            if (!horizontal)
            {
                if(EdgeVectors(x, pb).Max(j => j.y) > EdgeVectors(x, pb).Min(j => j.y))
                {
                    edges.Add(x);
                }
            }
            else
            {
                if (EdgeVectors(x, pb).Max(j => j.y) == EdgeVectors(x, pb).Min(j => j.y))
                {
                    edges.Add(x);
                }
            }
        });
        return edges;
    }

    public static List<pb_Face> AxisFaces(List<pb_Face> _faces,pb_Object pb, bool vertical = true)
    {
        List<pb_Face> faces = new List<pb_Face>();
        _faces.ForEach(x =>
        {
            if(FaceVerts(x,pb).Max(j => j.y) > FaceVerts(x,pb).Min(j => j.y))
            {
                faces.Add(x);
            }
        });
        return faces;
    }

    public static List<Vector3> EdgeVectors(pb_Edge edge, pb_Object pb)
    {
        List<pb_Edge> edges = new List<pb_Edge> { edge };
        return IndexPos(edges, pb).Values.ToList();
    }

    public static List<int> EdgeIndices(List<pb_Edge> edges, pb_Object pb)
    {
        Dictionary<int, int> lookup = pb.sharedIndices.ToDictionary();
        List<int> edgeIndices = new List<int>();

        HashSet<pb_EdgeLookup> distinctEdges = new HashSet<pb_EdgeLookup>(pb_EdgeLookup.GetEdgeLookup(edges, lookup));


        distinctEdges.ToList().ForEach(x => edgeIndices.AddRange(new List<int> { x.common.x, x.common.y }));

        edgeIndices = edgeIndices.Distinct().ToList();
        return edgeIndices;
    }

    public static List<Vector3> FaceVerts(pb_Face face, pb_Object pb)
    {
        return IndexPos(face.edges.ToList(),pb).Values.ToList();
    }

    public static Dictionary<int, Vector3> IndexPos(List<pb_Edge> edges, pb_Object pb)
    {
        Dictionary<int, int> lookup = pb.sharedIndices.ToDictionary();
        List<int> edgeIndices = new List<int>();

        HashSet<pb_EdgeLookup> distinctEdges = new HashSet<pb_EdgeLookup>(pb_EdgeLookup.GetEdgeLookup(edges, lookup));
        List<pb_Vertex> vertices = new List<pb_Vertex>(pb_Vertex.GetVertices(pb));


        distinctEdges.ToList().ForEach(x => edgeIndices.AddRange(new List<int> { x.common.x, x.common.y }));
        List<pb_Vertex> pbVertices = new List<pb_Vertex>();

        edgeIndices = edgeIndices.Distinct().ToList();

        distinctEdges.ToList().ForEach(x =>
        {
            pbVertices.Add(vertices[x.local.x]);
            pbVertices.Add(vertices[x.local.y]);
        });
        pbVertices = pbVertices.DistinctBy(x => x.position).ToList();
        Dictionary<int, Vector3> indexToPos = new Dictionary<int, Vector3>();
        for (int j = 0; j < edgeIndices.Count; j++)
        {
            indexToPos.Add(edgeIndices[j], pbVertices[j].position);
        }

        return indexToPos;
    }

    public static void ShiftIndices(pb_Object pb, Dictionary<int, Vector3> dict, List<int> indices, Vector3 dir)
    {
        indices.ForEach(x =>
        {
            pb.SetSharedVertexPosition(x, dict[x] + dir);
        });

    }

    public static void AdjVerts(pb_Object pb, Dictionary<int, Vector3> dict, List<int> indices, float Y)
    {
        indices.ForEach(x =>
        {
            pb.SetSharedVertexPosition(x, new Vector3(dict[x].x, Y, dict[x].z));
        });

    }

    public enum FaceType
    {
        side,
        front,
        top,
        back,
        blank
    }

    public static pb_Object ProBuilderize(GameObject gameObject)
    {
        foreach (MeshFilter mf in gameObject.GetComponentsInChildren<MeshFilter>())
        {
            if (mf.sharedMesh == null)
                continue;

            GameObject go = mf.gameObject;
            MeshRenderer mr = go.GetComponent<MeshRenderer>();

            int i = 0;
            try
            {
                pb_Object pb = gameObject.GetComponent<pb_Object>();
                if (pb == null)
                {
                    pb = gameObject.AddComponent<pb_Object>();
                }
                pbMeshOps.ResetPbObjectWithMeshFilter(pb, false);

                EntityType entityType = EntityType.Detail;

                if (mf != null)
                    go.GetComponent<MeshFilter>().sharedMesh = new Mesh();

                pb.ToMesh();
                pb.Refresh();
                //Optimize(pb);

                i++;

                if (!pb.gameObject.GetComponent<pb_Entity>())
                    gameObject.AddComponent<pb_Entity>().SetEntity(entityType);
                else
                    pb.gameObject.GetComponent<pb_Entity>().SetEntity(entityType);

                return pb;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Failed ProBuilderizing: " + go.name + "\n" + e.ToString());
            }
        }
        return null;
    }
}




