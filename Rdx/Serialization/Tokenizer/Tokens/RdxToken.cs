namespace Rdx.Serialization.Tokenizer.Tokens;

public record RdxToken(int Start, int Length, TokenType TokenType)
{
    public static RdxToken OpeningBracket(int index) => new(index, 1, TokenType.OpeningBracket);
    public static RdxToken ClosingBracket(int index) => new(index, 1, TokenType.ClosingBracket);
    public static RdxToken Colon(int index) => new(index, 1, TokenType.Colon);
    public static RdxToken Comma(int index) => new(index, 1, TokenType.Comma);
    public static RdxToken Value(int index, int length) => new(index, length, TokenType.Value);
    public static RdxToken Timestamp(int index, int length) => new(index, length, TokenType.Timestamp);
    
    public string GetValue(string source) => source[Start..(Start + Length)];
}
