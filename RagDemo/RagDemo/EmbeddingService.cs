using AllMiniLmL6V2Sharp;
using AllMiniLmL6V2Sharp.Tokenizer;

namespace RagDemo;

public class EmbeddingService
{
    private readonly AllMiniLmL6V2Embedder embedder;

    public EmbeddingService(string model, string vocab)
    {
        var tokenizer = new BertTokenizer(vocab);

        embedder = new AllMiniLmL6V2Embedder(
            model,
            tokenizer);
    }

    public float[] Generate(string text)
    {
        return embedder.GenerateEmbedding(text).ToArray();
    }
}