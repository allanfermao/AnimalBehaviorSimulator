using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public Sprite circle;
    // Start is called before the first frame update
    void Start(){
        List<Vector3> points = simm_levy(50, 2.5f, new Vector3(0,0,-0.5f));
        foreach (var item in points){ // Create points
            GameObject go = new GameObject();
            go.transform.position = item;
            go.transform.localScale = new Vector3(2,2,0);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.color = Color.red;
            sr.sprite = circle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<Vector3> simm_levy(int steps, float mu, Vector3 x0){
        float pi = 3.141592f;
        List<float> x = new List<float>();
        List<float> y = new List<float>();
        x.Add(0);
        y.Add(0);

        for (int i = 0; i < steps-1; i++){
            float ang = (float)Random.Range(-pi, pi); // Get random angles (uniform distribuiton)
            float dist = (float)System.Math.Pow(Random.Range(0f, 1.001f), (1/(1-mu))); // Get distance of steps (levy distribution)
            x.Add((float)System.Math.Cos(ang) * dist); // Get x axis coordinates
            y.Add((float)System.Math.Sin(ang) * dist); // Get y axis coordinates
        }

        x = cumulative_sum(x); // Cumulative sum of coordinates (correlated and coherent movements)
        y = cumulative_sum(y);

        List<Vector3> points = new List<Vector3>();
        for(int i=0; i < x.Count; i++){ // Create the Vector3 coordinates
            points.Add(new Vector3(x[i], y[i], -0.5f));
            Debug.Log(x[i] + "; " + y[i]);
        }    

        return points;
    }

    List<float> cumulative_sum(List<float> list){
        List<float> coord = new List<float>();
        coord.Add(0);
        for(int i=1; i < list.Count; i++){
            coord.Add(coord[i-1] + list[i]);
        }
        return coord;
    }
}
