using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



    /// <summary>
    /// This component is used to pull grab items toward it, and then reset it's position when not being grabbed
    /// </summary>
    public class HandleHelper : MonoBehaviour {

        public Rigidbody ParentRigid;

        /// <summary>
        /// The Transform that is following us
        /// </summary>
        public Transform HandleTransform;

        //Grabbable thisGrab;
        XRGrabInteractable thisGrab; 
        Rigidbody rb;
        bool didRelease = false;
        Collider col;

        void Start() {
        //thisGrab = GetComponent<Grabbable>();
        //thisGrab.CanBeSnappedToSnapZone = false;

        thisGrab = GetComponent<XRGrabInteractable>(); 
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();

            // Handle and parent shouldn't collide with each other
            if(col != null && ParentRigid != null && ParentRigid.GetComponent<Collider>() != null) {
                Physics.IgnoreCollision(ParentRigid.GetComponent<Collider>(), col, true);
            }
        }

        Vector3 lastAngularVelocity;

        void FixedUpdate() {

        //if(!thisGrab.BeingHeld) {
        if (!thisGrab.isSelected) { 

                if(!didRelease) {

                    //col.enabled = false;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    transform.localScale = Vector3.one;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;

                    if (ParentRigid) {
                        // ParentRigid.velocity = Vector3.zero;
                        // ParentRigid.angularVelocity = Vector3.zero;
                        ParentRigid.angularVelocity = lastAngularVelocity * 20;
                    }
                    col.enabled = true;
                    StartCoroutine(doRelease());

                    didRelease = true;
                }
            }
            else {

                // Object is being held, need to fire release
                didRelease = false;

            // Check Break Distance since we are always holding the helper
            //if (thisGrab.BreakDistance > 0 && Vector3.Distance(transform.position, HandleTransform.position) > thisGrab.BreakDistance) {
            //    thisGrab.DropItem(false, false);
            //}

            if ( Vector3.Distance(transform.position, HandleTransform.position) > 0.4f)
            {
                // FIXME
                //thisGrab.DropItem(false, false);

                // hhttp://snapandplug.com/xr-input-toolkit-2020-faq/#FAQ:-Can-I-force-a-hand/controller-to-drop-held-items?
                //thisGrab.allowSelect = false; 
                //thisGrab.Detach(); 

            }

            lastAngularVelocity = rb.angularVelocity * -1;
            }
        }

        private void OnCollisionEnter(Collision collision) {
            // Handle Helper Ignores All Collisions
            Physics.IgnoreCollision(col, collision.collider, true);
        }

        IEnumerator doRelease() {

            //for(int x = 0; x < 120; x++) {
            //    ParentRigid.angularVelocity = new Vector3(10, 1000f, 50);
            //    yield return new WaitForFixedUpdate();
            //}

            yield return new WaitForSeconds(1f);

            col.enabled = true;
        }
    }


