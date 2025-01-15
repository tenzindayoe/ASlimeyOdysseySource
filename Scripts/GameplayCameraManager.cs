using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public enum CameraMode
{
    ThirdPerson,
    Aim,
    // _2DCamera
}
public class GameplayCameraManager : MonoBehaviour
{
    public CinemachineCamera thirdPersonCamera;
    public CinemachineCamera aimCamera; 
    public CameraMode currentCameraMode = CameraMode.ThirdPerson;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setCameraMode(currentCameraMode);
    }

    public void setCameraMode(CameraMode mode)
    {
        switch (mode)
        {
            case CameraMode.ThirdPerson:
                thirdPersonCamera.Priority = 1;
                
                // wait for the recentering to complete and then disable i
                thirdPersonCamera.gameObject.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Recentering.Enabled = true;
                StartCoroutine(recenterThirdPersonCamera());
                aimCamera.Priority = 0;
                // _2DCamera.Priority = 0;
                break;
            case CameraMode.Aim:
                thirdPersonCamera.Priority = 0;
                aimCamera.Priority = 1;
                // _2DCamera.Priority = 0;
                break;
            // case CameraMode._2DCamera:
            //     thirdPersonCamera.Priority = 0;
            //     aimCamera.Priority = 0;
            //     _2DCamera.Priority = 1;
            //     break;
        }
    }

    public void setBothCamerasPriority(int priority)
    {
        thirdPersonCamera.Priority = priority;
        aimCamera.Priority = priority;
        // _2DCamera.Priority = priority;
    }

    public IEnumerator recenterThirdPersonCamera()
    {
        thirdPersonCamera.gameObject.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Recentering.Enabled = true;
        // wait for the recenter and then disable it
        float waitTime = thirdPersonCamera.gameObject.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Recentering.Wait;
        float recenteringTime = thirdPersonCamera.gameObject.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Recentering.Time;  
        float timeToWait = waitTime + recenteringTime;
        yield return new WaitForSeconds(timeToWait);
        thirdPersonCamera.gameObject.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Recentering.Enabled = false;
    }
   
   public IEnumerator instantThirdPersonRecenter(){
        float waitTime = thirdPersonCamera.gameObject.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Recentering.Wait;
        float recenteringTime = thirdPersonCamera.gameObject.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Recentering.Time; 
        
        thirdPersonCamera.gameObject.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Recentering.Wait = 0;
        thirdPersonCamera.gameObject.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Recentering.Time = 0;
         thirdPersonCamera.gameObject.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Recentering.Enabled = true;
        yield return new WaitForSeconds(0.1f);
         thirdPersonCamera.gameObject.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Recentering.Enabled = false;
        thirdPersonCamera.gameObject.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Recentering.Wait = waitTime;
        thirdPersonCamera.gameObject.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Recentering.Time = recenteringTime;
        
   }
}
