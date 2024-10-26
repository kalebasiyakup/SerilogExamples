using System.Text.RegularExpressions;
using Serilog.Enrichers.Sensitive;

namespace SerilogExamplesAPI.SerilogOperators;

public class CustomizedEmailAddressMaskingOperator : EmailAddressMaskingOperator
{
    protected override string PreprocessMask(string mask, Match match)
    {
        var parts = match.Value.Split('@');

        return mask + "@" + parts[1];
    }
}