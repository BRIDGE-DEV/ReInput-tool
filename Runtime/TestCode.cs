// using UnityEngine;
// using System;
// using System.IO;
// using System.Runtime.Serialization.Formatters.Binary;
// using System.Collections.Generic;
//  
// public class JSONio03 : MonoBehaviour {
//     public MainObjectData mainObject;
//     public InnerObjectData innerObject;
//  
//     List <InnerObjectData> objectList = new List<InnerObjectData> ();
//  
//     public InnerObjectData createSubObject(string name, int scores){
//         innerObject.name = name;
//         innerObject.scores = scores;
//         return innerObject;
//     }
//  
//     void Start () {
//         objectList.Add (createSubObject ("BadBoy", 8828));
//         objectList.Add (createSubObject ("MadMax", 4711));
//         mainObject.$$anonymous$$ghscore = objectList.ToArray();
//  
//         string generatedJsonString = JsonUtility.ToJson(mainObject);
//         Debug.Log ("generatedJsonString: " + generatedJsonString);
//     }
// }
//  
// [Serializable]
// public class MainObjectData {
//     public InnerObjectData [] $$anonymous$$ghscore;
// }
//  
// [Serializable]
// public class InnerObjectData {
//     public string name;
//     public int scores;
// }