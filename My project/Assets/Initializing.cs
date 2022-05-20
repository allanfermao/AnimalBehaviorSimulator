using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializing : MonoBehaviour
{

    public Sprite circle;
    public GameObject circleElement;
    public LineRenderer lineRenderer;
    public List<Vector3> points;
    public int stepCount = 1;

    // Start is called before the first frame update
    void Start(){
        circleElement.AddComponent<Animal>();

        GameObject animal = Instantiate(circleElement);        
    }

    // Update is called once per frame
    void Update(){

    }

    void FixedUpdate(){                        

    }
}    

    
