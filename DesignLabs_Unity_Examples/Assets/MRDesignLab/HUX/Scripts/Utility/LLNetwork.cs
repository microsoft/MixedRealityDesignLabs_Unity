//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class LLNetwork
{
    int chanId;
    int relChanId;

    int hostId;
    int connId;

    public bool bReceiverClient;
    bool connected;
    public int hostPort = 9004;

    public float reliablePeriod = 0.5f;
    public float unreliablePeriod = 1f / 30f;

    float relTimer;
    float unrelTimer;

    public event Action<bool> OnSendTimer;
    public event Action<byte[]> OnReceivedBytes;

    // Since Unity's low level network is more like a singleton
    static bool firstNetwork;
    static event Action<int, int, byte[], bool> networkCallback = new Action<int, int, byte[], bool>((a, b, c, d) => { });

    static bool weirdError;

    void Awake()
    {
        firstNetwork = false;
    }

    void StartNetwork(string hostIp)
    {
        if (!firstNetwork)
        {
            Debug.Log("Someone called StartNetwork");
            firstNetwork = true;

            // Try to workaround a bug in Unity?
            try
            {
                NetworkTransport.Init();
                int recHostId, connectionId, channelId, dataSize;
                const int bufferSize = 128;
                byte[] recBuffer = new byte[bufferSize];
                byte error;
                NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
            }
            catch (Exception e)
            {
                Debug.Log("NetworkTransport exception, trying to shutdown and re-init: " + e.Message);
                NetworkTransport.Shutdown();
            }

            NetworkTransport.Init();
        }

        networkCallback += networkOnEvent;

        ConnectionConfig cc = new ConnectionConfig();
        chanId = cc.AddChannel(QosType.UnreliableSequenced);
        relChanId = cc.AddChannel(QosType.ReliableSequenced);

        HostTopology top = new HostTopology(cc, 10);
        hostId = NetworkTransport.AddHost(top, bReceiverClient ? hostPort + 1 : hostPort);
        Debug.Log("hostid: " + hostId);

        if (bReceiverClient)
        {
            byte error;
            connId = NetworkTransport.Connect(hostId, hostIp, hostPort, 0, out error);
            Debug.Log("connid: " + connId);
        }
    }

    public void StartHost(int hostOnPort = 9004)
    {
        bReceiverClient = false;
        hostPort = hostOnPort;
        StartNetwork("");
    }

    public void StartClient(string connectToIP = "127.0.0.1", int connectToPort = 9004)
    {
        bReceiverClient = true;
        hostPort = connectToPort;
        StartNetwork(connectToIP);
    }

    public void SendBytes(byte[] bytes, bool reliable)
    {
        byte error;
        NetworkTransport.Send(hostId, connId, reliable ? relChanId : chanId, bytes, bytes.Length, out error);
    }

    public void Update()
    {
        // Send position/rotation of veil!
        if (connected)
        {
            //if (!bReceiverClient)
            {
                relTimer -= Time.deltaTime;
                unrelTimer -= Time.deltaTime;

                bool rel = relTimer <= 0;
                bool unrel = unrelTimer <= 0;

                // Send your updates
                if (rel || unrel)
                {
                    if (OnSendTimer != null)
                    {
                        OnSendTimer(rel);
                    }
                }

                // Reliable update every half second
                if (rel)
                {
                    relTimer += reliablePeriod;
                }

                // Unreliable updates 20 times / sec
                if (unrel)
                {
                    unrelTimer += unreliablePeriod;
                }
            }
        }

        if (firstNetwork)
        {
            NetUpdate();
        }
    }

    // LLNetwork instances process this
    void networkOnEvent(int recHostId, int recConnId, byte[] bytes, bool isConnected)
    {
        if (recHostId != hostId)
        {
            return;
        }

        if (bytes == null)
        {
            connected = isConnected;
        }
        else
        {
            OnReceivedBytes(bytes);
        }
    }

    // Called only by the first instance
    public static void NetUpdate()
    {
        // Update network
        int recHostId;
        int connectionId;
        int channelId;
        const int bufferSize = 128;
        byte[] recBuffer = new byte[bufferSize];
        int dataSize;
        byte error;

        if (weirdError)
        {
            return;
        }

        try
        {
            NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
            switch (recData)
            {

                case NetworkEventType.ConnectEvent:
                {
                    Debug.Log("connect event: host: " + recHostId + ", conn: " + connectionId);
                    networkCallback(recHostId, connectionId, null, true);
                    break;
                }

                case NetworkEventType.DataEvent:
                {
                    if (networkCallback != null)
                    {
                        byte[] dataBytes = new byte[dataSize];
                        Array.Copy(recBuffer, dataBytes, dataSize);

                        networkCallback(recHostId, connectionId, dataBytes, true);
                    }
                    break;
                }

                case NetworkEventType.DisconnectEvent:
                {
                    Debug.Log("disconnect event: host: " + recHostId + ", conn: " + connectionId);
                    networkCallback(recHostId, connectionId, null, false);
                    break;
                }


                case NetworkEventType.Nothing:
                default:
                {
                    break;
                }
            }
        }
        catch (Exception e)
        {
            weirdError = true;
            Debug.LogError(e);
        }
    }
}
