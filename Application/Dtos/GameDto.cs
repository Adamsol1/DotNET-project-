namespace DOTNET_PROJECT.Application.Dtos;

public sealed class GameStateDto
{
    public int PlayerCharacterId { get; set; }
    public int CurrentStoryNodeId { get; set; }   
}

public sealed class MakeChoiceDto
{
    public int PlayerCharacterId { get; set; }
    public int ChoiceId { get; set; }
}

public sealed class SaveProgressDto
{
    public int PlayerCharacterId { get; set; }
    public int CurrentStoryNodeId { get; set; }
}

public sealed class MoveToNextNodeDto
{
    public int PlayerCharacterId { get; set; }
    public int CurrentStoryNodeId { get; set; }
}

public sealed class MoveToPreviousNodeDto
{
    public int PlayerCharacterId { get; set; }
    public int PreviousStoryNodeId { get; set; }
}
