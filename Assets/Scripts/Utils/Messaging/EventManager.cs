/// <summary>
/// Copyright Aristocrat Technologies Australia Pty Ltd 1994-2018. All rights reserved.
/// 
/// Used to add, remove, and dispatch events. See EventManager class summary 
/// for more details.
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Utils
{
    //All callbacks are of this type when using EventManager
    //protected void serverMessageHandler(object sender, EventManagerEventArgs e)
    public delegate void Delegate(object sender, EventManagerEventArgs e);

    /// <summary>
    /// Gets passed to dispatchEvent to pass data to parent class
    /// EventManagerEventArgs args = new EventManagerEventArgs();
    /// args.eventObject = messageObject;
    /// EventManager.instance.dispatchEvent(this, EventManager.EVENT, args);
    /// </summary>
    public class EventManagerEventArgs : EventArgs
    {
        public object eventObject { get; set; }
    }

    /// <summary>
    /// server = new Server ();
    /// EventManager.instance.addEventListener(this, server, EventManager.EVENT, ServerMessageHandler);
    /// server.load();
    /// ...
    /// protected void ServerMessageHandler(object sender, EventManagerEventArgs e)
    /// {
    /// 	MessageObject messageObject	= (MessageObject)e.eventObject;
    /// ...
    /// </summary>
    public class EventManager
    {
        protected static EventManager instance;

        //Tracks children and events
        protected List<ParentObject> parentObjects;

        //Constructor
        protected EventManager()
        {
            parentObjects = new List<ParentObject>();
        }

        /// <summary>
        /// Use to access the Singleton. When referencing EventManager, use
        /// EventManager.Instance. syntax.
        /// </summary>
        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Registers an event listener object with an EventManager object so 
        /// that the listener receives notification of an event. If you no 
        /// longer need an event listener, remove it by calling 
        /// removeEventListener().
        /// </summary>
        /// <param name="thisParent"></param>
        /// <param name="thisChild"></param>
        /// <param name="thisEventName">The type of event.</param>
        /// <param name="thisCallback">The listener function that processes the event</param>
        public void AddEventListener(object thisParent, object thisChild, string thisEventName, Delegate thisCallback)
        {
            testEventListenerParamaters(thisParent, thisChild, thisEventName, thisCallback);
            
            bool listenersAdded = false;
            bool isParent = false;
            bool isChild = false;
            bool hasEvent = false;
            bool hasCallback = false;

            //Loop through the parents to see if this parent has already been assigned to a ParentObject
            for (int p = 0; p < parentObjects.Count; p++)
            {
                ParentObject parentObject = parentObjects[p];
                if (parentObject.Value == thisParent)
                {
                    isParent = true;
                }

                //Loop through the child objects to see if the child has already been assigned to a ChildObject
                for (int c = 0; c < parentObject.ChildObjects.Count; c++)
                {
                    ChildObject childObject = parentObject.ChildObjects[c];
                    if (childObject.Value == thisChild)
                    {
                        isChild = true;
                    }

                    //Loop through the event name objects to see if the event name has already been assigned to a Event Name Object
                    for (int e = 0; e < childObject.EventNameObjects.Count; e++)
                    {
                        EventNameObject eventNameObject = childObject.EventNameObjects[e];
                        if (eventNameObject.eventName == thisEventName && isChild)
                        {
                            hasEvent = true;
                        }

                        //Loop through the event callback objects to see if the callback has already been assigned to a Delegate Object
                        for (int f = 0; f < eventNameObject.callbacks.Count; f++)
                        {
                            Delegate callback = eventNameObject.callbacks[f];
                            if (hasEvent && isChild && callback == thisCallback)
                            {
                                hasCallback = true;
                            }
                            if (isParent && isChild && hasEvent && hasCallback)
                            {
                                Debug.LogError("EventManager::AddEventListener called, but object already has event listener for this event.\n\tParent Object: " + thisParent + "\n\tChild Object: " + thisChild + "\n\tevent: " + thisEventName + "\n" + getListenersAsString());
                                return;
                            }
                        }
                        if (isParent && isChild && hasEvent && !hasCallback && !listenersAdded)
                        {
                            eventNameObject.callbacks.Add(thisCallback);
                            listenersAdded = true;
                            hasCallback = true;
                            break;
                        }
                    }
                    if (isParent && isChild && !hasEvent && !listenersAdded)
                    {
                        EventNameObject newEventNameObject = new EventNameObject(thisEventName, thisCallback);
                        childObject.EventNameObjects.Add(newEventNameObject);
                        hasEvent = true;
                        listenersAdded = true;
                        break;
                    }
                }
                if (isParent && !isChild && !listenersAdded)
                {
                    ChildObject newChildObject = new ChildObject(thisChild, thisEventName, thisCallback);
                    parentObject.ChildObjects.Add(newChildObject);
                    isChild = true;
                    listenersAdded = true;
                    break;
                }

                isChild = false;
                hasEvent = false;
                hasCallback = false;
            }

            if (!isParent)
            {
                parentObjects.Add(new ParentObject(thisParent, thisChild, thisEventName, thisCallback));
                listenersAdded = true;
            }

            if (!listenersAdded)
            {
                Debug.LogError("EventManager::AddEventListener called but event listener could not be added for an unknown reason.\n\tParent Object: " + thisParent + "\n\tChild Object: " + thisChild + "\n\tevent: " + thisEventName + "\n\tCallback: " + thisCallback.Method.Name + "\n" + getListenersAsString());
            }
        }

        /// <summary>
        /// Removes a listener from the EventDispatcher object. If there is no matching listener registered with the EventDispatcher object, a call to this method has no effect.
        /// </summary>
        /// <param name="thisParent"></param>
        /// <param name="thisChild"></param>
        /// <param name="thisEventName"></param>
        /// <param name="thisCallback"></param>
        public void RemoveEventListener(object thisParent, object thisChild, string thisEventName, Delegate thisCallback)
        {
            testEventListenerParamaters(thisParent, thisChild, thisEventName, thisCallback);

            bool listenerRemoved = false;
            for (int p = 0; p < parentObjects.Count; p++)
            {
                ParentObject parentObject = parentObjects[p];
                if (parentObject.Value == thisParent)
                {
                    for (int c = 0; c < parentObject.ChildObjects.Count; c++)
                    {
                        ChildObject childObject = parentObject.ChildObjects[c];
                        if (childObject.Value == thisChild)
                        {
                            for (int e = 0; e < childObject.EventNameObjects.Count; e++)
                            {
                                EventNameObject eventNameObject = childObject.EventNameObjects[e];
                                if (eventNameObject.eventName == thisEventName)
                                {
                                    for (int f = 0; f < eventNameObject.callbacks.Count; f++)
                                    {
                                        Delegate callback = eventNameObject.callbacks[f];
                                        if (callback == thisCallback)
                                        {
                                            listenerRemoved = true;
                                            eventNameObject.callbacks.Remove(callback);
                                            if (eventNameObject.callbacks.Count == 0)
                                            {
                                                childObject.EventNameObjects.Remove(eventNameObject);
                                                if (childObject.EventNameObjects.Count == 0)
                                                {
                                                    parentObject.ChildObjects.Remove(childObject);
                                                    if (parentObject.ChildObjects.Count == 0)
                                                    {
                                                        parentObject = parentObject.Unload();
                                                        parentObjects.RemoveAt(p);
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            if (!listenerRemoved)
            {
                Debug.LogError("EventManager::RemoveEventListener called, but object does not have an event listener for this event.\nObject: " + thisParent + ", child: " + thisChild + ", event: " + thisEventName + "\n" + getListenersAsString());
            }
        }

        /// <summary>
        /// Similar to RemoveEventListener, but will remove all listeners for a class.
        /// </summary>
        /// <param name="thisParent"></param>
        public void RemoveClassListeners(object thisParent)
        {
            if (parentObjects.Count > 0)
            {
                for (int p = 0; p < parentObjects.Count; p++)
                {
                    ParentObject parentObject = parentObjects[p];
                    if (parentObject.Value == thisParent)
                    {
                        while (parentObject.ChildObjects.Count > 0)
                        {
                            ChildObject childObject = parentObject.ChildObjects[0];
                            while (childObject.EventNameObjects.Count > 0)
                            {
                                EventNameObject eventNameObject = childObject.EventNameObjects[0];
                                while (eventNameObject.callbacks.Count > 0)
                                {
                                    eventNameObject.callbacks.RemoveAt(0);
                                }
                                if (eventNameObject.callbacks.Count == 0)
                                {
                                    childObject.EventNameObjects.RemoveAt(0);
                                }
                            }
                            if (childObject.EventNameObjects.Count == 0)
                            {
                                parentObject.ChildObjects.RemoveAt(0);
                            }
                        }
                        parentObject = parentObject.Unload();
                        parentObjects.RemoveAt(p);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the EventDispatcher object has any listeners 
        /// registered for a specific type of event. This allows you to 
        /// determine where an EventDispatcher object has altered handling 
        /// of an event type in the event flow hierarchy.
        /// </summary>
        /// <param name="thisParent"></param>
        /// <param name="thisChild"></param>
        /// <param name="thisEventName"></param>
        /// <param name="thisCallback"></param>
        /// <returns></returns>
        public bool HasEventListener(object thisParent, object thisChild, string thisEventName, Delegate thisCallback)
        {
            testEventListenerParamaters(thisParent, thisChild, thisEventName, thisCallback);

            for (int p = 0; p < parentObjects.Count; p++)
            {
                ParentObject parentObject = parentObjects[p];
                if (parentObject.Value == thisParent)
                {
                    for (int c = 0; c < parentObject.ChildObjects.Count; c++)
                    {
                        ChildObject childObject = parentObject.ChildObjects[c];
                        if (childObject.Value == thisChild)
                        {
                            for (int e = 0; e < childObject.EventNameObjects.Count; e++)
                            {
                                EventNameObject eventNameObject = childObject.EventNameObjects[e];
                                if (eventNameObject.eventName == thisEventName)
                                {
                                    for (int f = 0; f < eventNameObject.callbacks.Count; f++)
                                    {
                                        Delegate callback = eventNameObject.callbacks[f];
                                        if (callback == thisCallback)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Dispatches an event into the event flow. The event target is the EventDispatcher object upon which the dispatchEvent() method is called.
        /// </summary>
        /// <param name="thisChild"></param>
        /// <param name="thisEventName"></param>
        /// <param name="thisEventObject"></param>
        public void DispatchEvent(object thisChild, string thisEventName, EventManagerEventArgs thisEventObject = null)
        {
            if (thisChild == null)
            {
                Debug.LogError("EventManager::DispatchEvent called on an object that is null.\nObject: " + thisChild + ", event: " + thisEventName);
                LogAllEventListeners();
            }
            else if (thisEventName == null)
            {
                Debug.LogError("EventManager::DispatchEvent called, but the event name is null.\nObject: " + thisChild + ", event: " + thisEventName);
                LogAllEventListeners();
            }
            List<Delegate> delegates = new List<Delegate>();

            for (int p = 0; p < parentObjects.Count; p++)
            {
                ParentObject parentObject = parentObjects[p];
                if (parentObject.ChildObjects == null)
                {
                    Debug.LogError("EventManager::DispatchEvent called, but parent object has no child objects. This can be caused by unloading a class without removing all of its event listeners.\nObject: " + thisChild + ", event: " + thisEventName + "\nparentObject: " + parentObject.Value + ", parentObject.ChildObjects: " + parentObject.ChildObjects);
                    LogAllEventListeners();
                    break;
                }
                for (int c = 0; c < parentObject.ChildObjects.Count; c++)
                {
                    ChildObject childObject = parentObject.ChildObjects[c];
                    for (int e = 0; e < childObject.EventNameObjects.Count; e++)
                    {
                        EventNameObject eventNameObject = childObject.EventNameObjects[e];
                        for (int f = 0; f < eventNameObject.callbacks.Count; f++)
                        {
                            Delegate callback = eventNameObject.callbacks[f];
                            if (thisChild == childObject.Value && eventNameObject.eventName == thisEventName)
                            {
                                delegates.Add(callback);
                            }
                        }
                    }
                }
            }

            for (int d = 0; d < delegates.Count; d++)
            {
                delegates[d].Invoke(thisChild, thisEventObject);
            }
        }

        /// <summary>
        /// Use to log all event listeners currently in the hiearchy.
        /// </summary>
        public void LogAllEventListeners()
        {
            Debug.Log(getListenersAsString());
        }

        /// <summary>
        /// Returns a string representation of all of the event listeners in EventManager.
        /// </summary>
        /// <returns></returns>
        protected string getListenersAsString()
        {
            StringBuilder stringInfo = new StringBuilder("Here is a list of all objects and event listeners:\n");
            for (int p = 0; p < parentObjects.Count; p++)
            {
                ParentObject parentObject = parentObjects[p];
                stringInfo.Append("Parent Object " + p + ": " + parentObject.Value.ToString() + "\n");
                for (int c = 0; c < parentObject.ChildObjects.Count; c++)
                {
                    ChildObject childObject = parentObject.ChildObjects[c];
                    stringInfo.Append("\tChild Object " + c + ": " + childObject.Value.ToString() + "\n");
                    for (int e = 0; e < childObject.EventNameObjects.Count; e++)
                    {
                        EventNameObject eventNameObject = childObject.EventNameObjects[e];
                        stringInfo.Append("\t\tEvent " + e + ": " + eventNameObject.eventName + "\n");
                        for (int f = 0; f < eventNameObject.callbacks.Count; f++)
                        {
                            Delegate callback = eventNameObject.callbacks[f];
                            stringInfo.Append("\t\tCallback " + f + ": " + callback.Method.Name + "\n");
                        }
                    }
                }
            }

            return stringInfo.ToString();
        }

        /// <summary>
        /// Called by several functions to test for problems.
        /// </summary>
        /// <param name="thisParent"></param>
        /// <param name="thisChild"></param>
        /// <param name="thisEventName"></param>
        /// <param name="thisCallback"></param>
        protected void testEventListenerParamaters(object thisParent, object thisChild, string thisEventName, Delegate thisCallback)
        {
            if (thisParent == null)
            {
                Debug.LogError("Could not handle event listener for parent object. Parent object is null.");
                return;
            }
            else if (thisChild == null)
            {
                Debug.LogError("Could not handle event listener for " + thisParent.ToString() + ". Child object is null.");
                return;
            }
            else if (thisEventName == null)
            {
                Debug.LogError("Could not handle event listener for " + thisParent.ToString() + ". Event name is null.");
                return;
            }
            else if (thisCallback == null)
            {
                Debug.LogError("Could not handle event listener for " + thisParent.ToString() + ". Callback name is null.");
                return;
            }

            if (thisParent is GameObject)
            {
                Debug.LogError("Could not handle event listener for " + thisParent.ToString() + ". Parent is a GameObject and not a script.");
                return;
            }
            if (thisChild is GameObject)
            {
                Debug.LogError("Could not handle event listener for " + thisChild.ToString() + ". Child is a GameObject and not a script.");
                return;
            }
        }

        /// <summary>
        /// Holds the parent object 
        /// </summary>
        protected class ParentObject
        {
            public object Value;
            public List<ChildObject> ChildObjects;

            public ParentObject(object pParentObject, object childObject, string eventName, Delegate pCallback)
            {
                Value = pParentObject;

                ChildObject childDataObject = new ChildObject(childObject, eventName, pCallback);
                ChildObjects = new List<ChildObject>() { childDataObject };
            }

            public ParentObject Unload()
            {
                Value = null;

                for (int c = 0; c < ChildObjects.Count; c++)
                {
                    ChildObjects[c] = ChildObjects[c].unload();
                }

                ChildObjects.Clear();
                ChildObjects = null;

                return null;
            }
        }

        /// <summary>
        /// Holds the child object 
        /// </summary>
        protected class ChildObject
        {
            public object Value;
            public List<EventNameObject> EventNameObjects;

            public ChildObject(object pChildObject, string eventName, Delegate pCallback)
            {
                Value = pChildObject;

                EventNameObject eventNameObject = new EventNameObject(eventName, pCallback);
                EventNameObjects = new List<EventNameObject>() { eventNameObject };
            }

            public ChildObject unload()
            {
                Value = null;

                for (int e = 0; e < EventNameObjects.Count; e++)
                {
                    EventNameObjects[e] = EventNameObjects[e].unload();
                }

                EventNameObjects.Clear();
                EventNameObjects = null;

                return null;
            }
        }

        /// <summary>
        /// Holds the event name object
        /// </summary>
        protected class EventNameObject
        {
            public string eventName;
            public List<Delegate> callbacks;

            public EventNameObject(string pEventName, Delegate pCallback)
            {
                eventName = pEventName;
                callbacks = new List<Delegate>() { pCallback };
            }

            public EventNameObject unload()
            {
                eventName = null;
                callbacks.Clear();
                callbacks = null;

                return null;
            }
        }
    }
}