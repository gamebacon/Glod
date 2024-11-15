using System;
using System.IO;
using System.Text;

public static class PacketHelper
{
    // Serialize packet data into a byte array
    public static byte[] SerializePacket(PacketType type, byte[] data)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(ms);

            // Write packet type
            writer.Write((int)type);

            // Write data length and data
            writer.Write(data.Length);
            writer.Write(data);

            return ms.ToArray();
        }
    }

    // Deserialize byte array into a packet
    public static (PacketType, byte[]) DeserializePacket(byte[] packetData)
    {
        using (MemoryStream ms = new MemoryStream(packetData))
        {
            BinaryReader reader = new BinaryReader(ms);

            // Read packet type
            PacketType type = (PacketType)reader.ReadInt32();

            // Read data
            int dataLength = reader.ReadInt32();
            byte[] data = reader.ReadBytes(dataLength);

            return (type, data);
        }
    }
}
