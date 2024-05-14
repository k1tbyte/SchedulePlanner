namespace SchedulePlanner.Converters;

public class ToBooleanConverter() : BaseBooleanConverter<bool>(true, false);
public class InvertedToBooleanConverter() : BaseBooleanConverter<bool>(false, true);