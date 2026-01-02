using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    void Awake()
    {
        // Eðer sahnede baþka bir müzik çalar varsa, kendini yok et (Çift ses çýkmasýn)
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Yoksa, bu objeyi ana müzik kutusu yap ve sahne deðiþse de silme
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}