using UglyToad.PdfPig;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using OpenAI.Chat;


namespace RagDemo;

public class Program
{


    static async Task Main(string[] args)
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


        string OpenAI_model = configuration.GetValue<string>("OpenAI:Model") ?? throw new Exception("OpenAI:Model not found.");
        string OpenAI_apiKey = configuration.GetValue<string>("OpenAI:ApiKey") ?? throw new Exception("OpenAI:ApiKey not found.");


        //(1)
        //EmbeedNewFile(sql, embedding);

        //(2)
        await AnswerMyQuestion(sql, embedding, OpenAI_model, OpenAI_apiKey);

    }

    private static async Task AnswerMyQuestion(SqlHelper sql, EmbeddingService embedding, string OpenAI_model, string OpenAI_apiKey)
    {
        Console.Write("Ask Question : ");
        string question = Console.ReadLine()!;


        List<SearchResult> results = AskQuestionToRAG(question, sql, embedding);
        string context = string.Join(
            Environment.NewLine + Environment.NewLine,
            results.Select(x =>
                $"""
                Source: {x.FileName}

                {x.ChunkText}
                """));


        string prompt = $"""
            You are an AI assistant for stock exchange circulars.

            Instructions:
            - Answer ONLY using the information provided in the Context.
            - Do NOT make assumptions or use outside knowledge.
            - If the answer is not present in the Context, reply:
              "I don't have enough information in the provided documents."
            - Be clear, concise and professional.
            - If applicable, mention the source document.

            Context:
            {context}

            Question:
            {question}

            Answer:
            """;


        ChatClient client = new ChatClient(OpenAI_model, OpenAI_apiKey);
        ChatCompletion completion =
            await client.CompleteChatAsync(prompt);

        Console.WriteLine("Answer From GPT  : ");
        Console.WriteLine(completion.Content[0].Text);
    }

    private static List<SearchResult> AskQuestionToRAG(string question, SqlHelper sql, EmbeddingService embedding)
    {
        const int TopK = 3;
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
          .Take(TopK)
          .ToList();

        //Console.WriteLine();
        //foreach (var item in result)
        //{
        //    Console.WriteLine($"Score: {item.SimilarityScore}, File: {item.FileName}");
        //    Console.WriteLine(item.ChunkText);
        //    Console.WriteLine("--------------------------------");
        //}
        return result;
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
