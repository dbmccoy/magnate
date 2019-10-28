using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Linq;

public class AssetBullitin 
{
    public AssetEvent AddAssetToBullitinEvent;
    public List<AssetListing> Available = new List<AssetListing>();

    public void Add(AssetListing asset) {
        if(asset == null) {
            throw new System.Exception("trying to add a null listing to assetbullitin");
        }
        if(asset.Asset == null) {
            throw new System.Exception("trying to add a listing with a null asset to assetbullitin");
        }
        if(asset.Asset.Name == null) {
            throw new System.Exception("trying to add a listing without an asset name to assetbullitin");
        }
        Available.Add(asset);
        //Debug.Log("adding " + asset.Asset.Name + " to bullitin for " + asset.Price);
        AddAssetToBullitinEvent.Invoke(asset);
    }

    public void Remove(AssetListing asset) {
        Available.Remove(asset);
    }

    public AssetListing Query(IAsset asset) {
        //tell owner his asset has been queried

        return Available.Where(x => x.Asset == asset).FirstOrDefault();
    }

    public List<AssetListing> MyListings(Entity e) {
        return Available.Where(x => x.OwnedBy == e).ToList();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddListener(UnityAction<AssetListing> method) {
        AddAssetToBullitinEvent.AddListener(method);
    }

    public void RemoveListener(UnityAction<AssetListing> method) {
        AddAssetToBullitinEvent.RemoveListener(method);
    }

    private AssetBullitin() {
        AddAssetToBullitinEvent = new AssetEvent();
    }

    private static AssetBullitin instance;

    public static AssetBullitin Instance {
        get {
            if (instance == null) {
                instance = new AssetBullitin();
            }

            return instance;
        }
    }

}

public class AssetEvent : UnityEvent<AssetListing> {

}
