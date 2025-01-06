using System.Text.RegularExpressions;
using Rdx.Serialization.Tokenizer.Tokens;

namespace Rdx.Serialization.Tokenizer.TokenHandlers;

public interface ITokenHandler
{
    public Regex Regex { get; }

    public int HandleToken(int currentPosition, int beginOfTokenPositions, string jRdx, out RdxToken token);
}