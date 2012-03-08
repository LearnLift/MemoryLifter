/**********************************
 Wave concatenation class

 CopyRights Ehab Essa 2006
 from http://www.codeproject.com/KB/graphics/Concatenation_Wave_Files.aspx
***********************************/
using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace MLifter.AudioTools
{
    public class WaveHeader
    {
        public int length;
        public short channels;
        public int samplerate;
        public int DataLength;
        public short BitsPerSample;

        public void WaveHeaderIN(string spath)
        {
            FileStream fs = new FileStream(spath, FileMode.Open, FileAccess.Read);

            BinaryReader br = new BinaryReader(fs);
            length = (int)fs.Length - 8;
            fs.Position = 22;
            channels = br.ReadInt16();
            fs.Position = 24;
            samplerate = br.ReadInt32();
            fs.Position = 34;

            BitsPerSample = br.ReadInt16();
            DataLength = (int)fs.Length - 44;
            br.Close();
            fs.Close();

        }
        public void WaveHeaderOUT(string sPath)
        {
            FileStream fs = new FileStream(sPath, FileMode.Create, FileAccess.Write);

            BinaryWriter bw = new BinaryWriter(fs);
            fs.Position = 0;
            bw.Write(new char[4] { 'R', 'I', 'F', 'F' });

            bw.Write(length);

            bw.Write(new char[8] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });

            bw.Write((int)16);

            bw.Write((short)1);
            bw.Write(channels);

            bw.Write(samplerate);

            bw.Write((int)(samplerate * ((BitsPerSample * channels) / 8)));

            bw.Write((short)((BitsPerSample * channels) / 8));

            bw.Write(BitsPerSample);

            bw.Write(new char[4] { 'd', 'a', 't', 'a' });
            bw.Write(DataLength);
            bw.Close();
            fs.Close();
        }
    }
}
