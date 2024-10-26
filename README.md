# Serilog Examples - Serilog ve Serilog.Enrichers.Sensitive Kullanımı

Bu proje, **Serilog** ve **Serilog.Enrichers.Sensitive** ile loglama işlemlerinde hassas verilerin maskeleme örneğini içermektedir. Uygulama içinde bazı hassas verilerin (e-posta, telefon numarası, kredi kartı bilgileri vb.) loglanırken gizlenmesi sağlanır.

## Proje Kurulumu

Projeyi klonladıktan sonra, bağımlılıkları yüklemek için aşağıdaki komutu çalıştırın:

```bash
dotnet restore
```

Ardından projeyi başlatabilirsiniz:

```bash
dotnet run
```

## Kullanılan Kütüphaneler

- **Serilog**: .NET uygulamalarında esnek ve yapılandırılabilir loglama çözümü sunar.
- **Serilog.Enrichers.Sensitive**: Serilog ile hassas verilerin maskelenmesini sağlar.

## Yapılandırma

Hassas verilerin maskeleme ayarları `appsettings.json` dosyasında tanımlanmıştır. Dosyaya örnek olarak aşağıdaki yapılandırma eklenmiştir:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "Logs\\log-development..json",
          "rollingInterval": "Day",
          "rollOnFileSizeLimit": true,
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ],
    "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithProcessId",
      "WithThreadId",
      "WithClientIp"
    ],
    "Properties": {
      "Application": "Your ASP.NET Core App",
      "Environment": "Development"
    }
  },
  "AllowedHosts": "*"
}
```

## Maskelenmiş Veriler

`Serilog.Enrichers.Sensitive`, belirli türdeki hassas bilgilerin (örneğin e-posta adresleri, telefon numaraları) otomatik olarak maskelenmesini sağlar. Bu örnekte:

- **E-posta adresleri** (Orjinalin dışında Custom olarak düzeneleme yapılmıştır.)
- **IBAN numaraları**
- **Telefon numaraları** Normal de böyle bir tanımı yok. Ama RegexMaskingOperator kalıtım alarak istediğiniz Operator yazabilirsiniz diye örnek yapmak istedim.
  
gibi bilgiler otomatik olarak maskelenmiştir.

### Örnek Kod

```csharp
logger.LogInformation("This is a secret email address: test@test.co.uk");

logger.LogInformation("This is a secret email address: {Email}", "test@test.co.uk");

logger.LogInformation("Bank transfer from Felix Leiter on TR330006100519700007840000");

logger.LogInformation("Bank transfer from Felix Leiter on {BankAccount}", "TR330006100519700007840000");

logger.LogInformation("Credit Card Number: 4111111111111111");

var x = new
{
    Key = 12345,
    XmlValue = "<MyElement><CreditCard>4111111111111111</CreditCard></MyElement>"
};
logger.LogInformation("Object dump with embedded credit card: {x}", x);

logger.LogInformation("GSM Number: 565009098");

logger.LogInformation("This is a CUSTOM_PROPERTY sensitive {CUSTOM_PROPERTY}", "Deneme");
```

Loglama çıktısında hassas bilgiler aşağıdaki gibi maskelenmiş olarak görülecektir:

```json
{
  "Email": "****@test.co.uk",
  "Iban": "****",
  "Card Number": "********111111111",
  "GSM Number": "*****09098"
}
```

## Lisans

Bu proje [MIT Lisansı](LICENSE) ile lisanslanmıştır.
