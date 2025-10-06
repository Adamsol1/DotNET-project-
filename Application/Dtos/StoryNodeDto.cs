namespace DOTNET_PROJECT.Application.Dtos;

public sealed class StoryNodeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BackgroundUrl { get; set; } = string.Empty;
}

public sealed class CreateStoryNodeDto {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BackgroundUrl { get; set; } = string.Empty;
}

public sealed class UpdateStoryNodeDto {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BackgroundUrl { get; set; } = string.Empty;
}