using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProBuilder2.Common;
using ProBuilder2.MeshOperations;
using System.Linq;

public class ProcProBuilder : MonoBehaviour {

    pb_Object pb;
    public Material mat;
    public Block block;
    public ProBuilding building;

	// Use this for initialization
	void Start () {
        //List<pb_Face> faces = new List<pb_Face>(); ;

        StartCoroutine(Init());
        //pb_Object.setf
        //pb.faces.ToList().ForEach(x => Debug.Log(x.indices.ToList().Count));
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        Lot lot = block.Lots[4];
        //pb_Face face;


        //Mesh buildMesh = new Mesh();
        //buildMesh.SetVertices(lot.GetComponent<MeshFilter>().mesh.vertices.ToList());
        //buildMesh.triangles = lot.triangles;

        //GameObject buildGO = new GameObject();
        //MeshFilter filter = buildGO.AddComponent<MeshFilter>();
        //MeshRenderer renderer = buildGO.AddComponent<MeshRenderer>();
        //filter.sharedMesh = buildMesh;
        ////filter.sharedMesh = block.Lots[4].GetComponent<Mesh>();

        //buildGO.transform.SetParent(block.Lots[4].transform);

        //pb = ProBuilderize(buildGO);
        //pb.MergeFaces(pb.faces);
        ////face = faces[0];

        //pb.Extrude(new pb_Face[] { pb.faces[0] }, ExtrudeMethod.FaceNormal, 5f);
        //// pb.SetFaceMaterial(pb.faces, mat);
        //pb.faces.ToList().ForEach(x => {
        //    x.material = mat;

        //});

        ////edge loop



        //pb.ToMesh();
        //pb.Refresh();

        //pb_Edge[] createdEdges;

        //if (pb.Connect(pbMeshUtils.GetEdgeRing(pb, new pb_Edge[] { pb.faces[2].edges[2] }), out createdEdges))
        //{
        //    Dictionary<int, int> lookup = pb.sharedIndices.ToDictionary();

        //    int i = 3;
        //    pb.ToMesh();
        //    pb.Refresh();
        //    List<pb_Vertex> vertices = new List<pb_Vertex>(pb_Vertex.GetVertices(pb)); 

        //    //edges.Select(x => new pb_EdgeLookup(new pb_Edge(lookup[x.x], lookup[x.y]), x));
        //    List<pb_Edge> commonEdges = new List<pb_Edge>();
        //    HashSet <pb_EdgeLookup> distinctEdges = new HashSet<pb_EdgeLookup>(pb_EdgeLookup.GetEdgeLookup(createdEdges, lookup));


        //    distinctEdges.ToList().ForEach(x => edgeIndices.AddRange(new List<int> { x.common.x, x.common.y }));
        //    List<pb_Vertex> pbVertices = new List<pb_Vertex>();

        //    edgeIndices = edgeIndices.Distinct().ToList();

        //    distinctEdges.ToList().ForEach(x =>
        //    {
        //        pbVertices.Add(vertices[x.local.x]);
        //        pbVertices.Add(vertices[x.local.y]);
        //    });
        //    pbVertices = pbVertices.DistinctBy(x => x.position).ToList();
        //    Dictionary<int, Vector3> indexToPos = new Dictionary<int, Vector3>();
        //    for (int j = 0; j < edgeIndices.Count; j++)
        //    {
        //        indexToPos.Add(edgeIndices[j], pbVertices[j].position);
        //        pb.SetSharedVertexPosition(edgeIndices[j], indexToPos[edgeIndices[j]] + Vector3.down);
        //    }


        //    pbVertices.ForEach(x => verts.Add(x.position));
        //    pb.ToMesh();
        //    pb.Refresh();
        //}

        building = CreateProBuilding(lot);
        List<pb_Edge> edges = AddEdgeLoop(pb, pb.faces[2], pb.faces[2].edges[2]);
        ShiftIndices(pb, IndicesPositionsDict(edges), EdgeIndices(edges));
    }

    public ProBuilding CreateProBuilding(Lot _lot)
    {
        Mesh buildMesh = new Mesh();
        buildMesh.SetVertices(_lot.GetComponent<MeshFilter>().mesh.vertices.ToList());
        buildMesh.triangles = _lot.triangles;

        GameObject buildGO = new GameObject();
        MeshFilter filter = buildGO.AddComponent<MeshFilter>();
        MeshRenderer renderer = buildGO.AddComponent<MeshRenderer>();
        filter.sharedMesh = buildMesh;

        buildGO.transform.SetParent(_lot.transform);

        pb = ProBuilderize(buildGO);
        pb.MergeFaces(pb.faces);
        pb.Extrude(new pb_Face[] { pb.faces[0] }, ExtrudeMethod.FaceNormal, 5f);
        // pb.SetFaceMaterial(pb.faces, mat);
        pb.faces.ToList().ForEach(x => {
            x.material = mat;
        });
        return new ProBuilding(pb, pb.faces[0],mat);
    }

    public List<pb_Edge> AddEdgeLoop(pb_Object pb, pb_Face face, pb_Edge edge)
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

    public List<int> EdgeIndices(List<pb_Edge> edges)
    {
        Dictionary<int, int> lookup = pb.sharedIndices.ToDictionary();
        List<int> edgeIndices = new List<int>();

        HashSet<pb_EdgeLookup> distinctEdges = new HashSet<pb_EdgeLookup>(pb_EdgeLookup.GetEdgeLookup(edges, lookup));


        distinctEdges.ToList().ForEach(x => edgeIndices.AddRange(new List<int> { x.common.x, x.common.y }));

        edgeIndices = edgeIndices.Distinct().ToList();
        return edgeIndices;
    }

    public Dictionary<int, Vector3> IndicesPositionsDict(List<pb_Edge> edges)
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

    public void ShiftIndices(pb_Object pb, Dictionary<int,Vector3> dict, List<int> indices)
    {
        indices.ForEach(x =>
        {
            pb.SetSharedVertexPosition(x, dict[x] + Vector3.down);
        });
        
    }


    // Update is called once per frame
    void Update () {
		
	}
    private pb_Object ProBuilderize(GameObject gameObject)
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

[System.Serializable]
public class ProBuilding
{
    public pb_Object pb;

    public pb_Face top;
    public List<pb_Edge> Edges_Top;
    List<int> Indices_Top;

    pb_Face Front;
    List<pb_Edge> Edges_Front;
    List<int> Indices_Front;

    public ProBuilding(pb_Object _pb, pb_Face _top, Material _mat)
    {
        pb = _pb;
        top = _top;
        pb.Refresh();
        pb.ToMesh();

        
    }

    

}


