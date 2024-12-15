using Rdx.Serialization.Tokenizer.Tokens;

namespace Rdx.Serialization.Parser;

public class RdxParser
{
    public static object Parse(TokensReader tokensReader)
    {
        if (tokensReader.GetTokenType() == TokenType.OpeningBracket)
        {
            return ParsePlex(tokensReader);
        }

        return ParseValue(tokensReader);
    }

    public static object ParsePlex(TokensReader tokensReader)
    {
        var result = new List<object>();
        var bracketType = tokensReader.GetValue();
        var closingBracketType = bracketType switch
        {
            "{" => "}",
            "[" => "]",
            "<" => ">",
            _ => throw new ArgumentOutOfRangeException()
        };

        tokensReader.MoveNext();
        var timestamp = string.Empty;
        if (tokensReader.GetTokenType() == TokenType.Timestamp)
        {
            timestamp = tokensReader.GetValue();
            tokensReader.MoveNext();
        }
        while (true)
        {
            result.Add(tokensReader.GetTokenType() == TokenType.OpeningBracket
                ? ParsePlex(tokensReader)
                : ParseValue(tokensReader));

            if (tokensReader.GetTokenType() != TokenType.ClosingBracket)
            {
                tokensReader.MoveNext();
                continue;
            }

            if (tokensReader.GetValue() != closingBracketType)
            {
                throw new ArgumentOutOfRangeException();
            }

            tokensReader.MoveNext();
            return new ParserRdxPlex(result, timestamp);
        }
    }

    public static object ParseValue(TokensReader tokensReader)
    {
        var value = tokensReader.GetValue();
        tokensReader.MoveNext();
        ParserRdxValue result;
        if (tokensReader.GetTokenType() == TokenType.Timestamp)
        {
            result = new ParserRdxValue(value, tokensReader.GetValue());
            tokensReader.MoveNext();
        }
        else
        {
            result = new ParserRdxValue(value, string.Empty);
        } 
        return result;
    }
}