using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Initializing : MonoBehaviour
{
    public Sprite circle;
    public GameObject animal;
    public List<Vector3> points;
    public int timeScaleInDays = 30;
    public int coordTimeIntervalInMinutes = 30;
    public LineRenderer lineRenderer;
    public static Tuple<float, float> valuesX = new Tuple<float, float>(Int32.MaxValue, Int32.MinValue);
    public static Tuple<float, float> valuesY = new Tuple<float, float>(Int32.MaxValue, Int32.MinValue);
    public int nAnimals = 3;
    public int stepCount = 1;

    // Start is called before the first frame update
    void Start(){

        Animal component = animal.AddComponent<Animal>(); // get the script
        component.timeScaleInDays = timeScaleInDays;
        component.coordTimeIntervalInMinutes = coordTimeIntervalInMinutes;

        for(int i=0; i < nAnimals-1; i++){
            GameObject animalCopy = Instantiate(animal); 
        }

        StartCoroutine(ResizeSquare());        
        
    }

    // Update is called once per frame
    void Update(){
        print("Init valuesX: " + valuesX.Item1 + " " + valuesX.Item2);
        print("Init valuesY: " + valuesY.Item1 + " " + valuesY.Item2);
    }

    void FixedUpdate(){                        

    }

    // get the max size of square and resize it
    IEnumerator ResizeSquare(){        
        // TO DO: implements position change of square
        // yield return new WaitWhile(() => {});                
        yield return new WaitForSeconds(0.5f);     

        float sizeX = Math.Abs(valuesX.Item1) > Math.Abs(valuesX.Item2) ? Math.Abs(valuesX.Item1) : Math.Abs(valuesX.Item2);
        sizeX *= 2;

        float sizeY = Math.Abs(valuesY.Item1) > Math.Abs(valuesY.Item2) ? Math.Abs(valuesY.Item1) : Math.Abs(valuesY.Item2);
        sizeY *= 2;

        transform.localScale = new Vector3(sizeX, sizeY, 1);           
    }
}    

    
