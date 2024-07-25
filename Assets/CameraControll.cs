using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class CameraControll : MonoBehaviour
{
    public float dragSpeed = 2;
    private Vector3 dragOrigin;
    private Vector3 origin;
 
 
    void Update()
    {
        Debug.Log(Input.mouseScrollDelta.y);
        if(Input.mouseScrollDelta.y !=0 && Camera.main.orthographicSize  - 10*Input.mouseScrollDelta.y>1){
            Camera.main.orthographicSize  -= 10*Input.mouseScrollDelta.y;
        }      
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = (Input.mousePosition);
            origin = transform.position;

            return;
        } 
        if (!Input.GetMouseButton(0)) {

            return;
        }
                Vector3 move = new Vector3((dragOrigin - (Input.mousePosition)).x , (dragOrigin - (Input.mousePosition)).y, 0);
        if (move.magnitude > 2){
        transform.position = origin + move;  
        }
        // Debug.Log(transform.position);

    }

}