using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Animal : MonoBehaviour{    

    public int stepCount = 1;
    public int timeScaleInDays;
    public List<Vector3> points;
    public Color color = Color.blue; 
    public LineRenderer lineRenderer;
    public SpriteRenderer spriteRenderer;
    public int coordTimeIntervalInMinutes;   
    public GameObject animal;
    // Devido à distruibuição homogênea os valores limites variam, geralmente, entre -500 e 500 nos dois eixos
    // Os valores dos limites podem ser escolhidos usando estudos empíricos
    public Dictionary<string, float> limits = new Dictionary<string, float>(){
        {"L", -300f},
        {"R", 300f},
        {"T", 300f},
        {"B", -300f}
    };
    public enum Specie {
        LION,
        ZEBRA,
        HYENA,
        BUFFALO,
        UNDEFINED
    };    
    public enum Feed {
        HERBIVORE,
        CARNIVORE
    };
    public enum Gender {
        M,
        F
    };

    // Individual Attributes
    public Gender gender;
    public int age = 0; // months    
    public int stamina = 100;
    public bool isDead = false;
    public int currentNChilds = 0;
    public bool isPregnant = false;
    public bool interacting = false;
    public int dailyLossStamina = 15;
    public float travelledDistance = 0;
    public Specie specie = Specie.UNDEFINED; // enum
    public int timeOfCurrentGestation = -1; // minutes
    public List<GameObject> childrens = new List<GameObject>();
    public List<string> huntingTargetsFailed = new List<string>();

    // Specie Attributes
    public Feed feed;
    public int deathRate;
    public int averageSpeed;
    public float speed = 100f;
    public int numberChildrens; 
    public int deathRateChildrens;
    public int gestationTime; // days
    public int maxSurvivalTime = 1000; // months
    public int sexualMaturity; // months
    public int timeBetweenBirths; // months
    public int timeToIndependence; // months
    public int populationDensity; // per 100km²
    public float dailyTravelledDistance; // km/24h
    public float groupHuntingSuccessRate; 
    public float individualHuntingSuccessRate; 
    public List<Specie> targets;

    // Start is called before the first frame update
    void Start(){
        StartCoroutine(InitializeAnimal());
        animal = GameObject.Find("AnimalBase");
        Animal animalScript = animal.GetComponent<Animal>();
        timeScaleInDays = animalScript.timeScaleInDays;
        coordTimeIntervalInMinutes = animalScript.coordTimeIntervalInMinutes;

        float mu = 2.2f;
        float x = UnityEngine.Random.Range(limits["L"], limits["R"]);
        float y = UnityEngine.Random.Range(limits["B"], limits["T"]);     

        int steps = (int)(60 / coordTimeIntervalInMinutes) * 24 * timeScaleInDays; // times per hour * 24 hours * days

        points = SimmLevy(steps, mu, new Vector3(x, y, -0.5f));

        spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite circle = spriteRenderer.sprite; // set the sprite

        transform.position = points[0]; // set initial position

    }

    // Update is called once per frame
    void Update(){

    }

    void FixedUpdate(){
        float step = speed * Time.fixedDeltaTime; // Set the step size

        // O modo ideal de implementação é fazer com que passe o mesmo tempo entre cada passo, o que significa ajustar 
        // a velocidade com base no tamanho do passo a ser dado. Passos mais curtos significam velocidades menores, já
        // que todos são executados no mesmo tempo

        // TODO: avaliar a distância percorrida por um indivíduo durante um espaço de tempo criar um somatório 
        // dos tamanhos de passos até atingir a distância total da simulação, ajustando a velocidade, sem pausas. 
        // Para que todos os indivíduos terminem juntos, uma abordagem é parar quando o primeiro finalizar.
        // outra abordagem é simular momentos de descanso e avaliar os valores de distância percorrida e tempo de atividade
        // frequentemente. Com base nisso, ajustar a distância percorrida, obedecendo a velocidade padrão. 

        if(stepCount < points.Count){
            Vector3 beforeStep = new Vector3(transform.position.x, transform.position.y, -0.5f);
            transform.position = Vector3.MoveTowards(transform.position, points[stepCount], step); // Move the individual gradually
            Vector3 afterStep = new Vector3(transform.position.x, transform.position.y, -0.5f);

            travelledDistance += Vector3.Distance(beforeStep, afterStep);

            if(Vector3.Distance(transform.position, points[stepCount]) == 0f)
                stepCount++;      

            Collider2D[] animalsInRange = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 50f);            
            if(huntingTargetsFailed.Count > 0){
                foreach(var animalName in new List<string>(huntingTargetsFailed)){ // remove target of vector of fails if is out of the range
                    if(!Array.Exists(animalsInRange, x => x.name == animalName))                
                        huntingTargetsFailed.Remove(animalName);
                }
            }
            foreach(var animal in animalsInRange){
                Animal animalS = animal.gameObject.GetComponent<Animal>();
                // PREDATION ROUTINE
                if(animal.name != name){
                    if(feed == Feed.CARNIVORE && !huntingTargetsFailed.Exists(x => x == animal.name) && !animalS.isDead && stamina <= (100 - 2*dailyLossStamina)){  // only if the animals not have interacted previously and failed                        
                        // proceed just if stamina is not high                    
                        if(!animalS.interacting && targets.Contains(animalS.specie) && age >= timeToIndependence){ // only if animal is independent
                            interacting = true;  
                            float chance = UnityEngine.Random.Range(0.000001f, 1f);
                            // sugestão para diminuir o número de mortes de herbívoros: diminuir o interesse dos animais na caça (simular períodos de caça com um sorteio de "interesse")
                            if(chance <= groupHuntingSuccessRate){ // success 
                                // TO DO: Usar taxa de sucesso. Avaliar possibilidade de caça em grupo e individual:
                                // avaliar se existe mais algum indivíduo da espécie ou bando no range para ter a taxa de grupo
                                animalS.enabled = false; // disable script of dead animal
                                animalS.spriteRenderer.color = Color.grey; 
                                animalS.stamina = 0; // targets are totaly consumed
                                animalS.isDead = true;
                                interacting = false;
                                spriteRenderer.color = color;
                                print(name + " matou " + animal.name);
                                stamina = 100;
                                // feed of childrens
                                foreach(var child in childrens){
                                    Animal scriptChild = child.GetComponent<Animal>();
                                    if(!scriptChild.isDead && age <= timeToIndependence) // if child is not dead and is not independent
                                        scriptChild.stamina = 100;
                                }
                            }
                            else{ // fail
                                // interacting = false;
                                // por não setar interacting pra false em falha, o animal continua interagindo com o outro
                                // mesmo que e outro já tenho morrido.
                                huntingTargetsFailed.Add(animal.name);
                                // print(name + " falhou com " + animal.name);
                            }                        
                        }                                               
                    } 
                    // scavenger possibility

                    // REPRODUCTIVE ROUTINE             
                    if(animalS.specie == specie && gender == Gender.M && animalS.gender == Gender.F && !animalS.isPregnant && !animalS.isDead){ 
                        if(animalS.age >= animalS.sexualMaturity && age >= sexualMaturity){                       
                            animalS.isPregnant = true;
                            animalS.timeOfCurrentGestation = 0;            
                            print(animalS.name + " copulou com " + name + " e está prenhe");            
                        }
                    }
                }                     
            }    

            // if(interacting){
            //     spriteRenderer.color = Color.Lerp(color, Color.red, Mathf.PingPong(Time.time*2, 1));
            // }  
            // else{
            //     spriteRenderer.color = color;
            // }  

            // if(animalsInRange.Length == 1){ // just the own element                
            //     spriteRenderer.color = color;
            //     interacting = false;
            // }

            Array.Clear(animalsInRange, 0, animalsInRange.Length);

            // REPRODUCTIVE ROUTINE (INDIVIDUAL)
            if(gender == Gender.F && isPregnant && Vector3.Distance(transform.position, points[stepCount-1]) == 0f){                
                timeOfCurrentGestation += coordTimeIntervalInMinutes; // in minutes
                int timeInDays = (int)((timeOfCurrentGestation / 60) / 24);
                if(timeInDays >= gestationTime){ // ready to born
                    System.Random rand = new System.Random(); 
                    for(int i=0; i < numberChildrens; i++){
                        GameObject child = Instantiate(animal);
                        Destroy(child.GetComponent<Animal>());
                        Animal scriptChild = child.AddComponent<Animal>();
                        scriptChild.enabled = false;
                        // copy points and stepCount of mother
                        scriptChild.points = points;
                        scriptChild.stepCount = stepCount;
                        scriptChild.specie = specie;
                        scriptChild.age = 0;
                        scriptChild.travelledDistance = 0f;
                        scriptChild.enabled = true;
                        child.name = name + '_' + (currentNChilds+1).ToString(); 
                        print(name + " deu à luz " + child.name);                        
                        currentNChilds++;
                        childrens.Add(child);
                    }
                    timeOfCurrentGestation = -1;
                    isPregnant = false;
                }
            }

            // INDIVIDUAL EVOLUTION
            if(Vector3.Distance(transform.position, points[stepCount-1]) == 0f){ // just at end of step
                if((((stepCount * coordTimeIntervalInMinutes / 60) / 24) % 30) == 0) // if have passed one month
                    age++;            
                
                if(feed == Feed.CARNIVORE && (stepCount * coordTimeIntervalInMinutes / 60) % 24 == 0) // if have passed one day
                    stamina -= dailyLossStamina; // REAVALIAR ESSE VALOR 

                // DEATH ROUTINE (natural causes)
                // evaluate stamina, maxSurvivalTime, deathRate and deathRateChildrens, 
                if((stamina <= 5 || age > maxSurvivalTime)){
                    isDead = true;
                    stamina = 10; // can be consumed by carnivores and scavengers
                    spriteRenderer.color = Color.grey; 
                    gameObject.GetComponent<Animal>().enabled = false;
                    print(name + " morreu por causas naturais");
                    if(gender == Gender.F && childrens.Count >= 0)
                        killChildrens();
                }            
            }

        }    
        else{
            print("Traveled distance by " + name + ": " + travelledDistance);
            gameObject.GetComponent<Animal>().enabled = false;
        }    
    }

    void killChildrens(){        
        foreach(var child in childrens){
            Animal scriptChild = child.GetComponent<Animal>();
            if(scriptChild.age <= timeToIndependence){ // just if is a young children
                scriptChild.isDead = true;
                scriptChild.stamina = 10; // can be consumed by carnivores and scavengers
                scriptChild.spriteRenderer.color = Color.grey; 
                scriptChild.enabled = false;
                print(child.name + " morreu em razão da morte da mãe");
            }
        }
    }

    List<Vector3> SimmLevy(int steps, float mu, Vector3 x0){ // Run the simulate e return Vector3 coordinates
        float pi = 3.141592f;
        List<float> x = new List<float>();
        List<float> y = new List<float>();
        x.Add(x0.x);
        y.Add(x0.y);

        for (int i = 0; i < steps - 1; i++){
            float ang = (float)UnityEngine.Random.Range(-pi, pi); // Get random angles (uniform distribuiton)
            float dist = (float)System.Math.Pow(UnityEngine.Random.Range(0.000001f, 1f), (1 / (1 - mu))); // Get distance of steps (levy distribution)            

            float xCoord = (float)System.Math.Cos(ang) * dist;
            // ########################## TO REMAKE THE MAX STEP SIZE LOGIC #####################################
            // animais com perfil mais migratório podem ter tamanhos de passos maiores
            // limit the x coord                 
            if(xCoord > limits["R"])
                xCoord = limits["R"];
            else if(xCoord < limits["L"])
                xCoord = limits["L"];            
            x.Add(xCoord); // Get x axis coordinates 

            float yCoord = (float)System.Math.Sin(ang) * dist;
            // limit the y coord
            if(yCoord > limits["T"])
                yCoord = limits["T"];
            else if(yCoord < limits["B"])
                yCoord = limits["B"];
            y.Add(yCoord); // Get y axis coordinates
        }

        float maxX, minX, maxY, minY;

        var cX = CumulativeSum(x); // Cumulative sum of coordinates (correlated and coherent movements)
        x = cX.Item1; // x coordinates
        if(cX.Item2 < Initializing.valuesX.Item1)
            minX = cX.Item2;
        else minX = Initializing.valuesX.Item1;
        if(cX.Item3 > Initializing.valuesX.Item2)
            maxX = cX.Item3;
        else maxX = Initializing.valuesX.Item2;
        Initializing.valuesX = new Tuple<float, float>(minX, maxX);

        var cY = CumulativeSum(y);
        y = cY.Item1; // y coordinates
        if(cY.Item2 < Initializing.valuesY.Item1)
            minY = cY.Item2;
        else minY = Initializing.valuesY.Item1;
        if(cY.Item3 > Initializing.valuesY.Item2)
            maxY = cY.Item3;
        else maxY = Initializing.valuesY.Item2;
        Initializing.valuesY = new Tuple<float, float>(minY, maxY);        

        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < x.Count; i++){ // Create the Vector3 coordinates
            points.Add(new Vector3(x[i], y[i], -0.5f));
        }

        return points;
    }

    Tuple<List<float>, float, float> CumulativeSum(List<float> list){
        List<float> coord = new List<float>();
        float max = Int32.MinValue, min = Int32.MaxValue;
        coord.Add(list[0]);
        for (int i = 1; i < list.Count; i++){
            float value = coord[i - 1] + list[i];
            coord.Add(value);
            if(value > max)
                max = value;
            if(value < min)
                min = value;
        }
        return new Tuple<List<float>, float, float>(coord, min, max);
    }

    IEnumerator InitializeAnimal(){
        yield return new WaitWhile(() => specie == Specie.UNDEFINED);
        
        switch (specie){
            case Specie.LION:
                InitializeLion();
                break;
            case Specie.ZEBRA:
                InitializeZebra();
                break;
            case Specie.HYENA:
                InitializeHyena();
                break;
            case Specie.BUFFALO:
                InitializeBuffalo();
                break;
        }
    }

    void InitializeLion(){
        System.Random rand = new System.Random();       
        color = Color.yellow;
        spriteRenderer.color = Color.yellow;
        int[] childs = {1,2,3,3,3,3,3,4,4,4};
        
        gender = rand.NextDouble() < 0.5f ? Gender.M : Gender.F;
        feed = Feed.CARNIVORE;
        // isPregnant = //random
        // evaluate sexualMaturity to set isPregnant and add to currentNChilds and add to childrens
        // deathRate = 10;
        timeToIndependence = 18;
        averageSpeed = 4;
        numberChildrens = gender == Gender.F ? childs[rand.Next(0,10)] : 0;
        // deathRateChildrens = 60;
        gestationTime = 110;
        maxSurvivalTime = 216;
        age = rand.Next(0, maxSurvivalTime+1);  // repensar idade com base no número de filhotes do bando
        // set the mom if is a child
        sexualMaturity = gender == Gender.M ? 5 : 4;
        timeBetweenBirths = 24;
        populationDensity = 12;
        // dailyTravelledDistance = 4.5-15; // random
        groupHuntingSuccessRate = 0.30f;
        individualHuntingSuccessRate = 0.17f;
        targets = new List<Specie> {
            Specie.BUFFALO,
            Specie.ZEBRA
        };
    }

    void InitializeHyena(){
        System.Random rand = new System.Random();       
        color = Color.green;
        spriteRenderer.color = Color.green; 
        int[] childs = {1,1,1,2,2,2,2,2,3,4};
        
        gender = rand.NextDouble() < 0.5f ? Gender.M : Gender.F;
        feed = Feed.CARNIVORE;
        // isPregnant = //random
        // evaluate sexualMaturity to set isPregnant and add to currentNChilds and add to childrens
        // deathRate = 14;
        timeToIndependence = 12;        
        averageSpeed = 10;        
        numberChildrens = gender == Gender.F ? childs[rand.Next(0,10)] : 0;
        // deathRateChildrens = 40-50; // random
        gestationTime = 112;
        maxSurvivalTime = 300;
        age = rand.Next(0, maxSurvivalTime+1);    
        // set the mom if is a child    
        sexualMaturity = gender == Gender.M ? 21 : 36;
        timeBetweenBirths = 15;
        populationDensity = 12;
        // dailyTravelledDistance = 27-80 (27-40); // random
        groupHuntingSuccessRate = 0.34f;
        individualHuntingSuccessRate = 0.29f; 
        targets = new List<Specie> {
            Specie.BUFFALO,
            Specie.ZEBRA
        };        
    }

    void InitializeZebra(){
        System.Random rand = new System.Random();       
        color = Color.blue;
        spriteRenderer.color = Color.blue;

        gender = rand.NextDouble() < 0.5f ? Gender.M : Gender.F;
        feed = Feed.HERBIVORE;
        // isPregnant = //random
        // evaluate sexualMaturity to set isPregnant and add to currentNChilds and add to childrens
        // deathRate = 14;        
        averageSpeed = 5;
        numberChildrens = gender == Gender.F ? 1 : 0;
        // deathRateChildrens = 50; // random
        gestationTime = 375;
        maxSurvivalTime = 240;
        age = rand.Next(0, maxSurvivalTime+1);     
        // set the mom if is a child   
        sexualMaturity = gender == Gender.M ? 54 : 24; 
        timeBetweenBirths = 12;
        populationDensity = 700;
        dailyTravelledDistance = 17; 
        targets = new List<Specie>();
    }

    void InitializeBuffalo(){
        System.Random rand = new System.Random();       
        color = Color.black;
        spriteRenderer.color = Color.black;

        gender = rand.NextDouble() < 0.5f ? Gender.M : Gender.F;
        feed = Feed.HERBIVORE;
        // isPregnant = //random
        // evaluate sexualMaturity to set isPregnant and add to currentNChilds and add to childrens
        // deathRate = 14;
        averageSpeed = 4;
        numberChildrens = gender == Gender.F ? rand.NextDouble() < 0.9f ? 1 : 2 : 0; // 1 (90%)
        // deathRateChildrens = 40-50; // random
        gestationTime = 340;
        maxSurvivalTime = 264;
        age = UnityEngine.Random.Range(0, maxSurvivalTime);        
        sexualMaturity = gender == Gender.M ? 54 : 60; 
        timeBetweenBirths = 18;
        populationDensity = 430;
        // dailyTravelledDistance = 17; 1.2-8
        targets = new List<Specie>();
    }

    void OnDrawGizmosSelected (){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere (transform.position, 50f);
    }

}

