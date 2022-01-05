using System;

namespace IMOOCServer
{
    public class DynamicBuffer
    {
        public byte[] Buffer { get; private set; } //存放内存的数组  
        private int firstIndex;
        private int firstOffset;
        private int secondIndex;
        private int secondOffset;

        public DynamicBuffer(int bufferSize)
        {
            firstOffset = 0;
            Buffer = new byte[bufferSize];
        }

        public int WriteBuffer(byte[] buffer, int offset, int count)
        {
            int writeOffset;
            if (Buffer.Length - firstOffset >= count) //缓冲区空间够，不需要申请  
            {
                writeOffset = firstOffset;
                Array.Copy(buffer, offset, Buffer, firstOffset, count);
                firstOffset += count;
            }
            else if (firstIndex - secondOffset >= count)
            {
                writeOffset = secondOffset;
                Array.Copy(buffer, offset, Buffer, secondOffset, count);
                secondOffset += count;
            }
            else //缓冲区空间不够，需要申请更大的内存，并进行移位  
            {
                writeOffset = firstOffset;
                int totalSize = firstOffset + count;
                byte[] tmpBuffer = new byte[totalSize];
                Array.Copy(Buffer, 0, tmpBuffer, 0, firstOffset); //复制以前的数据  
                Array.Copy(buffer, offset, tmpBuffer, firstOffset, count); //复制新写入的数据  
                firstOffset += count;
                Buffer = tmpBuffer; //替换  
            }
            return writeOffset;
        }

        public bool ClearBuffer(int offset, int count)
        {
            if (offset == firstIndex)// clear some in first packet
            {
                firstIndex += count;
                if (firstIndex == firstOffset)
                {
                    firstIndex = secondIndex;
                    firstOffset = secondOffset;
                    secondIndex = 0;
                    secondOffset = 0;
                }
                return true;
            }
            else if (offset == secondIndex)// clear some in second packet
            {
                secondIndex += count;
                if (secondIndex == secondOffset)
                {
                    secondIndex = 0;
                    secondOffset = 0;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
