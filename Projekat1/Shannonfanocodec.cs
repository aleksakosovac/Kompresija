namespace Projekat1
{
    static class ShannonFanoCodec
    {
        public static void Encode(string inputPath, string outputPath)
        {
            Codec.Encode(inputPath, outputPath, ShannonFano.BuildLengths);
        }

        public static void Decode(string inputPath, string outputPath)
        {
            Codec.Decode(inputPath, outputPath);
        }
    }
}