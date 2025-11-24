# 🧭 TaskManager API (Clean Architecture) — README

> **Full-Stack Backend**
> C# .NET 8 • Clean Architecture • JWT Auth • EF Core • Dapper • Stored Procedures • Caching • Unit Tests

---

# 📌 Overview

TaskManager API คือระบบจัดการโปรเจค และงาน (Projects & Tasks) ที่ออกแบบตามหลัก **Clean Architecture** เพื่อให้สามารถแยกความรับผิดชอบชัดเจน ขยายระบบง่าย และรองรับการทดสอบได้ดี โดยรองรับฟีเจอร์ดังนี้:

* ✔ Register/Login ด้วย JWT Authentication
* ✔ จัดการ Projects (CRUD)
* ✔ จัดการ Tasks (CRUD + Status)
* ✔ Report ผ่าน Stored Procedures (SP) เช่น:

  * ProjectTaskSummary
  * UserProjectOverview
* ✔ Memory Cache สำหรับลดการเรียก SP ซ้ำ
* ✔ Unit Tests ครบทั้ง Auth, Project, Task, Reports
* ✔ EF Core + Dapper + SQL Server

---

# 🏛 Architecture Diagram (Clean Architecture)
![Atl text](https://github.com/kasamawat/TaskManager/blob/ada30128845d26f816ecacdc4463c20c5cf6ef51/img/Architecture%20Diagram.png?raw=true)

---

# 📂 Project Structure

```
TaskManager/
│
├── TaskManager.Api/                 # ASP.NET Core API (JWT, Controllers)
│
├── TaskManager.Application/         # DTOs, Interfaces, Services, Validation
│   ├── DTOs/
│   │   ├── Auth
│   │   ├── Projects
│   │   ├── Tasks
│   ├── Interfaces/
│   ├── Services/
│   └── Validation/
│
├── TaskManager.Domain/              # Entities & Domain Logic
│
├── TaskManager.Infrastructure/      # EF Core, Repositories, Dapper, SP
│   ├── Persistence/
│   └── Repositories/
│
└── TaskManager.Tests/               # xUnit + Moq + FluentAssertions
```

---

# 🔐 Authentication

ระบบใช้ JWT Authentication โดย Token จะถูกสร้างผ่าน `IJwtTokenGenerator` แล้วส่งกลับหลัง Login

**Request ตัวอย่างสำหรับ Login:**

```json
POST /api/auth/login
{
  "email": "test@gmail.com",
  "password": "123456"
}
```

**Response:**

```json
{
  "email": "test@gmail.com",
  "token": "<jwt-token>"
}
```

Swagger รองรับปุ่ม `Authorize` สำหรับใส่ Token

---

# 📘 Features

## 1. ✔ Project Management

* Create Project
* Update Project
* Delete Project
* Get Project Detail

## 2. ✔ Task Management

* Add Task ให้ Project
* Update Task
* Change Task Status (Todo / InProgress / Done)
* Delete Task

## 3. ✔ Reporting (Stored Procedures)

### SP 1: `GetProjectTaskSummary`

สรุปจำนวนงานในโปรเจค เช่น:

* Total Tasks
* Todo / InProgress / Done
* Overdue

### SP 2: `GetUserProjectOverview`

สรุปโปรเจคทั้งหมดของ User

## 4. ✔ Caching Layer

ใช้ `IMemoryCache` เพื่อ Cache ผลของ SP 30–60 วินาที

---

# 🧱 Example Stored Procedure

```sql
CREATE PROCEDURE [dbo].[GetProjectTaskSummary]
  @ProjectId UNIQUEIDENTIFIER
AS
BEGIN
  SELECT
    p.Id AS ProjectId,
    p.Name AS ProjectName,
    COUNT(t.Id) AS TotalTasks,
    SUM(CASE WHEN t.Status = 0 THEN 1 ELSE 0 END) AS TodoCount,
    SUM(CASE WHEN t.Status = 1 THEN 1 ELSE 0 END) AS InProgressCount,
    SUM(CASE WHEN t.Status = 2 THEN 1 ELSE 0 END) AS DoneCount,
    SUM(CASE WHEN t.DueDate < GETUTCDATE() AND t.Status <> 2 THEN 1 ELSE 0 END) AS OverdueCount
  FROM Projects p
  LEFT JOIN ProjectTasks t ON t.ProjectId = p.Id
  WHERE p.Id = @ProjectId
  GROUP BY p.Id, p.Name;
END
GO
```

---

# 🧪 Unit Test Coverage

Unit Test ใช้ tools:

* **xUnit**
* **Moq** (mock repositories)
* **FluentAssertions** (assert แบบอ่านง่าย)

## ตัวอย่าง Test สำคัญ

* AuthServiceTests
* ProjectServiceTests
* TaskServiceTests
* ProjectReportServiceTests
* UserProjectOverviewServiceTests

ในรายงานสามารถอธิบายได้ว่า:

> “รายงานใช้ Mock Repository ทำให้ทดสอบ Business Logic ได้โดยไม่ต้องเชื่อมต่อ DB จริง”

---

# 🚀 Running the Project

## 1. Setup Connection String

ใน `appsettings.json` ของ TaskManager.Api:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TaskManager;Trusted_Connection=True;"
  }
}
```

## 2. Apply Migrations

```
dotnet ef database update
```

## 3. Run API

```
dotnet run --project TaskManager.Api
```

Swagger จะอยู่ที่:
👉 `https://localhost:<port>/swagger`

---

# 📦 Technologies Used

* .NET 8
* ASP.NET Core Web API
* Clean Architecture
* Entity Framework Core
* Dapper
* SQL Server
* JWT Authentication
* IMemoryCache
* xUnit + Moq
* FluentValidation

---

# 🎯 เป้าหมายของโปรเจคนี้

* ออกแบบระบบตาม Clean Architecture (API → App → Domain → Infra)
* ใช้ Dapper + Stored Procedure สำหรับ report-heavy workload
* ใช้ MemoryCache ทำ performance optimization
* เขียน Unit Test ให้ Service และรองรับ dependency injection
* มี Separation of Concerns ชัดเจน
* ใช้ EF Core สำหรับ persistence ปกติ และ Dapper สำหรับงานแบบ query-heavy

---

# 📄 License

MIT

---

# 🙌 Author

Kasamawat Thanakan

(โปรเจคนี้ได้ออกแบบครอบคลุม Clean Architecture + SP + JWT + Unit Test แบบเต็มรูปแบบ)
