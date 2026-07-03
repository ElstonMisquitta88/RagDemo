using UglyToad.PdfPig;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace RagDemo;

public class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
         .SetBasePath(AppContext.BaseDirectory)
         .AddJsonFile("appsettings.json", optional: true)
         .AddUserSecrets<Program>()
         .Build();

        string connectionString =
         configuration.GetConnectionString("SqlServer")
         ?? throw new Exception("Connection string not found.");

        SqlHelper sql = new SqlHelper(connectionString);

        var embedding = new EmbeddingService(
        @"D:\GitHub\RagDemo\RagDemo\RagDemo\Data\Model.onnx",
        @"D:\GitHub\RagDemo\RagDemo\RagDemo\Data\vocab.txt");

        //(1)
        //EmbeedNewFile(sql, embedding);

        //(2)
        AskQuestionToRAG(sql, embedding);
    }

    private static void AskQuestionToRAG(SqlHelper sql, EmbeddingService embedding)
    {
        Console.Write("Ask Question : ");

        string question = Console.ReadLine();

        float[] questionVector =
        embedding.Generate(question);

        List<SearchResult> result = sql.GetAll()
          .Select(x => new SearchResult
          {
              ChunkText = x.ChunkText,
              FileName = x.FileName,
              SimilarityScore = SimilarityHelper.CosineSimilarity(
                                  questionVector,
                                  x.Embedding)
          })
          .OrderByDescending(x => x.SimilarityScore)
          .Take(3)
          .ToList();

        Console.WriteLine();

        foreach (var item in result)
        {
            Console.WriteLine($"Score: {item.SimilarityScore}, File: {item.FileName}");
            Console.WriteLine(item.ChunkText);
            Console.WriteLine("--------------------------------");
        }
    }

    private static void EmbeedNewFile(SqlHelper sql, EmbeddingService embedding)
    {
        //string pdf = PdfReader.Read(@"D:\GitHub\RagDemo\RagDemo\RagDemo\Docs\Exchange.pdf");
        string filepath = @"D:\GitHub\RagDemo\RagDemo\RagDemo\Docs\CML75019.pdf";
        string filename = Path.GetFileName(filepath);
        string pdf = PdfReader.Read(filepath);

        var chunks = TextChunker.Split(pdf);
        int _chunkcount = 1;

        foreach (var chunk in chunks)
        {
            float[] vector = embedding.Generate(chunk);

            sql.Insert(filename, _chunkcount, chunk, vector);
            _chunkcount++;
        }
        Console.WriteLine("PDF Indexed.");
        Console.WriteLine();
    }
}
