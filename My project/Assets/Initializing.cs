using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializing : MonoBehaviour
{
    public Sprite circle;
    public GameObject animal;
    public LineRenderer lineRenderer;
    public List<Vector3> points;
    public int timeScaleInDays = 30;
    public int coordTimeIntervalInMinutes = 30;
    public int nAnimals = 3;
    public int stepCount = 1;

    // Start is called before the first frame update
    void Start(){

        Animal component = animal.AddComponent<Animal>();
        component.timeScaleInDays = timeScaleInDays;
        component.coordTimeIntervalInMinutes = coordTimeIntervalInMinutes;

        for(int i=0; i < nAnimals-1; i++){
            GameObject animalCopy = Instantiate(animal); 
        }
    }

    // Update is called once per frame
    void Update(){

    }

    void FixedUpdate(){                        

    }
}    

    
