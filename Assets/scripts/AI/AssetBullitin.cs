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
        Available.Add(asset);
        AddAssetToBullitinEvent.Invoke(asset);
    }

    public void Remove(AssetListing asset) {
        Available.Remove(asset);
    }

    public AssetListing Search(IAsset asset) {
        return Available.Where(x => x.Asset == asset).First();
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
