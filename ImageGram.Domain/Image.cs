using Microsoft.AspNetCore.Http;

namespace ImageGram.Domain;

public record Image(IFormFile imageFile, string imageFilename);
