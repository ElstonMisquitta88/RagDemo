using System;
using System.Collections.Generic;
using System.Text;

namespace RagDemo;

public class TextChunker
{
    public static List<string> Split(string text, int chunkSize = 500)
    {
        List<string> chunks = new();

        for (int i = 0; i < text.Length; i += chunkSize)
        {
            int len = Math.Min(chunkSize, text.Length - i);

            chunks.Add(text.Substring(i, len));
        }

        return chunks;
    }
}