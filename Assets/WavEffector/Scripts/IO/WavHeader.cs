using System.Text;

namespace WaveAnalyzer
{
    public struct WavHeader
    {
        // Header Structure
        // 0+4: ChunkID: "RIFF"
        // 4+4: ChunkSize
        // 8+4: Format. "WAVE"

        // 12+4: SubChunk1 ID: "fmt "
        // 16+4: SubChunk1 Size: 16:PCM
        // 20+2: Audio Format: 1:PCM
        // 22+2: Channels: 1 or 2
        // 24+4: SamplingRate: e.g. 44100
        // 28+4: ByteRate
        // 32+2: BlockAlign
        // 34+2: BitsPerSample: e.g. 16

        // some other chunk might be inserted here. such as "LIST"

        // 36+4: SubChunk2 ID: "data"
        // 40+4: SubChunk2 Size: FileSize
        // 44+x: wave data

        public char[] ChunkID; // "RIFF"
        public int ChunkSize;
        public char[] Format; // "WAVE"
        public char[] SubChunk1ID; // "fmt "
        public int SubChunk1Size; // format data size
        public short AudioFormat; // 1: PCM
        public short Channels; // 1 or 2
        public int SamplingRate; // 44100, 48000
        public int ByteRate;
        public short BlockAlign;
        public short BitsPerSample; // e.g. 16
        public char[] SubChunk2ID; // "data"
        public int SubChunk2Size; // file size

        public int FormatDataSize => SubChunk1Size;
        public int FileSize => SubChunk2Size;

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder
                .Append("ChunkID:").Append(ChunkID).Append("\t")
                .Append("ChunkSize:").Append(ChunkSize).Append("\t")
                .Append("Format:").Append(Format).Append("\t")
                .Append("SubChunkID:").Append(SubChunk1ID).Append("\t")
                .Append("SubChunk1Size:").Append(SubChunk1Size).Append("\t")
                .Append("AudioFormat:").Append(AudioFormat).Append("\t")
                .Append("Channels:").Append(Channels).Append("\t")
                .Append("SamplingRate:").Append(SamplingRate).Append("\t")
                .Append("ByteRate:").Append(ByteRate).Append("\t")
                .Append("BlockAlign:").Append(BlockAlign).Append("\t")
                .Append("BitsPerSample:").Append(BitsPerSample).Append("\t")
                .Append("SubChunk2ID:").Append(SubChunk2ID).Append("\t")
                .Append("SubChunk2Size:").Append(SubChunk2Size).Append("\t")
                ;
            return builder.ToString();
        }
    }
}