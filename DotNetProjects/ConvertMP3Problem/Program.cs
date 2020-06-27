using NAudio.Lame;
using NAudio.Wave;
using NLayer.NAudioSupport;
using System;
using System.IO;

namespace ConvertMP3Problem
{
    class Program
    {
        const string _dir = @"c:\temp\";

        // 8,000 Hz 16 bit PCM, get this one from http://www.nch.com.au/acm/8k16bitpcm.wav (link from https://en.wikipedia.org/wiki/WAV)
        const string _origWaveFilePath = _dir + "8k16bitpcm.wav";
        const int _mp3BitRate = 32;

        static void Main(string[] args)
        {
            var stereo = MakeStereo(_origWaveFilePath);
            Console.WriteLine(stereo);
            var mp3 = WaveToMP3WithLame(stereo);
            Console.WriteLine(mp3);
            Console.WriteLine(MP3ToWaveWithNLayer(mp3));
        }

        static string MakeStereo(string waveFilePath)
        {
            var stereoWaveFilePath = Path.Combine(_dir, Path.GetFileNameWithoutExtension(waveFilePath) + "_stereo.wav");
            using var reader = new WaveFileReader(waveFilePath);
            var multiplexer = new MultiplexingWaveProvider(new WaveStream[] { reader }, 2);
            using var writer = File.OpenWrite(stereoWaveFilePath);
            WaveFileWriter.WriteWavFileToStream(writer, multiplexer);
            return stereoWaveFilePath;
        }

        static string WaveToMP3WithLame(string waveFilePath)
        {
            var mp3FilePath = Path.Combine(_dir, Path.GetFileNameWithoutExtension(waveFilePath) + ".mp3");
            using var reader = new WaveFileReader(waveFilePath);
            using var writer = new LameMP3FileWriter(mp3FilePath, reader.WaveFormat, _mp3BitRate);
            reader.CopyTo(writer);
            return mp3FilePath;
        }

        static string MP3ToWaveWithNLayer(string mp3FilePath)
        {
            var waveFilePath = Path.Combine(_dir, Path.GetFileNameWithoutExtension(mp3FilePath) + "_nlayer.wav");
            var builder = new Mp3FileReader.FrameDecompressorBuilder(wf => new Mp3FrameDecompressor(wf));
            using var reader = new Mp3FileReader(mp3FilePath, builder);
            using var writer = new WaveFileWriter(waveFilePath, reader.WaveFormat);
            reader.CopyTo(writer);
            return waveFilePath;
        }
    }
}
