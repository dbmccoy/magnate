using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProBuilder2.Common;
using ProBuilder2.MeshOperations;
using System.Linq;

[System.Serializable]
public class ProBuilding
{
    public float floorHeight;
    public List<bl_Floor> floors = new List<bl_Floor>();
    float height;

    public pb_Object pb;
    public bl_Params parameters;

    public pb_Face top;
    public List<int> Indices_Top;

    //public pb_Face front;
    List<int> Indices_Front;
    public Lot lot;

    public ProBuilding(pb_Object _pb, pb_Face _top, Material _mat, Lot _lot, bl_Params _parameters)
    {
        parameters = _parameters;
        pb = _pb;
        top = _top;
        lot = _lot;
        height = bl.FaceVerts(top, pb).Max(x => x.y);
        Debug.Log("height" + height);
        //DefineFloors();
        //front.SetMaterial((Material)Resources.Load("materials/buildings/green"));
        //front.material = (Material)Resources.Load("materials/buildings/green");
        //bl.ShiftIndices(pb, bl.IndexPos(front.edges.ToList(), pb), bl.EdgeIndices(front.edges.ToList(), pb), Vector3.forward);
        //pb.Refresh();
        pb.ToMesh();
        //pbMeshOps.CombineObjects()

    }

    public void AddFloor(bl_Floor fl)
    {
        floors.Add(fl);
    }





}