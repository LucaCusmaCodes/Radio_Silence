using UnityEngine;
using System.Collections.Generic;

public class WaterLeaking : MonoBehaviour
{
    int Rate = 0;
    public GameObject LeakLocation;
    public GameObject WaterDroplet;
    GameObject droplet;
    List<GameObject> Droplets = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WaterDroplet.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
        GameObject droplet1 = Instantiate(WaterDroplet, LeakLocation.transform.position, LeakLocation.transform.rotation);
        GameObject droplet2 = Instantiate(WaterDroplet, LeakLocation.transform.position, LeakLocation.transform.rotation);
        GameObject droplet3 = Instantiate(WaterDroplet, LeakLocation.transform.position, LeakLocation.transform.rotation);
        GameObject droplet4 = Instantiate(WaterDroplet, LeakLocation.transform.position, LeakLocation.transform.rotation);
        GameObject droplet5 = Instantiate(WaterDroplet, LeakLocation.transform.position, LeakLocation.transform.rotation);
        GameObject droplet6 = Instantiate(WaterDroplet, LeakLocation.transform.position, LeakLocation.transform.rotation);
        GameObject droplet7 = Instantiate(WaterDroplet, LeakLocation.transform.position, LeakLocation.transform.rotation);
        GameObject droplet8 = Instantiate(WaterDroplet, LeakLocation.transform.position, LeakLocation.transform.rotation);
        Droplets.Add(droplet1);
        Droplets.Add(droplet2);
        Droplets.Add(droplet3);
        Droplets.Add(droplet4);
        Droplets.Add(droplet5);
        Droplets.Add(droplet6);
        Droplets.Add(droplet7);
        Droplets.Add(droplet8);
    }

    // Update is called once per frame
    void Update()
    {
        Rate++;
        if (Rate == 40)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.05f, 0.05f), 0f, Random.Range(-0.05f, 0.05f));
            GameObject droplet1 = Instantiate(WaterDroplet, LeakLocation.transform.position + randomOffset, LeakLocation.transform.rotation);
            Droplets.Add(droplet1);
            Destroy(Droplets[0]);
            Droplets.RemoveAt(0);
            Rate = 0;
        }
    }
}


