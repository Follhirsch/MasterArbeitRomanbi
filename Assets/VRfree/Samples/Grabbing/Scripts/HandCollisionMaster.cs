/*
 * This class is used to keep track of all collisions registered by HandCollision scripts.
 * All collisions to the same rigidbody are collected in a seperate CollisionItemList.
 * Based on the layer of the corresponding GameObject a different CollisionEvent from the collisionHandlers is called each FixedUpdate.
 * These events can be used for example for grabbing objects (=fixing to the hand), moving them in a specific position once grabbed, or execute actions on specific gestures (e.g. gun shooting).
 * 
 * Created 22.08.2016 by Hagen Seifert
 */

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace VRfreePluginUnity {
    [ScriptOrder(50)]
    [RequireComponent(typeof(HandController))]
    public class HandCollisionMaster : MonoBehaviour {
        [HideInInspector]
        public HandController handController;

        public bool drawDebugRays = false;

        public List<ContactItemList> collisionLists = new List<ContactItemList>();

        /* to observe size of list in editor */
        public int numCollisions = 0;

        //public FingerObjectAvoidingBehaviour fingerObjectAvoidingBehaviour;

        private void Awake() {
            handController = GetComponent<HandController>();
        }

        // Use this for initialization
        void Start() {
            //fingerObjectAvoidingBehaviour = GetComponent<FingerObjectAvoidingBehaviour>();
            handController.init();

            // add HandCollisionSlaves
            if(handController.handTransforms.wristTransform) {
                HandCollisionSlave slave = handController.handTransforms.wristTransform.gameObject.AddComponent<HandCollisionSlave>();
                slave.handCollisionMaster = this;
                slave.finger = -2;
                slave.phalanx = -2;
            }
            if(handController.handTransforms.handTransform) {
                HandCollisionSlave slave = handController.handTransforms.handTransform.gameObject.AddComponent<HandCollisionSlave>();
                slave.handCollisionMaster = this;
                slave.finger = -1;
                slave.phalanx = -1;
            }
            for(int f = 0; f < 5; f++) {
                for(int i = 0; i < 3; i++) {
                    if(handController.handTransforms.getFingerTransform(f, i)) {
                        HandCollisionSlave slave = handController.handTransforms.getFingerTransform(f, i).gameObject.AddComponent<HandCollisionSlave>();
                        slave.handCollisionMaster = this;
                        slave.finger = f;
                        slave.phalanx = i;
                    }
                }
            }
        }

        public int getNumGrabbedItems() {
            return collisionLists.FindAll(x => x.isGrabbed == true).Count;
        }

        public bool hasGrabbedItem() {
            return collisionLists.Find(x => x.isGrabbed == true) != null;
        }

        // All physics calculations and updates occur immediately after FixedUpdate.
        void FixedUpdate() {
            numCollisions = 0;

            collisionLists.RemoveAll(list => list.collisionRigidbody == null);

            // iterate through CollisionItemLists for each rigidbody the hand is interacting with
            foreach(ContactItemList list in collisionLists) {
                // keep rigidbodys from sleeping, as collisions then are no longer registered
                if(list.collisionRigidbody.IsSleeping()) {
                    list.collisionRigidbody.WakeUp();
                }
                list.contacts.RemoveAll(c => c.age > 0);

                if(list.contacts.Count == 0) {
                    // notify the corresponding Collision handler
                    if(list.collisionHandler != null) {
                        notifyMtmHandExit(list.collisionHandler);
                        list.collisionHandler.notifyRemoveCollisionList(list, this);
                    }

                    // moved removal to after the loop
                    //collisionLists.Remove(list);
                }

                numCollisions += list.contacts.Count;

                // increase age of every collision
                foreach(ContactItem item in list.contacts) {
                    item.age++;
                }

                //handle the collisions
                if(list.collisionHandler != null) {
                    list.collisionHandler.handleCollisionList(list, this);
                }
            }
            collisionLists.RemoveAll(list => list.contacts.Count == 0);

            foreach(ContactItemList list in collisionLists) {
                if(list.moveWithHand && list.collisionRigidbody != null) {
                    //list.relativePosition = Quaternion.Inverse(handCollisionMaster.handController.handTransforms.handTransform.rotation) * 
                    //      (transform.position - handCollisionMaster.handController.handTransforms.handTransform.position);
                    //list.relativeRotation = Quaternion.Inverse(handCollisionMaster.handController.handTransforms.handTransform.rotation) * transform.rotation;

                    list.collisionRigidbody.MovePosition(handController.handMasterTransforms.handTransform.rotation * list.relativePosition + handController.handMasterTransforms.handTransform.position);
                    list.collisionRigidbody.MoveRotation(handController.handMasterTransforms.handTransform.rotation * list.relativeRotation);
                }
            }
        }

        void Update() {
            if(drawDebugRays) {
                foreach(ContactItemList list in collisionLists) {
                    foreach(ContactItem item in list.contacts) {
                        Color color = (list.isGrabbed) ? Color.magenta : Color.blue;
                        if(item.contact.separation < Physics.defaultContactOffset)
                            Debug.DrawRay(item.contact.point, item.contact.normal * 0.1f, color);
                    }
                }
            }
        }

        public void ReportCollisionEnter(Collision collision, int finger, int phalanx) {
            if(collision.rigidbody == null)
                return;

            ContactItemList list = collisionLists.Find(x => x.collisionRigidbody == collision.rigidbody);
            if(list == null) {
                list = new ContactItemList(collision.rigidbody);
                collisionLists.Add(list);
                // notify the corresponding Collision handler
                if(list.collisionHandler != null) {
                    list.collisionHandler.notifyNewCollisionList(list, this);
                    notifyMtmHandEnter(list.collisionHandler);
                }
            }
            for(int i = 0; i < collision.contactCount; i++) {
                ContactPoint contact = collision.GetContact(i);
                // make sure we have each contact only once in our list
                list.contacts.RemoveAll(c => c.contact.thisCollider == contact.thisCollider && c.contact.otherCollider == contact.otherCollider);
                list.contacts.Add(new ContactItem(contact, finger, phalanx));
            }
        }

        public void ReportCollisionStay(Collision collision, int finger, int phalanx) {
            if(collision.rigidbody == null)
                return;

            ContactItemList list = collisionLists.Find(x => x.collisionRigidbody == collision.rigidbody);
            if(list == null) {
                list = new ContactItemList(collision.rigidbody);
                collisionLists.Add(list);
                //Debug.Log("registerContinuingCollision: Creating new CollisionItemList for rigidbody " + collision.rigidbody.transform.name);
                // notify the corresponding Collision handler
                if(list.collisionHandler != null) {
                    list.collisionHandler.notifyNewCollisionList(list, this);
                }
            }
            for (int i = 0; i < collision.contactCount; i++) {
                ContactPoint contact = collision.GetContact(i);
                // make sure we have each contact only once in our list
                list.contacts.RemoveAll(c => c.Equals(contact));
                list.contacts.Add(new ContactItem(contact, finger, phalanx));
            }
        }

        public void ReportCollisionExit(Collision collision, int finger, int phalanx) {
            if(collision.rigidbody == null)
                return;

            ContactItemList list = collisionLists.Find(x => x.collisionRigidbody == collision.rigidbody);
            
            if(list == null) {
                Debug.Log("Trying to remove collision from nonexistant list!");
                return;
            }
            for (int i = 0; i < collision.contactCount; i++) {
                ContactPoint contact = collision.GetContact(i);
                // remove all contacts (though probably this won't do anything)
                list.contacts.RemoveAll(c => c.Equals(contact));
            }
            if(list.contacts.Count == 0) {
                // notify the corresponding Collision handler
                if(list.collisionHandler != null) {
                    list.collisionHandler.notifyRemoveCollisionList(list, this);
                    Debug.Log("here 2");
                }
                Debug.Log("here 3");
                notifyMtmHandExit(list.collisionHandler);

                collisionLists.Remove(list);
            }
        }

        void notifyMtmHandEnter(CollisionHandler interactableHandler)
        {
            GameObject MTMhandler = GameObject.Find("MTM-transcription");
            //Debug.Log(MTMhandler.name);
            //Debug.Log(gameObject.name);
            //Debug.Log(interactableHandler.name);
            //Debug.Log(interactableHandler.GetComponent<MovablesCollisionHandler>().isGrabbed);
            MTMhandler.GetComponent<ObjectInteractions>().addGraspedObject(interactableHandler.gameObject,gameObject.GetComponent<VRfreeGlove>().isRightHand);
        }
        void notifyMtmHandExit(CollisionHandler interactableHandler)
        {
            GameObject MTMhandler = GameObject.Find("MTM-transcription");
            //Debug.Log(MTMhandler.name);
            //Debug.Log(gameObject.name);
            //Debug.Log(interactableHandler.name);
            //Debug.Log(interactableHandler.GetComponent<MovablesCollisionHandler>().isGrabbed);
            MTMhandler.GetComponent<ObjectInteractions>().removeGraspedObj(interactableHandler.gameObject,gameObject.GetComponent<VRfreeGlove>().isRightHand);
        }

    }
}