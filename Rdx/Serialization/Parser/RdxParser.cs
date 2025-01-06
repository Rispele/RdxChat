using Rdx.Serialization.Tokenizer.Tokens;

namespace Rdx.Serialization.Parser;

public class RdxParser
{
    private readonly TokensReader tokensReader;

    public RdxParser(TokensReader tokensReader)
    {
        this.tokensReader = tokensReader;
    }

    public object Parse()
    {
        return tokensReader.GetTokenType() == TokenType.OpeningBracket
            ? ParsePlex()
            : ParseValue();
    }

    private object ParsePlex()
    {
        var result = new List<object>();
        var bracketType = tokensReader.GetValueAndMoveNext();
        var timestamp = ParseTimestamp();

        var closingBracketType = GetClosingBracket(bracketType);
        while (true)
        {
            var rdxValue = tokensReader.GetTokenType() == TokenType.OpeningBracket
                ? ParsePlex()
                : ParseValue();
            result.Add(rdxValue);

            if (tokensReader.GetTokenType() != TokenType.ClosingBracket)
            {
                tokensReader.MoveNext();
                continue;
            }

            if (tokensReader.GetValue() != closingBracketType)
                throw new InvalidOperationException("closing bracket mismatch");

            tokensReader.MoveNext();
            return new ParserRdxPlex(GetPlexType(bracketType), result, timestamp);
        }
    }

    private static string GetClosingBracket(string bracketType)
    {
        return bracketType switch
        {
            "{" => "}",
            "[" => "]",
            "<" => ">",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static PlexType GetPlexType(string bracketType)
    {
        return bracketType switch
        {
            "{" or "}" => PlexType.Eulerian,
            "[" or "]" => PlexType.Linear,
            "<" or ">" => PlexType.XPles,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private object ParseValue()
    {
        return new ParserRdxValue(tokensReader.GetValueAndMoveNext(), ParseTimestamp());
    }

    private string? ParseTimestamp()
    {
        if (tokensReader.GetTokenType() != TokenType.TimestampMarker) return null;

        tokensReader.MoveNext();
        return tokensReader.GetValueAndMoveNext();
    }
}