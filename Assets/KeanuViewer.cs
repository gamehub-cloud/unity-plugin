using UnityEngine;
using Unity.Collections;
using UnityEditor;
using System;
[AddComponentMenu("Keanu/KeanuViewer")]
[ExecuteInEditMode]
public class KeanuViewer : MonoBehaviour
{
  GameObject refreshTrickObj;
  void Start() {
    if(!Camera.main.gameObject.GetComponent<CameraControll>()){
      Camera.main.gameObject.AddComponent<CameraControll>();
    }
  }
  void OnEnable() {

    try{
        DestroyImmediate(GameObject.Find("vert"));
    }catch{}
    EditorApplication.QueuePlayerLoopUpdate();
    refreshTrickObj = new GameObject("vert");
    refreshTrickObj.AddComponent<Vert>();
  }
  void OnDisable()
  {
    DestroyImmediate(refreshTrickObj);
  }
}