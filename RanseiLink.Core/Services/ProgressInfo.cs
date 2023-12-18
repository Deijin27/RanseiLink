namespace RanseiLink.Core.Services;
public record ProgressInfo(string? StatusText = null, int? Progress = null, int? MaxProgress = null, bool? IsIndeterminate = null);
