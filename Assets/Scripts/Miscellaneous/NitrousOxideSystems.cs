using UnityEngine;

public delegate void NitrousStageHandler(int change);

public enum NOS
{
    Stage1 = 0,
    Stage2 = 1,
    Stage3 = 2 
}

public class NitrousOxideSystems : MonoBehaviour {

    #region variables
    public static event NitrousStageHandler NitrousStageChanged;

    [SerializeField]
    private NOS requiredStage = NOS.Stage1;

    [SerializeField]
    private NOS changeToStage = NOS.Stage2;
    #endregion

    #region methods
    void OnNitrousStageChanged()
    {
        if (NitrousStageChanged != null)
        {
            NitrousStageChanged(changeToStage - requiredStage);
        }
    }

    public void IsGettingSuckedIn(NOS stage)
    {
        if(stage == requiredStage)
        {
            OnNitrousStageChanged();
        }
    }

    void OnDestroy(){
        NitrousStageChanged = null;
    }
    #endregion
}
