# 🧾 Shrash Tech Certificate Generation API

Welcome to the **Shrash Tech Certificate Generation API**, a modern RESTful service built with **ASP.NET Core** that allows educational institutes or training centers to manage certificates, send them via email, and track communication status. Includes contact query support and a health check.

---

## 🚀 Features

- 🎓 Create, update, delete, and retrieve student certificates
- 📧 Send certificates via email with status tracking
- 📊 View email send history per certificate
- 📬 "Contact Us" module for public queries
- ✅ Health check endpoint for monitoring

---

## 🛠️ Tech Stack

- **.NET 6 / ASP.NET Core Web API**
- **Entity Framework Core** – Code-first migrations
- **MailKit** – For sending emails
- **SQL Server** – Primary database
- **Swagger / Postman** – API Testing
- **CORS Enabled** for cross-origin requests
- **Hosted on**: [shrashcert.bsite.net](https://shrashcert.bsite.net)

---

## 📁 Project Structure

```bash
shrash_tech_certificate_gen_api/
│
├── Controllers/
│   ├── CertificatesController.cs
│   ├── ContactUsController.cs
│   └── HealthCheckController.cs
│
├── Entities/
│   ├── Certificates.cs
│   ├── EmailStatus.cs
│   └── ContactUs.cs
│
├── AppData/
│   └── AppDbContext.cs
│
├── Services/
│   └── EmailService.cs
│
├── appsettings.json
└── Program.cs
