using System;
using System.Windows.Ink;

namespace IMOOC.EduCourses.WhiteBoardModule
{
    public class CustomStrokes
    {
        private StrokeCollection strokes;
        private byte[] byteStrokes;
        private int destionationIndex;

        public CustomStrokes(StrokeCollection strokes)
        {
            this.strokes = strokes;
        }

        public byte[] GetBuffer()
        {
            if (strokes != null)
            {
                int totalLength = sizeof(int); //strokes.Count
                totalLength += sizeof(int) * strokes.Count;//stylusPoints.Count
                for (int i = 0; i < strokes.Count; i++)
                {
                    totalLength += sizeof(double) * 2 * strokes[i].StylusPoints.Count;//stylusPoints.x.y

                }

                byteStrokes = new byte[totalLength];
                destionationIndex = 0;

                WriteInt(strokes.Count);
                for (int i = 0; i < strokes.Count; i++)
                {
                    var stylusPoints = strokes[i].StylusPoints;
                    WriteInt(stylusPoints.Count);
                    for (int j = 0; j < stylusPoints.Count; j++)
                    {
                        WriteDouble(stylusPoints[j].X);
                        WriteDouble(stylusPoints[j].Y);
                    }
                }

                return byteStrokes;
            }
            else
            {
                return null;
            }
        }


        private void WriteInt(int value)
        {
            byte[] tmpBuffer = BitConverter.GetBytes(value);
            WriteBuffer(tmpBuffer, 0, tmpBuffer.Length);
        }

        private void WriteDouble(double value)
        {
            byte[] tmpBuffer = BitConverter.GetBytes(value);
            WriteBuffer(tmpBuffer, 0, tmpBuffer.Length);
        }
        
        private void WriteBuffer(byte[] buffer,int offset,int count)
        {
            Array.Copy(buffer, offset, byteStrokes, destionationIndex, count);
            destionationIndex += count;
        }
    }
}
