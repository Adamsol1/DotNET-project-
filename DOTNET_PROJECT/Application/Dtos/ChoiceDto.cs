namespace DOTNET_PROJECT.Application.Dtos;

public sealed class ChoiceDto 
{
    public int Id { get; set; }
    public int StoryNodeId { get; set; }
    public int NextStoryNodeId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }
    public int HealthEffect { get; set; }
    
}

public sealed class CreateChoiceDto
{
    public int StoryNodeId { get; set; }
    public int NextStoryNodeId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }
    public int HealthEffect { get; set; }
}

public sealed class UpdateChoiceDto
{
    public int Id { get; set; }
    public int StoryNodeId { get; set; }
    public int NextStoryNodeId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }
    public int HealthEffect { get; set; }
}

public sealed class ChoiceResultDto
{
    public int SaveId { get; set; }
    public int ChoiceId { get; set; }
    public int NextStoryNodeId { get; set; }
    public PlayerCharacterDto PlayerCharacter { get; set; } = new PlayerCharacterDto();
    public bool IsDead { get; set; }
    public string? AudioUrl { get; set; }
}
