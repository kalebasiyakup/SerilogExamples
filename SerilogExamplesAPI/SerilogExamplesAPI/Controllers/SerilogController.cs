using Microsoft.AspNetCore.Mvc;

namespace SerilogExamplesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SerilogController(ILogger<SerilogController> logger) : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet(Name = "GetLog")]
    public async Task<ActionResult<IEnumerable<string>>> Get()
    {
        // An e-mail address in text
        logger.LogInformation("This is a secret email address: james.bond@universal-exports.co.uk");

        // Works for properties too
        logger.LogInformation("This is a secret email address: {Email}", "james.bond@universal-exports.co.uk");

        // IBANs are also masked
        logger.LogInformation("Bank transfer from Felix Leiter on NL02ABNA0123456789");

        // IBANs are also masked
        logger.LogInformation("Bank transfer from Felix Leiter on {BankAccount}", "NL02ABNA0123456789");

        // Credit card numbers too
        logger.LogInformation("Credit Card Number: 4111111111111111");

        // even works in an embedded XML string
        var x = new
        {
            Key = 12345,
            XmlValue = "<MyElement><CreditCard>4111111111111111</CreditCard></MyElement>"
        };
        logger.LogInformation("Object dump with embedded credit card: {x}", x);

        //GSM Number
        logger.LogInformation("GSM Number: 5325109597");

        //Custom
        logger.LogInformation("This is a CUSTOM_PROPERTY sensitive {CUSTOM_PROPERTY}", "Denemeeeeeee");

        if (!Directory.Exists("Logs"))
        {
            return NotFound("Logs directory not found.");
        }

        try
        {
            var logFiles = Directory.GetFiles("Logs", "*.json");

            var logsContent = await Task.WhenAll(logFiles.Select(async logFile =>
            {
                await using var stream = new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync();
                return new
                {
                    FileName = Path.GetFileName(logFile),
                    Content = content
                };
            }));

            return Ok(logsContent);
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}