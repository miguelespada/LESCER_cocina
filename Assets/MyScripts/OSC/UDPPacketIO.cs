using System;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

  /// <summary>
  /// UdpPacket provides packetIO over UDP
  /// </summary>
  public class UDPPacketIO : MonoBehaviour 
  {
    private UdpClient Sender;
    private UdpClient Receiver;
    private bool socketsOpen;
    private string remoteHostName;
    private int remotePort;
    private int localPort;

    void Start() 
    {
    }

  	public void init(string hostIP, int remotePort, int localPort){
        RemoteHostName = hostIP;
        RemotePort = remotePort;
        LocalPort = localPort;
        socketsOpen = false;
  	}
  	

    ~UDPPacketIO()
    {
        // latest time for this socket to be closed
        if (IsOpen())
        {

            Debug.Log("closening udpclient listener on port " + localPort);
            Close();
        }
    }

    /// <summary>
    /// Open a UDP socket and create a UDP sender.
    /// 
    /// </summary>
    /// <returns>True on success, false on failure.</returns>
    public bool Open()
    {
        try
        {
            Sender = new UdpClient();
            Debug.Log("opening udpclient listener on port " + localPort);

            IPEndPoint listenerIp = new IPEndPoint(IPAddress.Any, localPort);
            
            Receiver = new UdpClient(listenerIp);
            socketsOpen = true;
            
            return true;
        }
        catch (Exception e)
        {
            //Debug.LogWarning("cannot open udp client interface at port "+localPort);
            Debug.LogWarning(e);
        }

        return false;
    }

    /// <summary>
    /// Close the socket currently listening, and destroy the UDP sender device.
    /// </summary>
    public void Close()
    {    
        if(Sender != null)
            Sender.Close();
        
        if (Receiver != null)
        {
          Debug.Log("UDP receiver closed");
        }
        Receiver = null;
        socketsOpen = false;

    }

    public void OnDisable()
    {
        Close();
    }

    /// <summary>
    /// Query the open state of the UDP socket.
    /// </summary>
    /// <returns>True if open, false if closed.</returns>
    public bool IsOpen()
    {
      return socketsOpen;
    }

    /// <summary>
    /// Send a packet of bytes out via UDP.
    /// </summary>
    /// <param name="packet">The packet of bytes to be sent.</param>
    /// <param name="length">The length of the packet of bytes to be sent.</param>
    public void SendPacket(byte[] packet, int length)
    {   
        if (!IsOpen())
            Open();
        if (!IsOpen())
            return;
      
        Sender.Send(packet, length, remoteHostName, remotePort);
        //Debug.Log("osc message sent to "+remoteHostName+" port "+remotePort+" len="+length);
    }

    /// <summary>
    /// Receive a packet of bytes over UDP.
    /// </summary>
    /// <param name="buffer">The buffer to be read into.</param>
    /// <returns>The number of bytes read, or 0 on failure.</returns>
    public int ReceivePacket(byte[] buffer)
    {
        if (!IsOpen())
            Open();
        if (!IsOpen())
            return 0;

      IPEndPoint iep = new IPEndPoint(IPAddress.Any, localPort);

        if (Receiver.Available > 0)
        {
            byte[] incoming = Receiver.Receive(ref iep);
            int count = Math.Min(buffer.Length, incoming.Length);
            System.Array.Copy(incoming, buffer, count);
            return count;
        }
        else
        {
            return 0;
        }
    }



    /// <summary>
    /// The address of the board that you're sending to.
    /// </summary>
    public string RemoteHostName
    {
      get
      { 
        return remoteHostName; 
      }
      set
      { 
        remoteHostName = value; 
      }
    }
  
    /// <summary>
    /// The remote port that you're sending to.
    /// </summary>
    public int RemotePort
    {
      get
      { 
        return remotePort; 
      }
      set
      { 
        remotePort = value; 
      }
    }

    /// <summary>
    /// The local port you're listening on.
    /// </summary>
    public int LocalPort
    {
      get
      {
        return localPort; 
      }
      set
      { 
        localPort = value; 
      }
    }
}
