# RAG Demo using C#, SQL Server, Local Embeddings & OpenAI

## Overview


<img width="1462" height="502" alt="RAG_output" src="https://github.com/user-attachments/assets/c7a91477-5607-4d44-8a29-89b225df9504" />


This project demonstrates a **Retrieval-Augmented Generation (RAG)**
application built using **C# (.NET)**.

The application reads PDF documents, converts them into embeddings using
a **local ONNX embedding model (AllMiniLM)**, stores the embeddings in
**SQL Server**, performs **semantic search** using **Cosine
Similarity**, and finally uses **OpenAI GPT** to generate a natural
language answer based only on the retrieved document content.

This project was built to understand the complete RAG pipeline instead
of relying on managed services such as Azure AI Foundry.

------------------------------------------------------------------------

# Architecture

``` text
                    +-------------------+
                    |     PDF File      |
                    +---------+---------+
                              |
                              v
                     Read PDF Content
                              |
                              v
                     Split into Chunks
                              |
                              v
                 Generate Embeddings (ONNX)
                   (AllMiniLM Local Model)
                              |
                              v
                   Store in SQL Server
          +----------------------------------+
          | File Name                        |
          | Chunk Number                     |
          | Chunk Text                       |
          | Embedding Vector                 |
          +----------------------------------+

=========================================================

                  User asks a Question
                              |
                              v
             Generate Question Embedding
                              |
                              v
              Cosine Similarity Comparison
                              |
                              v
             Retrieve Top Matching Chunks
                              |
                              v
                 Build Prompt + Context
                              |
                              v
                      OpenAI GPT Model
                              |
                              v
                Natural Language Response
```

------------------------------------------------------------------------

# Technologies Used

  Technology          Purpose
  ------------------- ------------------------------------------
  C# (.NET)           Application Development
  SQL Server          Store document chunks and embeddings
  ONNX Runtime        Execute local embedding model
  AllMiniLM           Local embedding generation
  UglyToad.PdfPig     Read PDF documents
  OpenAI API          Generate final natural language response
  Cosine Similarity   Semantic Search

------------------------------------------------------------------------

# Project Flow

## 1. Read PDF

The application reads the complete PDF document using **PdfPig**.

## 2. Split into Chunks

The document is divided into meaningful chunks to improve retrieval
accuracy.

## 3. Generate Embeddings

Each chunk is converted into an embedding vector using the local
**AllMiniLM ONNX** model.

## 4. Store in SQL Server

Each chunk is stored with: - File Name - Chunk Number - Chunk Text -
Embedding Vector

## 5. User Question

The user's question is converted into an embedding.

## 6. Semantic Search

Cosine Similarity is used to compare the question embedding with all
stored embeddings.

The top matching chunks are retrieved.

## 7. Prompt Construction

The retrieved chunks become the context sent to OpenAI.

Example:

``` text
Context:
<Retrieved Chunks>

Question:
<User Question>
```

## 8. Response Generation

OpenAI GPT generates a grounded answer using only the supplied context.

------------------------------------------------------------------------

# SQL Schema

``` sql
CREATE TABLE DocumentChunks
(
    Id INT IDENTITY PRIMARY KEY,
    FileName NVARCHAR(255),
    ChunkNumber INT,
    ChunkText NVARCHAR(MAX),
    Embedding NVARCHAR(MAX)
);
```

------------------------------------------------------------------------

# Project Structure

``` text
RagDemo
│
├── Data
│   ├── Model.onnx
│   └── vocab.txt
│
├── Docs
│
├── EmbeddingService.cs
├── PdfReader.cs
├── SearchResult.cs
├── SimilarityHelper.cs
├── SqlHelper.cs
├── TextChunker.cs
├── Program.cs
└── appsettings.json
```

------------------------------------------------------------------------

# Sample Workflow

``` text
PDF
 ↓
Chunk
 ↓
Embedding
 ↓
SQL Server
 ↓
Question
 ↓
Question Embedding
 ↓
Cosine Similarity
 ↓
Top 3 Chunks
 ↓
OpenAI GPT
 ↓
Grounded Answer
```

------------------------------------------------------------------------

# Sample Output

``` text
Ask Question:
INE0GMR01016

Answer:
INE0GMR01016 corresponds to equity shares allotted pursuant to conversion of warrants issued on a preferential basis...

Source:
CML75019.pdf
```

------------------------------------------------------------------------

# Key Concepts

-   Retrieval-Augmented Generation (RAG)
-   Local Embeddings (AllMiniLM)
-   ONNX Runtime
-   SQL Server
-   Cosine Similarity
-   Semantic Search
-   Prompt Engineering
-   OpenAI GPT
-   Context Grounding

------------------------------------------------------------------------

# Future Enhancements

-   Page number support
-   Paragraph-based chunking
-   Similarity threshold
-   SQL Server native vector search
-   ASP.NET Core Web API
-   React / Blazor UI
-   Azure AI Search
-   Azure OpenAI
-   Hybrid Search (Keyword + Vector Search)

------------------------------------------------------------------------

# Learning Outcomes

This project demonstrates:

-   Reading PDF documents
-   Chunking large documents
-   Generating local embeddings
-   Storing embeddings in SQL Server
-   Performing semantic retrieval using Cosine Similarity
-   Grounding an LLM with retrieved context
-   Producing accurate, source-backed answers

------------------------------------------------------------------------

## Author

**Elston Misquitta**

### Tech Stack

-   C#
-   .NET
-   SQL Server
-   ONNX Runtime
-   OpenAI
-   RAG
-   Semantic Search
