using System;
using System.IO;

namespace WaveAnalyzer
{
    public static class WavReader
    {
        private static readonly char[] CHAR_DATA = {'d', 'a', 't', 'a'};
        private static readonly char[] CHAR_RIFF = {'R', 'I', 'F', 'F'};
        private static readonly char[] CHAR_WAVE = {'W', 'A', 'V', 'E'};
        private static readonly char[] CHAR_FMT_ = {'f', 'm', 't', ' '};

        public static float[][] Read(string filename, out WavHeader header)
        {
            byte[] data = File.ReadAllBytes(filename);
            return Read(data, out header);
        }

        public static float[][] Read(byte[] data, out WavHeader header)
        {
            var offset = ReadHeader(data, out header);
            var channels = ReadChannels(data, offset, ref header);
            return channels;
        }

        private static float[][] ReadChannels(byte[] data, int offset, ref WavHeader header)
        {
            var bytePerSample = header.BitsPerSample / 8;
            var channelSize = header.FileSize / header.Channels;
            var channels = new float[header.Channels][];

            if (header.Channels == 2)
            {
                int i = 0;
                channels[0] = new float[channelSize];
                channels[1] = new float[channelSize];
                while (offset < data.Length)
                {
                    channels[0][i++] = bytesToFloat(data, offset, bytePerSample);
                    offset += bytePerSample;
                    channels[1][i++] = bytesToFloat(data, offset, bytePerSample);
                    offset += bytePerSample;
                }
            }
            else if (header.Channels == 1)
            {
                int i = 0;
                channels[0] = new float[channelSize];
                while (offset < data.Length)
                {
                    channels[0][i++] = bytesToFloat(data, offset, bytePerSample);
                    offset += bytePerSample;
                }
            }
            else
            {
                throw new FileLoadException($"Unsupported channel size: {header.Channels}");
            }

            return channels;
        }

        private static int ReadHeader(byte[] data, out WavHeader header)
        {
            int offset = 0;
            header = new WavHeader();

            // 0+4: RIFF
            // 4+4: ChunkSize
            // 8+4: WAVE
            // 12+4: SubChunk1 ID: "fmt "
            // 16+4: Lenght of above
            // 20+2: Audio Format
            // 22+2: Channels
            // 24+4: SamplingRate
            // 28+4: ByteRate
            // 32+2: BlockAlign
            // 34+2: BitsPerSample
            // 36+4: data
            // 40+4: FileSize

            // ChunkID
            if (!isChar4(data, CHAR_RIFF, offset))
            {
                throw new FileLoadException($"File is not RIFF format");
            }

            header.ChunkID = bytesToChar4(data, offset);
            offset += 4;

            // ChunkSize
            header.ChunkSize = bytesToInt(data, offset);
            offset += 4;

            // FileFormat
            if (!isChar4(data, CHAR_WAVE, offset))
            {
                throw new FileLoadException("File is not WAVE format");
            }

            header.Format = new char[]
            {
                (char) data[offset + 0],
                (char) data[offset + 1],
                (char) data[offset + 2],
                (char) data[offset + 3],
            };

            offset += 4;

            // Subchunk1 ID
            if (!isChar4(data, CHAR_FMT_, offset))
            {
                throw new FileLoadException("File is not fmt SubChunkID");
            }

            header.SubChunk1ID = bytesToChar4(data, offset);
            offset += 4;

            // Length
            header.SubChunk1Size = bytesToInt(data, offset);
            offset += 4;

            // AudioFormat
            header.AudioFormat = bytesToShort(data, offset);
            offset += 2;

            if (header.AudioFormat != 1)
            {
                throw new FileLoadException($"File is not PCM format: {header.AudioFormat}");
            }

            // Channels
            header.Channels = bytesToShort(data, offset);
            offset += 2;

            // Sampling Rate
            header.SamplingRate = bytesToInt(data, offset);
            offset += 4;

            // (SamplingRate * BitsPerSample * Channels)/8
            header.ByteRate = bytesToInt(data, offset);
            offset += 4;

            // (BitsPerSample*Channels)/8
            header.BlockAlign = bytesToShort(data, offset);
            offset += 2;

            // BitsPerSample
            header.BitsPerSample = bytesToShort(data, offset);
            offset += 2;

            // SubChunkID: "data"
            char[] subChunkID = null;
            int subChunkSize = -1;
            // NOTE: skip chunks while subChunkID 'data' is not found
            while (offset < data.Length - 8)
            {
                subChunkID = bytesToChar4(data, offset);
                offset += 4;

                subChunkSize = bytesToInt(data, offset);
                offset += 4;

                if (isChar4(subChunkID, CHAR_DATA))
                {
                    break;
                }

                offset += subChunkSize;
            }

            header.SubChunk2ID = subChunkID;
            header.SubChunk2Size = subChunkSize;

            return offset;
        }

        private static int bytesToInt(byte[] data, int offset) =>
            data[offset] +
            (data[offset + 1] << 8) +
            (data[offset + 2] << 16) +
            (data[offset + 3] << 24);

        private static short bytesToShort(byte[] data, int offset) =>
            (short) (data[offset] + (data[offset + 1] << 8));

        private static char[] bytesToChar4(byte[] data, int offset) => new[]
        {
            (char) data[offset + 0],
            (char) data[offset + 1],
            (char) data[offset + 2],
            (char) data[offset + 3],
        };

        private static bool isChar4(byte[] data, char[] chars, int offset) =>
            data[offset] == chars[0] &&
            data[offset + 1] == chars[1] &&
            data[offset + 2] == chars[2] &&
            data[offset + 3] == chars[3];

        private static bool isChar4(char[] data, char[] chars) =>
            data[0] == chars[0] &&
            data[1] == chars[1] &&
            data[2] == chars[2] &&
            data[3] == chars[3];

        private static float bytesToFloat(byte[] data, int offset, int bytePerSample)
        {
            switch (bytePerSample)
            {
                case 1:
                    return data[offset] / 128f;
                case 2:
                    if (!BitConverter.IsLittleEndian)
                    {
                        byte[] bytes = {data[offset + 1], data[offset]};
                        return BitConverter.ToInt16(bytes, offset) / 32768f;
                    }

                    return BitConverter.ToInt16(data, offset) / 32768f;
                case 4:
                    if (!BitConverter.IsLittleEndian)
                    {
                        byte[] bytes = {data[3], data[2], data[1], data[0]};
                        return BitConverter.ToInt32(bytes, offset) / 2147483648f;
                    }

                    return BitConverter.ToInt32(data, offset) / 2147483648f;
                default:
                    throw new ArgumentException($"Unsupported BytePerSample: {bytePerSample}");
            }
        }
    }
}