using Serilog;
using Serilog.Enrichers.Sensitive;
using SerilogExamplesAPI.SerilogOperators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.WithSensitiveDataMasking(options =>
        {
            options.MaskValue = "****";
            options.MaskProperties = new List<string> { "CUSTOM_PROPERTY" };
            options.Mode = MaskingMode.Globally; 
            options.MaskingOperators = new List<IMaskingOperator>
            {
                new CustomizedEmailAddressMaskingOperator(),
                new IbanMaskingOperator(),
                new PhoneNumberMaskingOperator()
            };
        })
);

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();