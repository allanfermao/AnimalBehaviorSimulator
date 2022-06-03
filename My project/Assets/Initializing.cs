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

        // TODO: Collider and store the traveled distance      
        
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
        // Observação: 
        // será simulado um ambiente de 100 km², porém, o square será redimensionado de acordo com o alcance
        // máximo das trajetórias. Será calculada a distância máxima percorrida pelas indivíduos no tempo de simulação
        // O tamanho máximo do square deve ser limitado (distX <= 2000 e distY idem) por estudos empíricos
        // e esse tamanho máximo equivale a 100 km²
        
        // yield return new WaitWhile(() => {});                
        yield return new WaitForSeconds(0.5f);     

        float distX = valuesX.Item2 - valuesX.Item1;
        float distY = valuesY.Item2 - valuesY.Item1;

        float midPointX = valuesX.Item1 + distX/2;
        float midPointY = valuesY.Item1 + distY/2;

        print("distX: " + distX);
        print("distY: " + distY);
        print("midX: " + midPointX);
        print("midY: " + midPointY);

        transform.localScale = new Vector3(distX, distY, 1); // resize square
        transform.position = new Vector3(midPointX, midPointY, 0); // reposition aquare              
    }
}    

    
