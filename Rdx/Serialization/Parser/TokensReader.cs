using Rdx.Serialization.Tokenizer.Tokens;

namespace Rdx.Serialization.Parser;

public class TokensReader
{
    private readonly List<RdxToken> tokens;
    private readonly string source;

    private int position = 0;
    private Lazy<string> value;
    
    public TokensReader(List<RdxToken> tokens, string source)
    {
        this.tokens = tokens;
        this.source = source;
        
        value = new Lazy<string>(() => this.tokens[position].GetValue(this.source));
    }

    public TokenType GetTokenType(int offset = 0)
    {
        return tokens[position + offset].TokenType;
    }
    
    public string GetValue(int offset = 0)
    {
        return offset == 0 
            ? value.Value 
            : tokens[position + offset].GetValue(source);
    }

    public void MoveNext()
    {
        position++;
        value = new Lazy<string>(() => tokens[position].GetValue(source));
    }
}