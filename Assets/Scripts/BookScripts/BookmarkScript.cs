using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BookmarkEvent : UnityEvent<GameObject> {
}

public class BookmarkScript : MonoBehaviour
{
    public BookmarkEvent bookmarkEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ControllerHitbox") && bookmarkEvent != null) {
            bookmarkEvent.Invoke(gameObject);
        }
    }
}
