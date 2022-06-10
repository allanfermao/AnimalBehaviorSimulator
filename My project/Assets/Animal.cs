using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Animal : MonoBehaviour{    

    public int stepCount = 1;
    public int timeScaleInDays;
    public List<Vector3> points;
    public LineRenderer lineRenderer;
    public SpriteRenderer spriteRenderer;
    public int coordTimeIntervalInMinutes;    
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
        BUFFALO
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
    public int age; // months    
    public int stamina = 100;
    public Specie specie; // enum
    public bool interacting = false;
    public float travelledDistance = 0;
    public int socialStatus; // (current) enum

    // Specie Attributes
    public Feed feed;
    public int deathRate;
    public int averageSpeed;
    public int numberChildrens; 
    public int deathRateChildrens;
    public int gestationTime; // days
    public int survivalTime; // months
    public int sexualMaturity; // months
    public int timeBetweenBirths; // months
    public int populationDensity; // per 100km²
    public float dailyTravelledDistance; // km/24h
    public int groupHuntingSuccessRate; // per cent
    public int individualHuntingSuccessRate; // per cent
    public List<Specie> targets;

    // Start is called before the first frame update
    void Start(){
        float mu = 2.5f;
        float x = UnityEngine.Random.Range(limits["L"], limits["R"]);
        float y = UnityEngine.Random.Range(limits["B"], limits["T"]);     

        int steps = (int)(60 / coordTimeIntervalInMinutes) * 24 * timeScaleInDays; // times per hour * 24 hours * days

        points = simm_levy(steps, mu, new Vector3(x, y, -0.5f));

        spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite circle = spriteRenderer.sprite; // set the sprite

        transform.position = points[0]; // set initial position

        // Render the walks

        // lineRenderer = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();
        // LineRenderer lineRendererC = Instantiate(lineRenderer);
        // lineRendererC.positionCount = steps;
        
        // int i = 0;
        // foreach (var item in points){ // Create points
        //     GameObject go = new GameObject();
        //     go.transform.position = item;
        //     go.transform.localScale = new Vector3(2, 2, 0);
        //     SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        //     sr.color = Color.red;
        //     sr.sprite = circle;

        //     lineRendererC.SetPosition(i, item);
        //     i++;
        // }
    }

    // Update is called once per frame
    void Update(){

    }

    void FixedUpdate(){
        float speed = 50f;
        float step = speed * Time.fixedDeltaTime; // Set the step size

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
            foreach(var animal in animalsInRange){
                // GameObject go = animal.gameObject;
                if(animal.name != name){
                    // print(name + " encontrou " + animal.name);
                    interacting = true;                
                    break;
                }
            }    
            if(interacting){
                spriteRenderer.color = Color.Lerp(Color.blue, Color.red, Mathf.PingPong(Time.time*2, 1));
            }    
            if(animalsInRange.Length == 1){ // just the own element
                spriteRenderer.color = Color.blue;
                interacting = false;
            }

            Array.Clear(animalsInRange, 0, animalsInRange.Length);
        }    
        else print("Traveled distance by " + name + ": " + travelledDistance);
    }

    List<Vector3> simm_levy(int steps, float mu, Vector3 x0){ // Run the simulate e return Vector3 coordinates
        float pi = 3.141592f;
        List<float> x = new List<float>();
        List<float> y = new List<float>();
        x.Add(x0.x);
        y.Add(x0.y);

        for (int i = 0; i < steps - 1; i++){
            float ang = (float)UnityEngine.Random.Range(-pi, pi); // Get random angles (uniform distribuiton)
            float dist = (float)System.Math.Pow(UnityEngine.Random.Range(0f, 1.001f), (1 / (1 - mu))); // Get distance of steps (levy distribution)            

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

        var cX = cumulative_sum(x); // Cumulative sum of coordinates (correlated and coherent movements)
        x = cX.Item1; // x coordinates
        if(cX.Item2 < Initializing.valuesX.Item1)
            minX = cX.Item2;
        else minX = Initializing.valuesX.Item1;
        if(cX.Item3 > Initializing.valuesX.Item2)
            maxX = cX.Item3;
        else maxX = Initializing.valuesX.Item2;
        Initializing.valuesX = new Tuple<float, float>(minX, maxX);

        var cY = cumulative_sum(y);
        y = cY.Item1; // y coordinates
        if(cY.Item2 < Initializing.valuesY.Item1)
            minY = cY.Item2;
        else minY = Initializing.valuesY.Item1;
        if(cY.Item3 > Initializing.valuesY.Item2)
            maxY = cY.Item3;
        else maxY = Initializing.valuesY.Item2;
        Initializing.valuesY = new Tuple<float, float>(minY, maxY);

        // print("valuesX: " + Initializing.valuesX.Item1 + " " + Initializing.valuesX.Item2);
        // print("valuesY: " + Initializing.valuesY.Item1 + " " + Initializing.valuesY.Item2);

        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < x.Count; i++){ // Create the Vector3 coordinates
            points.Add(new Vector3(x[i], y[i], -0.5f));
            // Debug.Log(i + ": " + x[i] + "; " + y[i]);
        }

        return points;
    }

    Tuple<List<float>, float, float> cumulative_sum(List<float> list){
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

    void InitializeLion(){
        // age; // random        
        specie = Specie.LION;
        // gender; // random
        // socialStatus; // random

        // Specie Attributes
        feed = Feed.CARNIVORE;
        deathRate = 10;
        averageSpeed = 4;
        // numberChildrens; // random
        deathRateChildrens = 60;
        gestationTime = 110;
        survivalTime = 168;
        // sexualMaturity; // dependent of gender
        timeBetweenBirths = 24;
        populationDensity = 12;
        // dailyTravelledDistance; // random
        groupHuntingSuccessRate = 30;
        individualHuntingSuccessRate = 17;
        targets = new List<Specie> {
            Specie.BUFFALO,
            Specie.ZEBRA
        };
    }

    void InitializeHyena(){

    }

    void InitializeZebra(){

    }

    void InitializeBuffalo(){

    }

    void OnDrawGizmosSelected (){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere (transform.position, 50f);
    }

    IEnumerator ChangeColor(float time){
        yield return new WaitForSeconds(time);

        spriteRenderer.color = spriteRenderer.color == Color.blue ? Color.red : Color.blue;
    }
}

