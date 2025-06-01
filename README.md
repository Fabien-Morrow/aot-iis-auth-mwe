
# 🔧 ASP.NET Core Web API (AOT) + IIS + Windows Auth — Swagger Empty

## 🎯 Goal

Build a **minimal ASP.NET Core Web API (AOT)** app that relies on **Windows authentication via IIS**, with a working Swagger interface.

* ASP.NET Core 8 Web API (AOT)
* Windows Authentication (Negotiate)
* Swagger enabled with `UseSwagger()` + `UseSwaggerUI()`
* No Razor / MVC Views — just API controllers


## 🛠️ IIS Setup

1. Install **IIS**
2. Enable these features:

   * ✅ *Windows Authentication* (Negociate, NTLM)
   * ❌ *Anonymous Authentication*
   * ❌ *ASP.NET Impersonation*
3. Install the **.NET Hosting Bundle**:

   * [.NET 9.0.5 Hosting Bundle (Windows)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
4. Restart IIS:

   ```bash
   iisreset /RESTART
   ```
5. Install required components:

   * **ARR 3.0 (Application Request Routing)**
     [Download ARR 3.0 (x64)](https://download.microsoft.com/download/e/9/8/e9849d6a-020e-47e4-9fd0-a023e99b54eb/requestRouter_amd64.msi)
   * **URL Rewrite Module**
     [Download URL Rewrite (x64)](https://download.microsoft.com/download/1/2/8/128E2E22-C1B9-44A4-BE2A-5859ED1D4592/rewrite_amd64_en-US.msi)

it generates this :
* **Web root**: `C:\inetpub\wwwroot`
* **Generated `web.config`**:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
  <system.web>
    <identity impersonate="false" />
  </system.web>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="ReverseProxyInboundRule1" stopProcessing="true">
          <match url="(.*)" />
          <action type="Rewrite" url="http://localhost:5000/{R:1}" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
```

* **Authentication settings**:

  * **Anonymous Authentication**: disabled ❌
  * **ASP.NET Impersonation**: disabled ❌
  * **Windows Authentication**: enabled ✅

    * *Providers*: `Negotiate`, `NTLM`

---

## 🛠️ Firewall Rules (local only)

```powershell
New-NetFirewallRule -DisplayName "AllowLocal5000" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow -RemoteAddress 127.0.0.1
New-NetFirewallRule -DisplayName "BlockExternal5000" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Block
```
---
## 🛠️ Setting up the mwe
1. Open the `WebApplication2` project
2. Publish the app:
   ```bash
   dotnet publish -c Release
3. Run the published executable (it listens on port `5000`)
4. Open a browser (e.g., Chrome) and navigate to:

   ```
   http://localhost:80
   ```
5. Sign in using a domain admin account

---

## ❌ Actual Result

* In **development** (`dotnet run`):

  * Swagger loads and lists endpoints ✅
  * Endpoints are reachable ✅

* In **production** (via IIS reverse proxy):

  * Swagger UI loads, but **endpoint is missing** ❌
  * The backend app is correctly running on `localhost:5000`

---

## ✅ Expected Result

* Swagger should **display the API endpoint** (e.g., `POST /api/Maintenance/reset`)
* I should be able to call them successfully with Windows authentication


