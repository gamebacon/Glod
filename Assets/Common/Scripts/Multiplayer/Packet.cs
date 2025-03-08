using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Packet : IDisposable
{
    private List<byte> buffer;
    private byte[] readableBuffer;
    private int readPos;
    private bool disposed;

    public Packet()
    {
        buffer = new List<byte>();
        readPos = 0;
    }

    public Packet(int _id)
    {
        buffer = new List<byte>();
        readPos = 0;
        Write(_id);
    }

    public Packet(byte[] _data)
    {
        buffer = new List<byte>();
        readPos = 0;
        SetBytes(_data);
    }

    private void EnsureReadableBuffer()
    {
        if (readableBuffer == null)
        {
            readableBuffer = buffer.ToArray();
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Packet Information:");
        sb.AppendLine($"Buffer Length: {buffer?.Count ?? 0}");
        sb.AppendLine($"Readable Buffer Length: {readableBuffer?.Length ?? 0}");
        sb.AppendLine($"Read Position: {readPos}");
        sb.AppendLine($"Disposed: {disposed}");

        if (buffer != null && buffer.Count > 0)
        {
            sb.Append("Buffer Data (Hex): ");
            foreach (byte b in buffer)
            {
                sb.Append($"{b:X2} ");
            }
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine("Buffer Data: Empty");
        }

        if (readableBuffer != null && readableBuffer.Length > 0)
        {
            sb.Append("Readable Buffer Data (Hex): ");
            foreach (byte b in readableBuffer)
            {
                sb.Append($"{b:X2} ");
            }
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine("Readable Buffer Data: Empty");
        }

        return sb.ToString();
    }

    public void SetBytes(byte[] _data)
    {
        if (_data == null || _data.Length == 0)
        {
            throw new ArgumentException("Data cannot be null or empty.", nameof(_data));
        }
        buffer.Clear();
        Write(_data);
        readableBuffer = buffer.ToArray();
        readPos = 0;
    }

    public void WriteLength() => buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
    public void InsertInt(int _value) => buffer.InsertRange(0, BitConverter.GetBytes(_value));

    public byte[] ToArray()
    {
        readableBuffer = buffer.ToArray();
        return readableBuffer;
    }

    public int Length() => buffer.Count;
    public int UnreadLength() => Length() - readPos;

    public void Reset(bool _shouldReset = true)
    {
        if (_shouldReset)
        {
            buffer.Clear();
            readableBuffer = null;
            readPos = 0;
        }
        else
        {
            readPos = Math.Max(0, readPos - 4); // Prevent negative read positions
        }
    }

    public void Write(byte _value) => buffer.Add(_value);

    public void Write(byte[] _value)
    {
        if (_value == null)
        {
            throw new ArgumentNullException(nameof(_value), "Cannot write a null byte array.");
        }
        buffer.AddRange(_value);
    }

    public void Write(short _value) => buffer.AddRange(BitConverter.GetBytes(_value));
    public void Write(int _value) => buffer.AddRange(BitConverter.GetBytes(_value));
    public void Write(long _value) => buffer.AddRange(BitConverter.GetBytes(_value));
    public void Write(float _value) => buffer.AddRange(BitConverter.GetBytes(_value));
    public void Write(bool _value) => buffer.AddRange(BitConverter.GetBytes(_value));

    public void Write(string _value)
    {
        if (_value == null)
        {
            throw new ArgumentNullException(nameof(_value), "Cannot write a null string.");
        }
        Write(_value.Length);
        buffer.AddRange(Encoding.ASCII.GetBytes(_value));
    }

    public void Write(Vector3 _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
    }

    public void Write(Quaternion _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
        Write(_value.w);
    }

    public byte ReadByte(bool _moveReadPos = true)
    {
        EnsureReadableBuffer();
        if (buffer.Count <= readPos)
            throw new Exception("Could not read value of type 'byte'!");
        byte value = readableBuffer[readPos];
        if (_moveReadPos)
            readPos++;
        return value;
    }

    public byte[] ReadBytes(int _length, bool _moveReadPos = true)
    {
        EnsureReadableBuffer();
        if (buffer.Count <= readPos)
            throw new Exception("Could not read value of type 'byte[]'!");
        byte[] value = buffer.GetRange(readPos, _length).ToArray();
        if (_moveReadPos)
            readPos += _length;
        return value;
    }

    public byte[] CloneBytes() => buffer.ToArray();

    public short ReadShort(bool _moveReadPos = true)
    {
        EnsureReadableBuffer();
        if (readPos + sizeof(short) > readableBuffer.Length)
            throw new Exception("Could not read value of type 'short'!");
        short value = BitConverter.ToInt16(readableBuffer, readPos);
        if (_moveReadPos)
            readPos += sizeof(short);
        return value;
    }

    public int ReadInt(bool _moveReadPos = true)
    {
        EnsureReadableBuffer();
        if (readPos + sizeof(int) > readableBuffer.Length)
            throw new Exception("Could not read value of type 'int'!");
        int value = BitConverter.ToInt32(readableBuffer, readPos);
        if (_moveReadPos)
            readPos += sizeof(int);
        return value;
    }

    public long ReadLong(bool _moveReadPos = true)
    {
        EnsureReadableBuffer();
        if (readPos + sizeof(long) > readableBuffer.Length)
            throw new Exception("Could not read value of type 'long'!");
        long value = BitConverter.ToInt64(readableBuffer, readPos);
        if (_moveReadPos)
            readPos += sizeof(long);
        return value;
    }

    public float ReadFloat(bool _moveReadPos = true)
    {
        EnsureReadableBuffer();
        if (readPos + sizeof(float) > readableBuffer.Length)
            throw new Exception("Could not read value of type 'float'!");
        float value = BitConverter.ToSingle(readableBuffer, readPos);
        if (_moveReadPos)
            readPos += sizeof(float);
        return value;
    }

    public bool ReadBool(bool _moveReadPos = true)
    {
        EnsureReadableBuffer();
        if (readPos + sizeof(bool) > readableBuffer.Length)
            throw new Exception("Could not read value of type 'bool'!");
        bool value = BitConverter.ToBoolean(readableBuffer, readPos);
        if (_moveReadPos)
            readPos++;
        return value;
    }

    public string ReadString(bool _moveReadPos = true)
    {
        EnsureReadableBuffer();
        try
        {
            int length = ReadInt();
            if (length <= 0)
                return string.Empty;

            string value = Encoding.ASCII.GetString(readableBuffer, readPos, length);
            if (_moveReadPos)
                readPos += length;
            return value;
        }
        catch
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }

    public Vector3 ReadVector3(bool moveReadPos = true) =>
        new Vector3(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));

    public Quaternion ReadQuaternion(bool moveReadPos = true) =>
        new Quaternion(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));

    protected virtual void Dispose(bool _disposing)
    {
        if (disposed)
            return;
        if (_disposing)
        {
            buffer = null;
            readableBuffer = null;
            readPos = 0;
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}