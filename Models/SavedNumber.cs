namespace AdminApi.Models;

public class SavedNumber {
    public int SavedNumberId { get; set; }
    public int Value { get; set; }
    public bool IsSuspicious { get; set; } = default;
    public bool IsBanned { get; set; } = default;
}