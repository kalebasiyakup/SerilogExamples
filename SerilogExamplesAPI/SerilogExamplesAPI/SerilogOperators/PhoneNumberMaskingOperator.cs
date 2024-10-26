using System.Text.RegularExpressions;
using Serilog.Enrichers.Sensitive;

namespace SerilogExamplesAPI.SerilogOperators;

public class PhoneNumberMaskingOperator : RegexMaskingOperator
{
    private const string PhonePattern =
        @"(?:\+?\d{1,3})?[-.\s]?(\(?\d{3}\)?)?[-.\s]?(\d{3})[-.\s]?(\d{4})";

    public PhoneNumberMaskingOperator() : base(PhonePattern, RegexOptions.IgnoreCase | RegexOptions.Compiled)
    {
    }

    protected override string PreprocessInput(string input)
    {
        return input;
    }

    protected override string PreprocessMask(string mask, Match match)
    {
        string lastThreeDigits = match.Value.Length >= 3 ? match.Value[^3..] : match.Value;
        string maskedPart = new string('*', match.Value.Length - 3);

        return maskedPart + lastThreeDigits;
    }

    protected override bool ShouldMaskInput(string input)
    {
        return Regex.IsMatch(input, PhonePattern);
    }
}