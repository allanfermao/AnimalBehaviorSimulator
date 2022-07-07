using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Initializing : MonoBehaviour{
    public GameObject animal;
    public int timeScaleInDays = 120;
    public static int coordTimeIntervalInMinutes = 30;
    // max and min values
    public static Tuple<float, float> valuesX = new Tuple<float, float>(Int32.MaxValue, Int32.MinValue);
    public static Tuple<float, float> valuesY = new Tuple<float, float>(Int32.MaxValue, Int32.MinValue);
    public enum Densities {
        LION = 12,
        HYENA = 12,
        BUFFALO = 430,
        ZEBRA = 700
    };

    // Start is called before the first frame update
    void Start(){

        Animal component = animal.AddComponent<Animal>(); // get the script
        // TO DO: não add o script e sim adiciona-lo em cada for com o addComponent e copiar os dois atributos a seguir
        component.timeScaleInDays = timeScaleInDays;
        component.coordTimeIntervalInMinutes = coordTimeIntervalInMinutes;

        for(int i=0; i < (int)Densities.LION; i++){
            GameObject animalCopy = Instantiate(animal);         
            Animal scriptCopy = animalCopy.GetComponent<Animal>();            
            scriptCopy.specie = Animal.Specie.LION;
            animalCopy.name = "Lion" + (i+1).ToString(); 
        }

        for(int i=0; i < (int)Densities.HYENA; i++){
            GameObject animalCopy = Instantiate(animal);         
            Animal scriptCopy = animalCopy.GetComponent<Animal>();            
            scriptCopy.specie = Animal.Specie.HYENA;
            animalCopy.name = "Hyena" + (i+1).ToString(); 
        }

        for(int i=0; i < (int)Densities.BUFFALO; i++){
            GameObject animalCopy = Instantiate(animal);         
            Animal scriptCopy = animalCopy.GetComponent<Animal>();  
            scriptCopy.specie = Animal.Specie.BUFFALO;
            animalCopy.name = "Buffalo" + (i+1).ToString(); 
        }

        for(int i=0; i < (int)Densities.ZEBRA; i++){
            GameObject animalCopy = Instantiate(animal);         
            Animal scriptCopy = animalCopy.GetComponent<Animal>();            
            scriptCopy.specie = Animal.Specie.ZEBRA;
            animalCopy.name = "Zebra" + (i+1).ToString(); 
        }

        // Destroy(animal);

        StartCoroutine(ResizeSquare());    
        
    }

    // Update is called once per frame
    void Update(){
        // print("Init valuesX: " + valuesX.Item1 + " " + valuesX.Item2);
        // print("Init valuesY: " + valuesY.Item1 + " " + valuesY.Item2);
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

    
