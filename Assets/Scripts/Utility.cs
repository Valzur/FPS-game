using UnityEngine;

public class Utility
{
    public static void SetLayerRecursively(GameObject Obj, int newLayer){
        if (Obj==null){
            return;
        } else {
            Obj.layer= newLayer;
            foreach(Transform child in Obj.transform){
                if(child==null)
                continue;

                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }
}
