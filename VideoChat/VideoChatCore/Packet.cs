using System.Net;
using VideoChatCore;

public class Packet
{

    public PacketType Type => _type;
    public FrameState State => _state;
    public IPEndPoint From => _from;
    public byte[] Data => _data;

    private PacketType _type;
    private FrameState _state;
    private IPEndPoint _from;
    private byte[] _data;

    public Packet(PacketType type, FrameState state, byte[] data)
    {
        _type = type;
        _data = data;
        _state = state;
    }
    public void SetSender(IPEndPoint ep)
    {
        _from = ep;
    }
    public void ShowPacketData()
    {
        Console.WriteLine($"Package sender: {_from}");
        Console.WriteLine($"Package type: {_type}");
        Console.WriteLine($"Package frameState: {_state}");
        if(_data != null && _data.Length > 0)
        Console.WriteLine($"Package paylolad: {_data.Length}");
    }
    public byte[] GetBytes()
    {
        byte[] result = new byte[_data.Length + 2];
        result[0] = (byte)_type;
        result[1] = (byte)_state;
        Array.Copy(_data, 0, result, 2, _data.Length);

        return result;
    }

    public static Packet Connect()
    {
        return new Packet(PacketType.Connect, FrameState.Completed, new byte[0]);
    }
    public static Packet Disconnect()
    {
        return new Packet(PacketType.Disconnect, FrameState.Completed, new byte[0]);
    }
    public static Packet PacketFromBytes(byte[] bytes)
    {
        
        PacketType pType = (PacketType)bytes[0];
        FrameState state = (FrameState)bytes[1];
        byte[] payload = bytes[2..bytes.Length];

        return new Packet(pType,state, payload);
    }
}

public enum PacketType : int
{
    Connect,
    Disconnect,

    Video,
    Audio
}

