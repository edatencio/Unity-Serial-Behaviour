using UnityEngine;

public class Timer
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     private float startTime;
     private bool ranOnce;

     /*************************************************************************************************
     *** Properties
     *************************************************************************************************/
     public bool isRunning
     {
          get;
          private set;
     }

     public float ElapsedSeconds
     {
          get
          {
               if (ranOnce) return Time.time - startTime;
               else return 0f;
          }
     }

     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     public void Start()
     {
          ranOnce = true;
          isRunning = true;
          startTime = Time.time;
     }

     public void Stop()
     {
          isRunning = false;
     }
}
