using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProBuilder2.Common;
using ProBuilder2.MeshOperations;
using System.Linq;

public class bl_Controller : MonoBehaviour
{

    pb_Object pb;
    public Material mat;
    public Material frontMat;
    public Block block;
    public ProBuilding building;

    public GameObject window;
    public bl_Plan plan;

    // Use this for initialization
    void Start()
    {
        //List<pb_Face> faces = new List<pb_Face>(); ;

        StartCoroutine(Init());
        //pb_Object.setf
        //pb.faces.ToList().ForEach(x => Debug.Log(x.indices.ToList().Count));
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        Lot lot = block.Lots[4];

        plan = new bl_Plan(mat, frontMat, .5f);
        block.Lots.ForEach(x => CreateProBuilding(x, plan));
        //building = CreateProBuilding(lot);
    }

    public ProBuilding CreateProBuilding(Lot _lot, bl_Plan _plan)
    {
        if(_lot.z_Class == Zone.ZoneClass.R5)
        {
            _lot.Build();
            return null;
        }
        Mesh buildMesh = new Mesh();
        buildMesh.SetVertices(_lot.GetComponent<MeshFilter>().mesh.vertices.ToList());
        buildMesh.triangles = _lot.triangles;

        GameObject buildGO = new GameObject();
        MeshFilter filter = buildGO.AddComponent<MeshFilter>();
        MeshRenderer renderer = buildGO.AddComponent<MeshRenderer>();
        filter.sharedMesh = buildMesh;

        buildGO.transform.SetParent(_lot.transform);
        buildGO.transform.name = _lot.Address + " G";

        pb = bl.ProBuilderize(buildGO);
        pb.MergeFaces(pb.faces);
        pb.Extrude(new pb_Face[] { pb.faces[0] }, ExtrudeMethod.FaceNormal, _plan.floorHeight);
        // pb.SetFaceMaterial(pb.faces, mat);
        pb_Face front = null;
        ProBuilding bld = new ProBuilding(pb, pb.faces[0], mat, _lot, _plan);
        pb.faces.ToList().ForEach(x => x.material = mat);
        //front.material = frontMat;
        bl_Floor gr = new bl_Floor(pb,bld);
        gr.front.material = _plan.frontMat;

        pb.ToMesh();
        pb.Refresh();
        GenFacade(gr);
        bld.AddFloor(gr);
        bl_Floor fl = AddFloor(pb, bld);
        
        //pbMeshOps.CombineObjects(new pb_Object[] { pb, fl.pb },out pb);
        return bld;
    }

    public bl_Floor AddFloor(pb_Object _pb, ProBuilding bld)
    {
        Mesh buildMesh = new Mesh();
        //buildMesh.SetVertices(_lot.GetComponent<MeshFilter>().mesh.vertices.ToList());
        buildMesh.SetVertices(bl.FaceVerts(_pb.faces[0], _pb));

        buildMesh.triangles = Utils.Triangles(buildMesh);
        //buildMesh.triangles = _pb.faces[0].indices;

        GameObject buildGO = new GameObject();
        MeshFilter filter = buildGO.AddComponent<MeshFilter>();
        MeshRenderer renderer = buildGO.AddComponent<MeshRenderer>();
        filter.sharedMesh = buildMesh;

        buildGO.transform.SetParent(bld.lot.transform);
        buildGO.transform.name = bld.lot.Address + (bld.floors.Count + 1);

        pb = bl.ProBuilderize(buildGO);
        pb.MergeFaces(pb.faces);
        pb.Extrude(new pb_Face[] { pb.faces[0] }, ExtrudeMethod.FaceNormal, 1f);
        pb.faces.ToList().ForEach(x => x.material = bld.parameters.baseMat);
        bl_Floor fl = new bl_Floor(pb,bld);
        GenFacade(fl);
        bld.AddFloor(fl);
        fl.front.material = bld.parameters.frontMat;
        pb.ToMesh();
        pb.Refresh();
        return fl;
    }

    public void GenFacade(bl_Floor fl)
    {
        fl.walls.ForEach(j =>
        {
            if(fl.faceTypeDict[j] == bl.FaceType.front || fl.faceTypeDict[j] == bl.FaceType.side)
            {
                fl.centerDict[j].ForEach(x => Instantiate(window, x, fl.WallNormal(fl.centerDict[j]), fl.pb.transform));
            }
            //fl.centerDict.Values.ToList().ForEach(x =>
            //{
            //    x.ForEach(j => Instantiate(window, j, fl.WallNormal(x), fl.pb.transform));
            //});
        });
    }
}

public class bl_Plan
{
    public Material baseMat;
    public Material frontMat;
    public Material topMat;

    public float floorHeight;

    public bl_Plan(Material _baseMat, Material _frontMat, float fl_Height)
    {
        baseMat = _baseMat;
        frontMat = _frontMat;
        floorHeight = fl_Height;
        //topMat = _topMat;
    }
}





