using FaissNet;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Connectors.Ollama;
//using Microsoft.SemanticKernel.Connectors.Ollama.Client;
using Microsoft.SemanticKernel.Embeddings;
using OllamaSharp;
using System.Collections;
using System.IO;
using System.Text;
using TSR.ApiService.Models;

namespace TSR.ApiService;


public class ImageVectorGenerator
{
    private const string OllamaEndpoint = "http://localhost:11434";
    private const string EmbeddingModelName = "moondream";
    private readonly InMemoryVectorStore vectorStore;

    public ImageVectorGenerator()
    {
        vectorStore = new InMemoryVectorStore();
    }

    public async Task<float[]> GenerateVectorFromImage(byte[] imageBytes, string fileName)
    {
        // 1. Initialize the Ollama client
        var ollamaClient = new OllamaApiClient(
            new Uri(OllamaEndpoint),
            EmbeddingModelName
        );

        // 2. Use the recommended extension method to get the embedding generation service
        var embeddingService = ollamaClient.AsEmbeddingGenerationService();

        // 3. Prepare the image content for the model
        string base64Image = Convert.ToBase64String(imageBytes);

        // 4. Create a prompt structure that includes the image data.
        string multimodalPrompt = $"Here is an image: data:image/jpeg;base64,{base64Image} . Generate the most representative embedding vector for this image.";

        // 5. Generate the embeddings
        var embeddings = await embeddingService.GenerateEmbeddingsAsync(new[] { multimodalPrompt });

        if (embeddings is not null && embeddings.Count > 0)
        {
            // Convert IList<ReadOnlyMemory<float>> to float[] for FaissNet.Index.AddFlat
            var firstEmbeddingMemory = embeddings[0];
            float[] firstEmbedding = firstEmbeddingMemory.ToArray();
                        
            FaissNet.Index idx = FaissNet.Index.CreateDefault(firstEmbedding.Length, FaissNet.MetricType.METRIC_INNER_PRODUCT);
            /*
            // If you want to add all embeddings, flatten them into a single float[] array. Otherwise, just add the first embedding
            idx.AddFlat(1, firstEmbedding);
            var tx = idx.SearchFlat(1, firstEmbedding, 1); //, out long[] labels, out float[] distances);
            */
   
            var collection = vectorStore.GetCollection<string, TSRImage>("images");
            await collection.EnsureCollectionExistsAsync();
            TSRImage img = new TSRImage
            {
                FileName = fileName,
                ImageEmbedding = firstEmbedding,
            };
            await collection.UpsertAsync(img);

            

            return firstEmbedding;
        }

        return Array.Empty<float>();
    }
}