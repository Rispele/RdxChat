namespace Rdx.Serialization.Tokenizer.Tokens;

public record RdxToken(int Start, int Length, TokenType TokenType)
{
    public static RdxToken OpeningBracket(int index)
    {
        return new RdxToken(index, 1, TokenType.OpeningBracket);
    }

    public static RdxToken ClosingBracket(int index)
    {
        return new RdxToken(index, 1, TokenType.ClosingBracket);
    }

    public static RdxToken Colon(int index)
    {
        return new RdxToken(index, 1, TokenType.Colon);
    }

    public static RdxToken Comma(int index)
    {
        return new RdxToken(index, 1, TokenType.Comma);
    }

    public static RdxToken Value(int index, int length)
    {
        return new RdxToken(index, length, TokenType.Value);
    }

    public static RdxToken TimestampMarker(int index)
    {
        return new RdxToken(index, 1, TokenType.TimestampMarker);
    }

    public string GetValue(string source)
    {
        return source[Start..(Start + Length)];
    }
}