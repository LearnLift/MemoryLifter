/**********************************
 Wave concatenation class

 CopyRights Ehab Essa 2006
 from http://www.codeproject.com/KB/graphics/Concatenation_Wave_Files.aspx
***********************************/
using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace MLifterAudioBookGenerator.Audio
{
    public class WaveCat
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
        public void Merge(string[] files, string outfile)
        {
            WaveCat wa_IN = new WaveCat();
            WaveCat wa_out = new WaveCat();

            wa_out.DataLength = 0;
            wa_out.length = 0;


            //Gather header data
            foreach (string path in files)
            {
                wa_IN.WaveHeaderIN(@path);
                wa_out.DataLength += wa_IN.DataLength;
                wa_out.length += wa_IN.length;

            }

            //Recontruct new header
            wa_out.BitsPerSample = wa_IN.BitsPerSample;
            wa_out.channels = wa_IN.channels;
            wa_out.samplerate = wa_IN.samplerate;
            wa_out.WaveHeaderOUT(@outfile);

            foreach (string path in files)
            {
                FileStream fs = new FileStream(@path, FileMode.Open, FileAccess.Read);
                byte[] arrfile = new byte[fs.Length - 44];
                fs.Position = 44;
                fs.Read(arrfile, 0, arrfile.Length);
                fs.Close();

                FileStream fo = new FileStream(@outfile, FileMode.Append, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(fo);
                bw.Write(arrfile);
                bw.Close();
                fo.Close();
            }
        }
    }
}
