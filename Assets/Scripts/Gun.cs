using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.XR.Interaction.Toolkit;
public class Gun : MonoBehaviour
{
    ObjectPooler pooler;
    [SerializeField] float bulletSpeed = 100,bulletSpreadDegrees = 0;
    [SerializeField] int shotsPerFiring = 1;
    [SerializeField] int ammoLeft, maxAmmo = 10;
    [SerializeField] float fireCoolDown = 1,reloadDuration = 1;
    float fireTimer = 0,reloadTimer;
    bool reloading = false;
    [SerializeField] Transform origin;
    Rigidbody body;
    [SerializeField]Vector3 recoilForce;
    XRGrabInteractable interactable;
    // Start is called before the first frame update
    void Start()
    {
        pooler = GetComponent<ObjectPooler>();
        body = GetComponent<Rigidbody>();
        interactable = GetComponent<XRGrabInteractable>();
        ammoLeft = maxAmmo;
    }
    private void Update()
    {
        fireTimer -= Time.deltaTime;
        if (reloading)
        {
            reloadTimer += Time.deltaTime;
            if(reloadTimer >= reloadDuration)
            {
                Reload();
            }
        }
    }

    public void Shoot(ActivateEventArgs eventArgs)
    {
        if(fireTimer <= 0 && !reloading)
        {
            if (ammoLeft > 0)
            {
                for (int i = 0; i < shotsPerFiring; i++)
                {
                    GameObject bulletGo = pooler.SpawnObj();
                    Bullet b = bulletGo.GetComponent<Bullet>();

                    Vector2 randVal = Random.insideUnitCircle * bulletSpreadDegrees;
                    Vector3 dir = Quaternion.Euler(randVal.x, randVal.y, 0) * transform.forward;
                    b.Shoot(dir * bulletSpeed, origin.position);
                }
                ammoLeft--;
                fireTimer = fireCoolDown;
                Debug.Log(interactable.firstInteractorSelecting);
                //.transform.GetComponent<XRController>().SendHapticImpulse(1, 0.1f);
                body.AddForce(transform.up * recoilForce.y + transform.forward * recoilForce.z + transform.right * recoilForce.x);
                //play gun sound / fx
                
            }
            //play no ammo sound / fx
        }
        
        

        
    }
    public void StartReload()
    {
        if (reloading || ammoLeft == maxAmmo)
            return;
        reloading = true;
        reloadTimer = 0;
    }
    void Reload()
    {
        ammoLeft = maxAmmo;
        reloading = false;
    }
}