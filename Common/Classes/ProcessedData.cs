namespace Common.Classes;

public readonly record struct ProcessedData<T>(TopicData OriginalData, T Result);