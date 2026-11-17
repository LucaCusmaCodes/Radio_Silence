using UnityEngine;

public class SidewalkLines : MonoBehaviour
{
    public GameObject line;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int dist = -5;
        for(int i = 0; i < 140;i++)
        {
            if(line.transform.position.z < 40 && (line.transform.position.x+(dist*i)>-162 || line.transform.position.x+(dist*i)<-182))
            {
                Instantiate(line, new Vector3(line.transform.position.x+(dist*i), line.transform.position.y, line.transform.position.z), Quaternion.identity);
            }
            if(line.transform.position.z >60 && line.transform.position.z < 62)
            {
                Instantiate(line, new Vector3(line.transform.position.x+(dist*i), line.transform.position.y, line.transform.position.z), Quaternion.identity);
            }
            if(line.transform.position.z < 136 && line.transform.position.z > 130 && (line.transform.position.x+(dist*i)>-162 || line.transform.position.x+(dist*i)<-182))
            {
                Instantiate(line, new Vector3(line.transform.position.x+(dist*i), line.transform.position.y, line.transform.position.z), Quaternion.identity);
            }
            if(line.transform.position.z >158 && line.transform.position.z < 160)
            {
                Instantiate(line, new Vector3(line.transform.position.x+(dist*i), line.transform.position.y, line.transform.position.z), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
