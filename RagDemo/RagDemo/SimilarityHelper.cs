namespace RagDemo;

public class SimilarityHelper
{
    public static double CosineSimilarity(
        float[] vectorA,
        float[] vectorB)
    {
        double dot = 0;
        double magA = 0;
        double magB = 0;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dot += vectorA[i] * vectorB[i];

            magA += vectorA[i] * vectorA[i];

            magB += vectorB[i] * vectorB[i];
        }

        return dot /
        (Math.Sqrt(magA) * Math.Sqrt(magB));
    }
}