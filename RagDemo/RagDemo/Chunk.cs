using System;
using System.Collections.Generic;
using System.Text;
    
namespace RagDemo
{
    public class Chunk
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public string ChunkText { get; set; }

        public float[] Embedding { get; set; }
    }
}
