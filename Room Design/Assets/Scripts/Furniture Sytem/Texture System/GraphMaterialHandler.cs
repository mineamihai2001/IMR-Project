using Adobe.Substance;
using Adobe.Substance.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GraphMaterialHandler : MonoBehaviour
{
    public SubstanceGraphSO graphSO;
    protected SubstanceRuntimeGraph runtimeSubstance;

    protected SemaphoreSlim materialLock = new SemaphoreSlim(1);

    private void instantiateSubstance()
    {
        gameObject.SetActive(false);
        runtimeSubstance = gameObject.AddComponent<SubstanceRuntimeGraph>();
        runtimeSubstance.GraphSO = graphSO;
        gameObject.SetActive(true);
        //Debug.Log("Instantiated");
    }

    //Awake
    //public void Awake()
    //{
    //    Debug.Log("Awake");
    //}

    //public void Start()
    //{
    //    Debug.Log("Start");
    //}

    public async System.Threading.Tasks.Task<Material> getMaterial(Dictionary<string, float> inputValues)
    {

        await materialLock.WaitAsync();
        try
        {
            //Debug.Log("we start");
            instantiateSubstance();
            foreach (var setting in inputValues)
            {
                runtimeSubstance.SetInputFloat(setting.Key, setting.Value);
            }
            var renderTask = runtimeSubstance.RenderAsync();
            //renderTask.Wait();
            //renderTask.RunSynchronously();
            await renderTask;
            return new Material(runtimeSubstance.DefaulMaterial);
        }
        finally
        {
            Destroy(runtimeSubstance);
            runtimeSubstance = null;
            materialLock.Release();
            //Debug.Log("we finish");
        }
    }
}
