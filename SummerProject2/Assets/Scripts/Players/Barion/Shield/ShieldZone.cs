using UnityEngine;
using System.Collections;

public class ShieldZone : MonoBehaviour 
{
    SphereCollider collider;

    PlayersManager players_manager;

    void Awake()
    {
        collider = GetComponent<SphereCollider>();
        players_manager = GetComponentInParent<PlayersManager>();
    }

	void Start () 
    {
        collider.enabled = false;
	}
	
	
    void OnTriggerEnter(Collider col)
    {
        if(col.tag == Tags.player && col.name != Objects.barion)
        {
           if(col.name == Objects.cosmo) //Cosmo
           {
               CosmoController cosmo = col.gameObject.GetComponent<CosmoController>();
               if(cosmo.GetState() == cosmo.idle_state)
               {
                   cosmo.ChangeStateTo(cosmo.chained_state);
               }
           }

           if (col.name == Objects.nyx) //Nyx
           {
               NyxController nyx = col.gameObject.GetComponent<NyxController>();

               if(nyx.GetState() == nyx.idle_state)
               {
                   nyx.ChangeStateTo(nyx.chained_state);
               }
           }
        }
    }

    /// <summary>
    /// Enables/Disables collision zone.
    /// </summary>
    public void EnableCollision(bool enable)
    {
        collider.enabled = enable;

        if(enable == false)
        {
            players_manager.ShieldDisabled();
        }
    }
}
