using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{

    public Sprite circle;
    public LineRenderer lineRenderer;
    public List<Vector3> points;
    public int stepCount = 1;
    public int timeScaleInDays;
    public int coordTimeIntervalInMinutes;
    public Dictionary<string, float> limits = new Dictionary<string, float>(){
        {"L", -100f},
        {"R", 100f},
        {"T", 100f},
        {"B", -100f}
    };

    // Start is called before the first frame update
    void Start(){
        float mu = 2.5f;
        float x = Random.Range(limits["L"], limits["R"]);
        float y = Random.Range(limits["B"], limits["T"]);     

        int steps = (int)(60 / coordTimeIntervalInMinutes) * 24 * timeScaleInDays; // times per hour * 24 hours * days

        points = simm_levy(steps, mu, new Vector3(x, y, -0.5f));

        lineRenderer = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();
        LineRenderer lineRendererC = Instantiate(lineRenderer);
        lineRendererC.positionCount = steps;

        circle = GetComponent<SpriteRenderer>().sprite; // set the sprite

        transform.position = points[0]; // set initial position

        int i = 0;
        foreach (var item in points){ // Create points
            GameObject go = new GameObject();
            go.transform.position = item;
            go.transform.localScale = new Vector3(2, 2, 0);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.color = Color.red;
            sr.sprite = circle;

            lineRendererC.SetPosition(i, item);
            i++;
        }
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
            transform.position = Vector3.MoveTowards(transform.position, points[stepCount], step); // Move the individual gradually

            if(Vector3.Distance(transform.position, points[stepCount]) == 0f)
                stepCount++;      
        }                  
    }

    List<Vector3> simm_levy(int steps, float mu, Vector3 x0){ // Run the simulate e return Vector3 coordinates
        float pi = 3.141592f;
        List<float> x = new List<float>();
        List<float> y = new List<float>();
        x.Add(x0.x);
        y.Add(x0.y);

        for (int i = 0; i < steps - 1; i++){
            float ang = (float)Random.Range(-pi, pi); // Get random angles (uniform distribuiton)
            float dist = (float)System.Math.Pow(Random.Range(0f, 1.001f), (1 / (1 - mu))); // Get distance of steps (levy distribution)
            x.Add((float)System.Math.Cos(ang) * dist); // Get x axis coordinates
            y.Add((float)System.Math.Sin(ang) * dist); // Get y axis coordinates
        }

        x = cumulative_sum(x); // Cumulative sum of coordinates (correlated and coherent movements)
        y = cumulative_sum(y);

        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < x.Count; i++){ // Create the Vector3 coordinates
            points.Add(new Vector3(x[i], y[i], -0.5f));
            Debug.Log(i + ": " + x[i] + "; " + y[i]);
        }

        return points;
    }

    List<float> cumulative_sum(List<float> list){
        List<float> coord = new List<float>();
        coord.Add(list[0]);
        for (int i = 1; i < list.Count; i++){
            coord.Add(coord[i - 1] + list[i]);
        }
        return coord;
    }
}
