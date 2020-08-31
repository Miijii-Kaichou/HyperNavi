using System;
using UnityEngine;

public static class Keymapper
{
    /// <summary>
    /// The certain state of a specified key
    /// </summary>
    public enum KeyState
    {
        NONE,
        DOWN,
        RELEASED,
        HOLD
    }

    /// <summary>
    /// No Key Exception
    /// </summary>
    public class NoKeyException : Exception
    {
        public NoKeyException() : base() { }
        public NoKeyException(object context) : base(string.Format("This is no key logged on this keymapper: {0}", context)) { }
    }

    /// <summary>
    /// Create a key with a name, key code, and event
    /// </summary>
    public class Key
    {
        /// <summary>
        /// Name of the key
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The associated key
        /// </summary>
        public KeyCode KeyCode { get; private set; }

        /// <summary>
        /// Event tied to interacting with the key
        /// </summary>
        EventManager.Event keyEvent;

        /// <summary>
        /// The key state in which to execute the event
        /// </summary>
        public KeyState KeyState { get; private set; } = KeyState.NONE;

        /// <summary>
        /// Log in a new key
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        public Key(string name, KeyCode key)
        {
            Name = name;
            KeyCode = key;
            
        }

        /// <summary>
        /// Log in a new key with an attached event
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <param name="keyEvent"></param>
        public Key(string name, KeyCode key, EventManager.Event keyEvent)
        {
            Key newKey = new Key(name, key);
            newKey.SetKeyEvent(keyEvent);
        }

        /// <summary>
        /// Log in a new key with an attached event given a certain state of the key
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <param name="keyEvent"></param>
        /// <param name="keyState"></param>
        public Key(string name, KeyCode key, EventManager.Event keyEvent, KeyState keyState = KeyState.NONE)
        {
            _ = new Key(name, key, keyEvent);
            KeyState = keyState;
        }

        /// <summary>
        /// Set key event
        /// </summary>
        /// <param name="keyEvent"></param>
        public void SetKeyEvent(EventManager.Event keyEvent)
        {
            if(this.keyEvent == null)
                this.keyEvent = keyEvent;
        }

        /// <summary>
        /// Toggle KeyEvent
        /// </summary>
        public void ToggleKeyEvent()
        {
            if(keyEvent.HasListerners())
                keyEvent.Trigger();
        }

        /// <summary>
        /// Check if there's an event attached to this key
        /// </summary>
        /// <returns></returns>
        public bool HasAttachedEvent() => keyEvent != null;
    }

    static Key[] loggedKeys;

    public static void Configure(params Key[] keys)
    {
        int size = keys.Length + 1;
        
        loggedKeys = new Key[size];

        foreach (Key key in keys)
        {
            AddNewKey(key);
        }
    }

    static void AddNewKey(Key newKey)
    {
        for (int index = 0; index < loggedKeys.Length - 1; index++)
        {
            if (loggedKeys[index] == null)
            {
                loggedKeys[index] = newKey;
                return;
            }
        }
    }

    public static bool OnKeyDown(string name, bool triggerEvent = false)
    {
        try
        {
            Key currentKey = GetKeyByName(name);

            if (currentKey == null) throw new NoKeyException(currentKey);

            if (currentKey.HasAttachedEvent() && 
                triggerEvent && 
                (currentKey.KeyState == KeyState.NONE ||
                currentKey.KeyState == KeyState.DOWN))
                currentKey.ToggleKeyEvent();

            return Input.GetKeyDown(currentKey.KeyCode);
        }
        catch(NoKeyException noKey) {
            Debug.LogException(noKey);
            return false;
        }
    }

    public static bool OnKey(string name, bool triggerEvent = false)
    {
        try
        {
            Key currentKey = GetKeyByName(name);

            if (currentKey == null) throw new NoKeyException(currentKey);

            if (currentKey.HasAttachedEvent() && 
                triggerEvent &&
                (currentKey.KeyState == KeyState.NONE ||
                currentKey.KeyState == KeyState.HOLD))
                currentKey.ToggleKeyEvent();

            return Input.GetKey(currentKey.KeyCode);
        }
        catch(NoKeyException noKey) { 
            Debug.LogException(noKey); 
            return false;
        }
    }

    public static bool OnKeyRelease(string name, bool triggerEvent = false)
    {
        try
        {
            Key currentKey = GetKeyByName(name);

            if (currentKey == null) throw new NoKeyException(currentKey);

            if (currentKey.HasAttachedEvent() &&
                triggerEvent &&
                (currentKey.KeyState == KeyState.NONE ||
                currentKey.KeyState == KeyState.RELEASED))
                currentKey.ToggleKeyEvent();

            return Input.GetKeyUp(currentKey.KeyCode);
        } catch(NoKeyException noKey) {
            Debug.LogException(noKey);
            return false;
        }
    }

    static Key GetKeyByName(string name)
    {
        for (int index = 0; index < loggedKeys.Length - 1; index++)
        {
            if (loggedKeys[index].Name.Equals(name))
                return loggedKeys[index];
        }
        return null;
    }

    static void RemoveKey(string name)
    {
        for(int index = 0; index < loggedKeys.Length - 1; index++)
        {
            if (loggedKeys[index].Name.Equals(name))
            {
                loggedKeys[index] = null;
                return;
            }
        }
    } 
}
