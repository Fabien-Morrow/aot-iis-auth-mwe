
# 🔧 ASP.NET Core Web API (AOT) + IIS + Windows Auth

## 🎯 Goal

Build a **minimal ASP.NET Core Web API (AOT)** app that relies on **Windows authentication via IIS**

* ASP.NET Core 10 Web API (AOT)
* Windows Authentication (Negotiate)
* No Razor / MVC Views — just API controllers


## 🛠️ IIS Setup

1. Install **IIS**
2. Enable these features:

**Authentication settings**:

  * **Anonymous Authentication**: disabled ❌
  * **ASP.NET Impersonation**: disabled ❌
  * **Windows Authentication**: enabled ✅

    * *Providers*: `Negotiate`, `NTLM`

3. Install the **.NET Hosting Bundle** and the sdk:

   * [.NET 10 Hosting Bundle (Windows)](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-10.0.0-preview.5-windows-hosting-bundle-installer)
  
   *  [.NET 10 sdk (Windows)](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-10.0.100-preview.5-windows-x64-installer)
4. Restart IIS:

   ```bash
   iisreset /RESTART
   ```
5. Install required components:

   * **ARR 3.0 (Application Request Routing)**
     [Download ARR 3.0 (x64)](https://download.microsoft.com/download/e/9/8/e9849d6a-020e-47e4-9fd0-a023e99b54eb/requestRouter_amd64.msi)
   * **URL Rewrite Module**
     [Download URL Rewrite (x64)](https://download.microsoft.com/download/1/2/8/128E2E22-C1B9-44A4-BE2A-5859ED1D4592/rewrite_amd64_en-US.msi)

6. url Rewrite -> Add rule -> Reverse Proxy -> redirect to localhost:5000

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
---

## 🛠️ Firewall Rules (local only)

```powershell
New-NetFirewallRule -DisplayName "AllowLocal5000" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow -RemoteAddress 127.0.0.1
New-NetFirewallRule -DisplayName "BlockExternal5000" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Block
```
---
## 🛠️ Setting up the mwe
1. Open the `WebApplication1` project
2. uncomment the two commented blocks
3. Publish the app:
   ```bash
   dotnet publish -c Release
4. Run the published executable (it listens on port `5000`)
5. Open a browser (e.g., Chrome) and navigate to:

   ```
   http://localhost:80/hello
   ```
6. Sign in

---

## ❌ Actual Result

  * the browser constantly asks for sign-in

---

## ✅ Expected Result

* I should be able to see the result "hello world"


