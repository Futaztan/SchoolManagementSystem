namespace Shared.DTOs;

public class GradeBatchUpdateDto
{
    public required List<GradeValueRecord> UpdatedIdsAndValues { get; set; }
    public required List<int> DeletedIds { get; set; }
}
public record GradeValueRecord(int GradeId, int GradeValue);
