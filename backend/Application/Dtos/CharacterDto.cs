namespace backend.Application.Dtos;

// NPC character dto.
public sealed class CharacterDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<DialogueDto> Dialogues { get; set; } = new List<DialogueDto>();
}

public sealed class CreateCharacterDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

public sealed class UpdateCharacterDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
//TODO: fix to save when implemented

// // Player character dto.
// public sealed class PlayerCharacterDto
// {
//     public int Id { get; set; }
//     public string Name { get; set; } = string.Empty;
//     public int Health { get; set; }
//     public int UserId { get; set; }
//     public int CurrentStoryNodeId { get; set; }
// }
//
// // Create player character dto.
// public sealed class CreatePlayerCharacterDto
// {
//     public string Name { get; set; } = string.Empty;
//     public int UserId { get; set; }
//     public int CurrentStoryNodeId { get; set; } = 1;
//     // Health defaults to 100 in domain model
// }
//
// // Update player character dto.
// public sealed class UpdatePlayerCharacterDto
// {
//     public int Id { get; set; }
//     public string Name { get; set; } = string.Empty;
//     public int Health { get; set; }
//     public int CurrentStoryNodeId { get; set; }
// }