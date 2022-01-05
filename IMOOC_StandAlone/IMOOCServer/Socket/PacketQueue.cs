using System.Collections.Generic;
using System.Net.Sockets;

namespace IMOOCServer
{
    public struct BufferPacket
    {
        public int offset;
        public int count;
    }

    public class PacketQueue
    {
        private DynamicBuffer m_DynamicBuffer;
        private Queue<BufferPacket> m_Queue;

        public PacketQueue()
        {
            m_Queue = new Queue<BufferPacket>();
            m_DynamicBuffer = new DynamicBuffer(2000);
        }

        public void SetPacket(byte[] buffer, int offset, int count)
        {
            var newPacket = new BufferPacket();
            newPacket.offset = m_DynamicBuffer.WriteBuffer(buffer, offset, count);
            newPacket.count = count;
            m_Queue.Enqueue(newPacket);
        }

        public bool GetPacket(List<SocketAsyncEventArgs> socketArgsList)
        {
            lock (m_Queue)
            {
                if (m_Queue.Count > 0)
                {
                    var packet = m_Queue.Dequeue();
                    for (int i = 0; i < socketArgsList.Count; i++)
                    {
                        ((AsyncUserToken)socketArgsList[i].UserToken).IsSend = true;
                        socketArgsList[i].SetBuffer(m_DynamicBuffer.Buffer, packet.offset, packet.count);
                    }
                    m_DynamicBuffer.ClearBuffer(packet.offset, packet.count);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
        }

    }
}
