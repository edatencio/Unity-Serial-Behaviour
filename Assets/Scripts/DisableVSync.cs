using UnityEngine;

public class DisableVSync : MonoBehaviour
{
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
    }
}
