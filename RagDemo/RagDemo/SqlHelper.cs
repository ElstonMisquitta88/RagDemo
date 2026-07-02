using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace RagDemo;

public class SqlHelper
{
    private readonly string cs;

    public SqlHelper(string connectionString)
    {
        cs = connectionString;
    }

    public void Insert(string filename, int chunkCount, string chunk, float[] embedding)
    {
        using SqlConnection con = new(cs);

        con.Open();

        SqlCommand cmd = new(
        @"INSERT INTO DocumentChunks
        (
            Filename,
            ChunkCount,
            ChunkText,
            Embedding
        )
        VALUES
        (
            @Filename,
            @ChunkCount,
            @Chunk,
            @Embedding
        )", con);

        cmd.Parameters.AddWithValue("@Filename", filename);
        cmd.Parameters.AddWithValue("@ChunkCount", chunkCount);
        cmd.Parameters.AddWithValue("@Chunk", chunk);
        cmd.Parameters.AddWithValue(
            "@Embedding",
            JsonSerializer.Serialize(embedding));

        cmd.ExecuteNonQuery();
    }

    public List<Chunk> GetAll()
    {
        List<Chunk> list = new();

        using SqlConnection con = new(cs);

        con.Open();

        SqlCommand cmd = new(
            "select * from DocumentChunks",
            con);

        SqlDataReader dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            list.Add(new Chunk
            {
                Id = (int)dr["Id"],

                FileName = dr["FileName"].ToString(),
                ChunkText = dr["ChunkText"].ToString(),

                Embedding =
                    JsonSerializer.Deserialize<float[]>(
                        dr["Embedding"].ToString())
            });
        }

        return list;
    }
}