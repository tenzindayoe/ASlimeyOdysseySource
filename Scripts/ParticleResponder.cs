using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ParticleResponder : MonoBehaviour{

    private float originalMass ; 
    private Vector3 originalScale ;
    
    public bool shouldReset = false; 
    public float resetFromLastInteractionSecondsNoAnim = 3f;
    public float resetDuration = 5f; 
    public float resetFromLastInteractionSecondsAnimWarning = 3f; 

    

    [Header("Scalers")]
    public Vector3 scaleStepV3 = new Vector3(0.001f,0.001f,0.001f) ;

    
    [Header("Scale Limits ")]
    [Header("Limits are ignored if useScaleLimit is false")]
    public bool useScaleLimits; 
    [Header("These lmits are added to the original scale of the object")]
    public Vector3 expandLimit = new Vector3(1f,1f,1f) ;
    public Vector3 shrinkLimit = new Vector3(0.9f,0.9f,0.9f) ;
    public bool useScaleOffset = false ;
    public Vector3 scaleOffsetDirection = new Vector3(-1f,-1f,-1f) ;


    [Header("Mass Scaler")]
    [Tooltip("How much should the particle mass effect change the mass of the object")]  

    public float maxMass = 100f;

    public int totalNumberOfParticlesToGetMaxMass = 100;
    private int currentNumberOfParticlesAdded = 0;
    public bool addMassOnCollision = true; 

    //soon will be removed
    public float massScaler = 1f ;


    private Vector3 dotweenTargetScale;
    private Tween activeTween; 
    public DOTweenAnimation selfDoTweenAnimation ;
    public IEnumerator resetCoroutine; 
    //
    public HingeJoint hingeJoint; 
 
    public void Start(){
        originalMass = this.GetComponent<Rigidbody>().mass ; 
        originalScale = this.transform.localScale ; 
        selfDoTweenAnimation = gameObject.AddComponent<DOTweenAnimation>(); 
        dotweenTargetScale = originalScale;
        hingeJoint = this.GetComponent<HingeJoint>();
    }




    public void scalerVector3()
    {
        dotweenTargetScale += scaleStepV3;

        if (useScaleLimits)
        {
            dotweenTargetScale.x = Mathf.Clamp(dotweenTargetScale.x, shrinkLimit.x + originalScale.x, expandLimit.x + originalScale.x);
            dotweenTargetScale.y = Mathf.Clamp(dotweenTargetScale.y, shrinkLimit.y + originalScale.y, expandLimit.y + originalScale.y);
            dotweenTargetScale.z = Mathf.Clamp(dotweenTargetScale.z, shrinkLimit.z + originalScale.z, expandLimit.z + originalScale.z);
        }

        activeTween?.Kill(); // Kill the active tween if it exists
        float dynamicDuration = CalculateScaleDuration(transform.localScale, dotweenTargetScale);

        Sequence scaleAndShakeSequence = DOTween.Sequence();
        // scaleAndShakeSequence.Append(transform.DOShakeScale(1f, 0.2f, vibrato: 10, randomness: 90));
        Vector3 originalLocalPos = transform.localPosition;
        scaleAndShakeSequence.Append(
            transform.DOShakeScale(1f, 0.2f, vibrato: 10, randomness: 90)
                .OnUpdate(() => {
                    // Keep position locked each frame of the shake
                    transform.localPosition = originalLocalPos;
                })
        );

        if (useScaleOffset)
        {
            // 1) Figure out how much the scale changed this frame.
            Vector3 scaleDiff = dotweenTargetScale - transform.localScale;

            // 2) Build a local-space offset from that scale difference.
            Vector3 localOffset = new Vector3(
                scaleOffsetDirection.x * scaleDiff.x,
                scaleOffsetDirection.y * scaleDiff.y,
                scaleOffsetDirection.z * scaleDiff.z
            );

            // 3) Convert local offset to the parent’s space (because localPosition
            //    is defined in the parent’s coordinate system).
            Vector3 parentSpaceOffset = transform.localRotation * localOffset;

            // 4) The final local position in parent space:
            Vector3 finalLocalPos = transform.localPosition + parentSpaceOffset;

            // 5) Animate to finalLocalPos with DOTween
            scaleAndShakeSequence.Join(
                transform.DOLocalMove(finalLocalPos, dynamicDuration).SetEase(Ease.OutQuad)
            );
        }

        
        

        // Add the shake animation to run simultaneously
        
        scaleAndShakeSequence.Join(transform.DOScale(dotweenTargetScale, dynamicDuration).SetEase(Ease.OutQuad));

        // Store the active tween
        activeTween = scaleAndShakeSequence;

        // Play the sequence
        scaleAndShakeSequence.Play();
        if(shouldReset){
            startResetCoroutine();
        }

        
        
    }
    private void startResetCoroutine(){
        if(resetCoroutine != null){
            StopCoroutine(resetCoroutine);
        }
        resetCoroutine = resetMassAndScale();
        StartCoroutine(resetCoroutine);
    }

    private float CalculateScaleDuration(Vector3 currentScale, Vector3 targetScale)
    {
        // Calculate the scale difference
        float scaleDifference = Vector3.Distance(currentScale, targetScale);

        // Map the scale difference to a duration (you can adjust the factor for fine-tuning)
        float durationFactor = 2f; // Adjust this value to control scaling speed
        return scaleDifference * durationFactor;
    }

    
 
    public void setMassBasedOnInternalTracker(){
     
        this.GetComponent<Rigidbody>().mass = (currentNumberOfParticlesAdded/totalNumberOfParticlesToGetMaxMass) * maxMass ; 

    }
    public void addMass(){
      
        if(!addMassOnCollision){
            return ; 
        }
        if(currentNumberOfParticlesAdded >= totalNumberOfParticlesToGetMaxMass){

            if (hingeJoint != null){
                hingeJoint.useSpring = false; 
            }
            return ; 
        }
       
        currentNumberOfParticlesAdded+= 1 ; 
        setMassBasedOnInternalTracker() ; 
        //all the animations are in the scalerVector3 function
        
        
        
    }
    
    // public IEnumerator resetMassAndScale()
    // {
    //     // 1) Wait for the specified delay

    //     //lets do tween animations. per second
    //     yield return new WaitForSeconds(resetFromLastInteractionSecondsNoAnim);
    //     for (int i = 0; i < (int)resetFromLastInteractionSecondsAnimWarning; i++)
    //     {
    //         // Example: shake the scale for 1 second with increasing amplitude
    //         float shakeStrength = 0.1f * (i + 1); // scale amplitude grows each second
    //         Vector3 originalLocalPos = transform.localPosition;

    //         Sequence perSecondSequence = DOTween.Sequence();
    //         perSecondSequence.Append(
    //             transform.DOShakeScale(1f, shakeStrength, vibrato: 10, randomness: 90)
    //                 .OnUpdate(() => {
    //                     // Force position to remain fixed
    //                     transform.localPosition = originalLocalPos;
    //                 })
    //         );

    //         // Wait for the tween to complete
    //         yield return perSecondSequence.WaitForCompletion();
    //     }
       

    //     // 2) Lerp the mass back to the original over resetDuration
    //     float elapsedTime = 0f;
    //     float startMass = this.GetComponent<Rigidbody>().mass;
    //     while (elapsedTime < resetDuration)
    //     {
    //         float t = elapsedTime / resetDuration;
    //         float newMass = Mathf.Lerp(startMass, originalMass, t);
    //         this.GetComponent<Rigidbody>().mass = newMass;

    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }
    //     // Make sure mass is exactly set after finishing
    //     this.GetComponent<Rigidbody>().mass = originalMass;

    //     // 3) Reset the scale back to original using the same approach you used before:
    //     //    - optional shake + offset + scale
    //     activeTween?.Kill(); // Kill any existing tween

    //     Vector3 currentScale = transform.localScale;
    //     Vector3 targetScale = originalScale;
    //     float dynamicDuration = CalculateScaleDuration(currentScale, targetScale);

    //     Sequence resetSequence = DOTween.Sequence();
    //     // Add a shake
    //     Vector3 originalLocalPosFinal = transform.localPosition;
    //     resetSequence.Append(
    //         transform.DOShakeScale(1f, 0.2f, vibrato: 10, randomness: 90)
    //             .OnUpdate(() => {
    //                 transform.localPosition = originalLocalPosFinal;
    //             })
    //     );
    //     if (useScaleOffset)
    //     {
    //         // Use the same offset logic
    //         Vector3 scaleDiff = targetScale - currentScale;
    //         Vector3 localOffset = new Vector3(
    //             scaleOffsetDirection.x * scaleDiff.x,
    //             scaleOffsetDirection.y * scaleDiff.y,
    //             scaleOffsetDirection.z * scaleDiff.z
    //         );
    //         Vector3 parentSpaceOffset = transform.localRotation * localOffset;
    //         Vector3 finalLocalPos = transform.localPosition + parentSpaceOffset;

    //         resetSequence.Join(
    //             transform.DOLocalMove(finalLocalPos, dynamicDuration).SetEase(Ease.OutQuad)
    //         );
    //     }

    //     // Scale back to original
    //     resetSequence.Join(transform.DOScale(targetScale, dynamicDuration).SetEase(Ease.OutQuad));

    //     // Store the active tween and play
    //     activeTween = resetSequence;

    //     resetSequence.Play();
    //     if(shouldReset){
    //         startResetCoroutine();
    //     }
    // }

    public IEnumerator resetMassAndScale()
    {
        // (1) Wait with no animation
        yield return new WaitForSeconds(resetFromLastInteractionSecondsNoAnim);

        // (2) Small "warning shakes" each second
        for (int i = 0; i < (int)resetFromLastInteractionSecondsAnimWarning; i++)
        {
            float shakeStrength = 0.1f * (i + 1);
            Vector3 originalLocalPos = transform.localPosition;

            Sequence perSecondSequence = DOTween.Sequence();
            perSecondSequence.Append(
                transform.DOShakeScale(1f, shakeStrength, vibrato: 10, randomness: 90)
                    .OnUpdate(() =>
                    {
                        // Lock position if you want NO drift
                        transform.localPosition = originalLocalPos;
                    })
            );
            yield return perSecondSequence.WaitForCompletion();
        }

        // Kill any existing tween before final reset
        activeTween?.Kill();

        // (3) Final reset of both scale AND mass in parallel
        Vector3 currentScale = transform.localScale;
        Vector3 targetScale = originalScale;
        float dynamicDuration = CalculateScaleDuration(currentScale, targetScale);

        // Create a brand-new sequence
        Sequence resetSequence = DOTween.Sequence();

        // Optional small shake at the start
        Vector3 originalLocalPosFinal = transform.localPosition;
        resetSequence.Append(
            transform.DOShakeScale(0.5f, 0.2f, vibrato: 10, randomness: 90)
                .OnUpdate(() =>
                {
                    transform.localPosition = originalLocalPosFinal;
                })
        );

        // Now that we appended a short shake, let's do the parallel transitions:
        // (a) Scale to original
        resetSequence.Join(
            transform.DOScale(targetScale, dynamicDuration).SetEase(Ease.OutQuad)
        );

        // (b) Mass to original (DOTween.To -> custom getter/setter)
        float currentMass = GetComponent<Rigidbody>().mass;
        resetSequence.Join(
            DOTween.To(
                () => currentMass,
                x => { GetComponent<Rigidbody>().mass = x; },
                originalMass,
                dynamicDuration
            )
        );

        // (c) If you use scale offset, we can do it too
        if (useScaleOffset)
        {
            Vector3 scaleDiff = targetScale - currentScale;
            Vector3 localOffset = new Vector3(
                scaleOffsetDirection.x * scaleDiff.x,
                scaleOffsetDirection.y * scaleDiff.y,
                scaleOffsetDirection.z * scaleDiff.z
            );
            Vector3 parentSpaceOffset = transform.localRotation * localOffset;
            Vector3 finalLocalPos = transform.localPosition + parentSpaceOffset;

            // Move it in parallel as well
            resetSequence.Join(
                transform.DOLocalMove(finalLocalPos, dynamicDuration).SetEase(Ease.OutQuad)
            );
        }

        // Store and play
        activeTween = resetSequence;
        resetSequence.Play();
    }
    


    private void OnParticleCollision(GameObject other)
{
    
    // Add your response logic here (e.g., scaling, mass increment, etc.)
}
}