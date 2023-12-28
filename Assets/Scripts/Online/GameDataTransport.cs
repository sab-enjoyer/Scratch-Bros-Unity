using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameDataTransport : MonoBehaviour
{
    private Render render;
    private void Start()
    {
        if (GameManager.instance.gameType != GameManager.GameTypes.Online)
        {
            Destroy(gameObject);
            return;
        }
        render = GameObject.Find("Render").GetComponent<Render>();
        
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("SendData", RecieveData);
        }
    }

    public void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("SendData");
    }
    public void SendMessage(TransportData data)
    {
        var customMessagingManager = NetworkManager.Singleton.CustomMessagingManager;
        byte[] serializedData = ToByte(data);
        FastBufferWriter writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(serializedData), Unity.Collections.Allocator.Temp);

        using (writer)
        {
            writer.WriteValueSafe(serializedData);

            customMessagingManager.SendNamedMessage("SendData", RelayManager.instance.ConnectedPlayers, writer);
            Debug.Log("sent data to opponent for frame " + data.gameFrame);
        }
    }

    public void RecieveData(ulong senderID, FastBufferReader messageData)
    {
        byte[] recievedData;
        messageData.ReadValueSafe(out recievedData);
        TransportData transportData = (TransportData)ToObject(recievedData);

        if (transportData.playerId != render.localPlayerID)
        {
            render.opponentInputs.Add(transportData.gameFrame, transportData.playerInput);
            Debug.Log("recieved opponent inputs for frame " + transportData.gameFrame);
        }

    }


    // you think i wrote this code?
    public byte[] ToByte(System.Object obj)
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new();
        MemoryStream ms = new();
        bf.Serialize(ms, obj);

        return ms.ToArray();
    }
    public System.Object ToObject(byte[] arrBytes)
    {
        MemoryStream ms = new();
        BinaryFormatter bf = new();
        ms.Write(arrBytes, 0, arrBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);
        System.Object obj = bf.Deserialize(ms);

        return obj;
    }
}

[System.Serializable]
public struct TransportData
{
    public int gameFrame;
    public byte playerId;
    public PlayerInput playerInput;

}