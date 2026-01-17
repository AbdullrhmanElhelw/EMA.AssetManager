namespace EMA.AssetManager.Services.Dtos.Search;

public class SearchResultDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty; // العنوان الرئيسي (اسم الجهاز / رقم التذكرة)
    public string Description { get; set; } = string.Empty; // وصف فرعي (سيريال / اسم المبلغ)
    public string Type { get; set; } = string.Empty; // نوع النتيجة (Asset, Ticket, Item)
    public string Url { get; set; } = string.Empty; // الرابط اللي هيروح له لما يدوس
    public string Icon { get; set; } = string.Empty; // أيقونة تعبر عن النوع
}
