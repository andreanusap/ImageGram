using Microsoft.AspNetCore.Http;

namespace ImageGram.Application.RequestModels;

public record PostRequestModel(string userId, string caption, IFormFile imageFile);