using System;
using UnityEngine;
using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class WaterGunController : MonoBehaviour
{
   
    [SerializeField] bool _update;
    [SerializeField] Transform _CreationPoint;
    [SerializeField] WaterBall WaterBallPrefab;

    public GameObject gunObject; 
    public bool waterReady = false; 

    public Transform target; 
    public int maxCapacity = 5; 
    public int usedCapacity = 0 ; 

    public void enableGunObject(){
        gunObject.SetActive(true); 
    }
    public void disableGunObject(){
        gunObject.SetActive(false); 
    }
    public void chargeWater(){
        // do not use this. 
        StartCoroutine(charge());
    }


    public IEnumerator reloadCoroutine = null; 
    WaterBall waterBall;
    public void startFiring(){
        if(waterReady && waterBall != null){
            waterBall.Throw(target.position);
            GameAudioManager.Instance.PlayAudio(GameAudioManager.Instance.waterGunFireSound, GameAudioManager.Instance.sfxMixerGroup, true, _CreationPoint.position);
            waterReady = false;
            StartCoroutine(charge());
        }else{
            if(usedCapacity == maxCapacity){
                // play sound here
            }
        }
    }
    public IEnumerator charge(){
        if(usedCapacity < maxCapacity){
            GameAudioManager.Instance.PlayAudio(GameAudioManager.Instance.waterGunChargeSound, GameAudioManager.Instance.sfxMixerGroup, true, _CreationPoint.position);
            CreateWaterBall();
            yield return new WaitForSeconds(GameAudioManager.Instance.waterGunChargeSound.length);
            usedCapacity++;
            waterReady = true; 
        }else{
            // play sound for no bullets left
        }
    }

    public void startReload(){
        if(reloadCoroutine != null){
            return ; 
        }else{
            reloadCoroutine = ReloadCoroutine();
            StartCoroutine(reloadCoroutine);
        }
    }

    public void stopReload(){
        if(reloadCoroutine != null){
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
    }
    public IEnumerator ReloadCoroutine(){

        float lerpDuration = 3.0f; // Total duration of the reload
        float elapsedTime = 0f; 
        int startingCapacity = usedCapacity;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / lerpDuration; // Normalized time [0, 1]
            usedCapacity = Mathf.RoundToInt(Mathf.Lerp(startingCapacity, 0, t)); // Lerp and round to an integer
            yield return null; // Wait for the next frame
        }
        usedCapacity = 0 ; 
        // lerp usedWatercapcity to 0 
    }
   
  

    public void destroyCharged(){
        if(waterBall != null){
            //StartCoroutine(lerpShrinkAndDestroy());
            Destroy(waterBall.gameObject);
            waterBall = null ; 
            
            waterReady = false;
        }
    }
    public IEnumerator lerpShrinkAndDestroy(){
        float lerp = 0;
        Vector3 startPos = waterBall.transform.localScale;
        // while (lerp < 1)
        // {
        //     waterBall.transform.localScale = Vector3.Lerp(startPos, Vector3.zero, lerp);
        //     lerp += Time.deltaTime * 1.0f;
        //     yield return null;
        // }
        yield return null; 
        
    }
    void Start(){
        
        //
    }

    public void increaseMaxWaterCapacity(int delta){
        
        SaveStateUtils.GetCurrentSaveState().currentMaxWaterLevel += delta;
        maxCapacity = SaveStateUtils.GetCurrentSaveState().currentMaxWaterLevel;
        
    }
    private void Update()
    {

        updateChargedWaterPosition();
        
        // if (!_update)
        // {
        //     return;
        // }



        // if (Input.GetMouseButtonDown(0))
        // {
        //     if (WaterBallCreated())
        //     {
        //         CreateWaterBall();
        //     }
        //     else
        //     {
        //         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //         RaycastHit hit;
        //         if (Physics.Raycast(ray, out hit))
        //         {
        //             if (waterBall != null)
        //             {
        //                 ThrowWaterBall(hit.point);
        //             }
        //         }
        //     }
        // }
    }

    private void updateChargedWaterPosition(){
        if(waterBall != null ){// water ready check was also here 
            waterBall.transform.position = _CreationPoint.position;
            waterBall.transform.rotation = _CreationPoint.rotation;
        }
    }
    public bool WaterBallCreated()
    {
        return waterBall != null;
    }
    public void CreateWaterBall()
    {
        waterBall = Instantiate(WaterBallPrefab, _CreationPoint.position, Quaternion.identity);
    }

    public void ThrowWaterBall(Vector3 pos)
    {
        waterBall.Throw(pos);
    }
}
