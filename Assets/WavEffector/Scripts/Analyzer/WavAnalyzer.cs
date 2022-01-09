namespace WaveAnalyzer.Analyzer
{
    public static class WavAnalyzer
    {
        public static float DetectVolumeLevel(float[] data, int start, int end)
        {
            var max = 0f;
            var min = 0f;

            for (var i = start; i < end; i++)
            {
                if (max < data[i]) max = data[i];
                if (min > data[i]) min = data[i];
            }

            return max - min;
        }
    }
}