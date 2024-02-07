using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTest : MonoBehaviour
{
    private GameObject laserAria;
    private PlayerLaser playerLaser;

    // Start is called before the first frame update
    void Start()
    {
        laserAria = this.transform.Find("PlayerRotate").Find("LaserAria").gameObject;
        playerLaser = new PlayerLaser();
        playerLaser.Shoot_Setting(laserAria.transform);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TestRay();
    }



    private void TestRay()
    {
        /*RaycastHit2D hit = Physics2D.Raycast(laserAria.transform.position, transform.up, 15, LayerMask.GetMask("Wall"));
        
        if(hit.collider != null)
        {
            Debug.Log(hit.collider.name);
        }

        if(hit.distance <= 0)
            laserAria.transform.localScale = new Vector3(1, 15, 1);
        else
            laserAria.transform.localScale = new Vector3(1, hit.distance, 1);*/

        laserAria.transform.localScale = playerLaser.Shoot_Laser();
    }

    private void Control_Aria_Laser_Active(bool active)
    {


    }

}
