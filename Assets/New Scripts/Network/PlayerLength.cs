
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLength : NetworkBehaviour
{
    [SerializeField] GameObject tailPrefab;

    public NetworkVariable<ushort> length = new(1);

    private List<GameObject> _tails;
    private Transform _lastTail;
    private Collider2D _collider2D;

    public static event Action<ushort> ChangedLengthEvent;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _tails = new List<GameObject>();
        _lastTail = transform;
        _collider2D = GetComponent<Collider2D>();

        if(!IsServer)length.OnValueChanged += LengthChangedEvent;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        DestroyTails();
    }

    private void DestroyTails()
    {
        while(_tails.Count != 0)
        {
            GameObject tail = _tails[0];
            _tails.RemoveAt(0);

            Destroy(tail);
        }
    }

    // Will only be called by the server
    //[ContextMenu("Add Length")]
    public void AddLengthServer()
    {
        if (!IsServer)
            return;

        length.Value += 1;
        LengthChanged();
    }

    private void LengthChanged()
    {
        InstantiateTail();

        if (!IsOwner) return;
        ChangedLengthEvent?.Invoke(length.Value);
        ClientMusicPlayer.Instance.PlayNomAudioClip();
    }

    private void LengthChangedEvent(ushort prevValue, ushort newValue)
    {
        Debug.Log("Length Changed Callback");
        LengthChanged();
    }

    private void InstantiateTail()
    {
        GameObject tailGameobject = Instantiate(tailPrefab, transform.position, Quaternion.identity);
        tailGameobject.GetComponent<SpriteRenderer>().sortingOrder = -length.Value;

        if(tailGameobject.TryGetComponent(out Tail tail))
        {
            tail.networkedOwner = transform;
            tail.followTransform = _lastTail;
            _lastTail = tailGameobject.transform;
            Physics2D.IgnoreCollision(tailGameobject.GetComponent<Collider2D>(), _collider2D);
        }
        _tails.Add(tailGameobject);
    }
}
