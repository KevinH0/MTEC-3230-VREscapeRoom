using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A UnityEvent with a float as a parameter
/// </summary>
[System.Serializable]
public class FloatEvent : UnityEvent<float> { }


/// <summary>
/// A UnityEvent with a 2 floats as parameters
/// </summary>
[System.Serializable]
public class FloatFloatEvent : UnityEvent<float, float> { }


/// <summary>
/// A UnityEvent with a RaycastHit as the parameter
/// </summary>
[System.Serializable]
public class RaycastHitEvent : UnityEvent<RaycastHit> { }



/// <summary>
/// A UnityEvent with a Vector2 as a parameter
/// </summary>
[System.Serializable]
public class Vector2Event : UnityEvent<Vector2> { }

/// <summary>
/// A UnityEvent with a Vector3 as a parameter
/// </summary>
[System.Serializable]
public class Vector3Event : UnityEvent<Vector3> { }

/// <summary>
/// A UnityEvent with a Vector3 as a parameter
/// </summary>
[System.Serializable]
public class PointerEventDataEvent : UnityEvent<UnityEngine.EventSystems.PointerEventData> { }
