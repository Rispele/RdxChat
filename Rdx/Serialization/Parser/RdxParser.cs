using Rdx.Serialization.Tokenizer.Tokens;

namespace Rdx.Serialization.Parser;

public class RdxParser
{
    private readonly TokensReader tokensReader;

    public RdxParser(TokensReader tokensReader)
    {
        this.tokensReader = tokensReader;
    }

    public object Parse(PlexType? outerPlexType = null)
    {
        if (IsOutOfBracesTuple(outerPlexType)) return ParseOutOfBracesTuple();

        return tokensReader.GetTokenType() == TokenType.OpeningBracket
            ? ParsePlex()
            : ParseValue();
    }

    private ParserRdxPlex ParsePlex()
    {
        var result = new List<object>();
        var braceType = tokensReader.GetValueAndMoveNext();
        var timestamp = ParseTimestampIfExists();

        var closingBracketType = ResolveClosingBracket(braceType);
        var plexType = ResolvePlexType(braceType);
        while (true)
        {
            result.Add(Parse(plexType));

            if (tokensReader.GetTokenType() != TokenType.ClosingBracket)
            {
                tokensReader.MoveNext();
                continue;
            }

            if (tokensReader.GetValue() != closingBracketType)
                throw new InvalidOperationException("closing bracket mismatch");

            tokensReader.MoveNext();
            return new ParserRdxPlex(plexType, result, timestamp);
        }
    }

    private bool IsOutOfBracesTuple(PlexType? outerPlexType)
    {
        if (outerPlexType == PlexType.XPles) return false;

        var nextTokenPosition = GetTokenNextToCurrentObject();

        return tokensReader.HasToken(nextTokenPosition)
         && tokensReader.GetTokenType(nextTokenPosition) == TokenType.Colon;
    }

    private int GetTokenNextToCurrentObject()
    {
        if (tokensReader.GetTokenType() == TokenType.OpeningBracket)
        {
            var offset = 1;
            for (var bracesCount = 1; bracesCount > 0; offset++)
            {
                var tokenType = tokensReader.GetTokenType(offset);
                bracesCount += tokenType switch
                {
                    TokenType.OpeningBracket => 1,
                    TokenType.ClosingBracket => -1,
                    _ => 0
                };
            }

            return offset + 1;
        }

        if (tokensReader.GetTokenType() != TokenType.Value)
            throw new InvalidOperationException("unexpected token type");

        return tokensReader.HasToken(1) && tokensReader.GetTokenType(1) == TokenType.TimestampMarker
            ? 3
            : 1;
    }

    private ParserRdxPlex ParseOutOfBracesTuple()
    {
        const PlexType plexType = PlexType.XPles;

        var result = new List<object>();
        while (true)
        {
            result.Add(Parse(plexType));
            if (tokensReader.GetTokenType() != TokenType.Colon) return new ParserRdxPlex(plexType, result, null);
            tokensReader.MoveNext();
        }
    }

    private static string ResolveClosingBracket(string bracketType)
    {
        return bracketType switch
        {
            "{" => "}",
            "[" => "]",
            "<" => ">",
            _ => throw new ArgumentOutOfRangeException(nameof(bracketType))
        };
    }

    private static PlexType ResolvePlexType(string bracketType)
    {
        return bracketType switch
        {
            "{" or "}" => PlexType.Eulerian,
            "[" or "]" => PlexType.Linear,
            "<" or ">" => PlexType.XPles,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private ParserRdxValue ParseValue()
    {
        return new ParserRdxValue(tokensReader.GetValueAndMoveNext(), ParseTimestampIfExists());
    }

    private string? ParseTimestampIfExists()
    {
        if (tokensReader.GetTokenType() != TokenType.TimestampMarker) return null;

        tokensReader.MoveNext();
        return tokensReader.GetValueAndMoveNext();
    }
}