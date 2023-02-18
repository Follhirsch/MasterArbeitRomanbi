using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.UIElements;

public class CalibrateHumanSize : MonoBehaviour
{
    public float scale = 0.1f;
    public float legMip = 1.14f;
    public float armMip = 1f;

    public GameObject neck;
    public GameObject hip;
    public GameObject rightHand;

    public VRIK vrikSolver;
    // Start is called before the first frame update
    void Start()
    {
        scale = 0.1f;
        legMip = 1.14f;
        armMip = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            CalibrateModel();
        }
        
        if (Input.GetKeyDown("v"))
        {
            ScaleModel(scale);
        }
        if (Input.GetKeyDown("b"))
        {
            Debug.Log(rightHand.transform.position.x);
        }


        if (vrikSolver is null)
        {
            vrikSolver = gameObject.GetComponent<VRIK>();
        }


    }

    void CalibrateModel()
    {
        scale = calculateScale();
        legMip = calculateLegMip(scale);
        armMip = calculateArmMip(scale);
        
        applyCalibration(scale,legMip,armMip);
    }

    public void applyCalibration(float scaleIn, float legMipIn, float armMipIn)
    {
        ScaleModel(scaleIn);
        applyLegMip(legMipIn);
        applyArmMip(armMipIn);
    }
    
    

    float calculateScale()
    {
        float deltaHH = neck.transform.position.y - hip.transform.position.y;
        return 0.2397f * deltaHH - 0.0319f;
    }
    
    float calculateLegMip(float scaleIn)
    {
        float hipHight = hip.transform.position.y;
        float mlp = (hipHight *1.0567f+0.0151f)*0.1f/scaleIn; //0.1f is the scale in witch the calibratino was performed
        Debug.Log(mlp);
        return mlp;
    }

    float calculateArmMip(float scaleIn)
    {
        return 1f;
    }

    void ScaleModel(float newScale)
    {
        Vector3 scaleVector = new Vector3(newScale, newScale, newScale);
        gameObject.transform.localScale = scaleVector;
    }

    void applyLegMip(float newLegMip)
    {
        Debug.Log(vrikSolver);
        vrikSolver.solver.rightLeg.legLengthMlp = newLegMip;
        vrikSolver.solver.leftLeg.legLengthMlp = newLegMip;
    }

    void applyArmMip(float newArmMip)
    {
        vrikSolver.solver.leftArm.armLengthMlp = newArmMip;
        vrikSolver.solver.rightArm.armLengthMlp = newArmMip;
    }
    
    
    
}
