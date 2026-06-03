# TekNokta Backend

TekNokta ilan paylaşım uygulamasının Clean Architecture tabanlı .NET Core backend projesi.

## Mimari

Proje `src` klasörü altında dört ana katmandan oluşur:

- `TekNokta.Domain`
- `TekNokta.Application`
- `TekNokta.Infrastructure`
- `TekNokta.Api`

## Katman Sorumlulukları

### Domain

Entity, enum, model ve domain objelerini içerir. Başka hiçbir projeye bağımlı değildir.

### Application

Repository ve service abstractionları, DTO'lar ve result modellerini içerir. Sadece `Domain` katmanına referans verir.

### Infrastructure

Entity Framework, repository implementasyonları, servis implementasyonları, JWT, persistence ve migration işlemlerini içerir. `Application` ve `Domain` katmanlarına referans verir.

### API

Controller endpointleri, authentication middleware, CORS ve Swagger yapılandırmasını içerir. `Application` ve `Infrastructure` katmanlarına referans verir.

## Kurulum

Bağımlılıkları yükleyin:

```powershell
dotnet restore
```

Projeyi build edin:

```powershell
dotnet build
```

Veritabanını migration ile güncelleyin:

```powershell
dotnet ef database update --project src/TekNokta.Infrastructure --startup-project src/TekNokta.Api
```

API'yi çalıştırın:

```powershell
dotnet run --project src/TekNokta.Api
```

Swagger arayüzü için uygulama çalıştıktan sonra `/swagger` adresine gidin.

## Register ve Login Endpointleri

### Register

`POST /api/auth/register`

Örnek request:

```json
{
  "firstName": "Test",
  "lastName": "Kullanici",
  "email": "test@teknokta.com",
  "password": "Test123!",
  "phoneNumber": "5551112233"
}
```

Başarılı olduğunda kullanıcı oluşturulur ve access token döner. Aynı e-posta ile tekrar kayıt denenirse hata döner.

### Login

`POST /api/auth/login`

Örnek request:

```json
{
  "email": "test@teknokta.com",
  "password": "Test123!"
}
```

Doğru bilgilerle access token döner. Yanlış şifre veya bulunamayan kullanıcı için hata döner.

## JWT Kullanımı

Register veya login response içindeki `accessToken` değeri alınır. Swagger arayüzünde `Authorize` butonuna basılır ve token şu formatta girilir:

```text
Bearer ACCESS_TOKEN
```

JWT doğrulamasını test etmek için:

- Public endpoint: `GET /api/test/public`
- Protected endpoint: `GET /api/test/protected`

Protected endpoint JWT token olmadan `401 Unauthorized` döner.

Production ortamında JWT secret değeri `appsettings.json` içinde tutulmamalıdır. Environment variable, user secrets veya secret manager kullanılmalıdır.

## Migration Komutları

Yeni migration oluşturmak için:

```powershell
dotnet ef migrations add InitialCreate --project src/TekNokta.Infrastructure --startup-project src/TekNokta.Api --output-dir Persistence/Migrations
```

Veritabanını güncellemek için:

```powershell
dotnet ef database update --project src/TekNokta.Infrastructure --startup-project src/TekNokta.Api
```

`dotnet-ef` yüklü değilse:

```powershell
dotnet tool install --global dotnet-ef
```

## Katman Bağımlılık Kuralları

- `TekNokta.Domain` hiçbir projeye referans vermez.
- `TekNokta.Application` sadece `TekNokta.Domain` projesine referans verir.
- `TekNokta.Infrastructure`, `TekNokta.Application` ve `TekNokta.Domain` projelerine referans verir.
- `TekNokta.Api`, `TekNokta.Application` ve `TekNokta.Infrastructure` projelerine referans verir.
- Controller'lar Infrastructure implementasyonlarını doğrudan kullanmaz; servis interface'leri üzerinden çalışır.
- Entity Framework ve `DbContext` sadece Infrastructure katmanında bulunur.
- Authentication business logic controller içinde değil, `AuthService` içinde yer alır.
