using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
//using Microsoft.SemanticKernel.Connectors.Ollama.Client;
using Microsoft.SemanticKernel.Embeddings;
using OllamaSharp;
using System.IO;
using System.Text;

namespace TSR.ApiService;


public class ImageVectorGenerator
{
    private const string OllamaEndpoint = "http://localhost:11434";
    // NOTE: The model must be capable of generating embeddings AND handling image input.
    private const string EmbeddingModelName = "moondream";

    public async Task<float[]> GenerateVectorFromImage(byte[] imageBytes)
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
        var embeddings = await embeddingService.GenerateEmbeddingsAsync([multimodalPrompt]);

        if (embeddings.Count > 0)
        {
            return embeddings[0].ToArray();
        }

        return Array.Empty<float>();
    }
}