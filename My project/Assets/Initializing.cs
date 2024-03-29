using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Initializing : MonoBehaviour{
    public GameObject animal;
    public int timeScaleInDays = 240;
    public static int coordTimeIntervalInMinutes = 30;
    public static List<Animal> lions = new List<Animal>();
    public static List<Animal> hyenas = new List<Animal>();
    public static List<Animal> buffalos = new List<Animal>();
    public static List<Animal> zebras = new List<Animal>();
    public static int logCount = 0;    
    // max and min values
    public static Tuple<float, float> valuesX = new Tuple<float, float>(Int32.MaxValue, Int32.MinValue);
    public static Tuple<float, float> valuesY = new Tuple<float, float>(Int32.MaxValue, Int32.MinValue);
    public enum Densities {
        LION = 12,
        HYENA = 12,
        BUFFALO = 430,
        ZEBRA = 700
    };
    public static List<Animal> femaleLions = new List<Animal>();    
    public static List<Animal> femaleHyenas = new List<Animal>();

    // Start is called before the first frame update
    void Start(){

        Animal component = animal.AddComponent<Animal>(); // get the script
        component.timeScaleInDays = timeScaleInDays;
        component.coordTimeIntervalInMinutes = coordTimeIntervalInMinutes;

        for(int i=0; i < (int)Densities.LION; i++){
            GameObject animalCopy = Instantiate(animal);         
            Animal scriptCopy = animalCopy.GetComponent<Animal>();     
            lions.Add(scriptCopy);
            scriptCopy.specie = Animal.Specie.LION;
            animalCopy.name = "Lion" + (i+1).ToString();
        }

        for(int i=0; i < (int)Densities.HYENA; i++){
            GameObject animalCopy = Instantiate(animal);         
            Animal scriptCopy = animalCopy.GetComponent<Animal>();       
            hyenas.Add(scriptCopy);     
            scriptCopy.specie = Animal.Specie.HYENA;
            animalCopy.name = "Hyena" + (i+1).ToString(); 
        }

        for(int i=0; i < (int)Densities.BUFFALO; i++){
            GameObject animalCopy = Instantiate(animal);         
            Animal scriptCopy = animalCopy.GetComponent<Animal>();  
            buffalos.Add(scriptCopy);
            scriptCopy.specie = Animal.Specie.BUFFALO;
            animalCopy.name = "Buffalo" + (i+1).ToString(); 
        }

        for(int i=0; i < (int)Densities.ZEBRA; i++){
            GameObject animalCopy = Instantiate(animal);         
            Animal scriptCopy = animalCopy.GetComponent<Animal>();      
            zebras.Add(scriptCopy);      
            scriptCopy.specie = Animal.Specie.ZEBRA;
            animalCopy.name = "Zebra" + (i+1).ToString(); 
        }

        StartCoroutine(ResizeSquare());    
        
    }

    // Update is called once per frame
    void Update(){

    }

    void FixedUpdate(){                        

    }

    public static void feedToGroup(Animal.Specie specie){
        if(specie == Animal.Specie.LION){            
            lions.Sort((x, y) => x.stamina.CompareTo(y.stamina));          

            for(int i=0; i < 3; i++){
                if((lions[i].stamina < 100) && !lions[i].isDead) {               
                    print(lions[i].name + " (" + lions[i].stamina + ')' + " se alimentou numa caça em grupo");
                    lions[i].stamina = 100;
                }    
            }    
        }
        if(specie == Animal.Specie.HYENA){            
            hyenas.Sort((x, y) => x.stamina.CompareTo(y.stamina));          

            for(int i=0; i < 3; i++){
                if((hyenas[i].stamina < 100) && !hyenas[i].isDead) {               
                    print(hyenas[i].name + " (" + hyenas[i].stamina + ')' + " se alimentou numa caça em grupo");
                    hyenas[i].stamina = 100;
                }    
            }    
        }
    }

    // get the max size of square and resize it
    IEnumerator ResizeSquare(){                
        yield return new WaitForSeconds(0.5f);     

        float distX = valuesX.Item2 - valuesX.Item1;
        float distY = valuesY.Item2 - valuesY.Item1;

        float midPointX = valuesX.Item1 + distX/2;
        float midPointY = valuesY.Item1 + distY/2;

        transform.localScale = new Vector3(distX, distY, 1); // resize square
        transform.position = new Vector3(midPointX, midPointY, 0); // reposition aquare              
    }
}    

    
