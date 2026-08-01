# Course API

A RESTful Web API built with **ASP.NET Core** and **Entity Framework Core** for managing courses, students, teachers, enrollments, and prerequisites.

**Author:** Saly Najjar — `salynajjar923@gmail.com`  
**Repository:** [github.com/salynajjar/CourseApi](https://github.com/salynajjar/CourseApi)

---

## Features

| Area | Capabilities |
|------|-------------|
| **Courses** | CRUD, search by title, filter by price, prerequisite management |
| **Students** | CRUD, course enrollment, status tracking, global paginated search |
| **Teachers** | CRUD with course associations |
| **Security** | JWT authentication, protected endpoints, role claims |
| **Quality** | Validation, enum checks, exception middleware, async EF Core |

---

## Tech Stack

- ASP.NET Core Web API (.NET 10)
- Entity Framework Core + SQL Server
- JWT Bearer Authentication
- Swagger / OpenAPI
- DTOs & ViewModels

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or full instance)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/salynajjar/CourseApi.git
   cd CourseApi
   ```

2. **Configure the connection string** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CourseApiDb;Trusted_Connection=True;"
   }
   ```

3. **Apply migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the API**
   ```bash
   dotnet run
   ```

5. **Open Swagger** at `https://localhost:7xxx/swagger` (see `launchSettings.json` for ports).

---

## Authentication

1. Register a user:
   ```http
   POST /api/Auth/register
   {
     "username": "saly",
     "email": "salynajjar923@gmail.com",
     "password": "SecurePass123!"
   }
   ```

2. Login and copy the JWT token:
   ```http
   POST /api/Auth/login
   {
     "email": "salynajjar923@gmail.com",
     "password": "SecurePass123!"
   }
   ```

3. Include the token in all protected requests:
   ```
   Authorization: Bearer {your-token}
   ```

---

## Key Endpoints

### Auth
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/Auth/register` | Register new user |
| POST | `/api/Auth/login` | Login and receive JWT |

### Courses *(requires JWT)*
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/Courses` | List all courses |
| GET | `/api/Courses/search?title=` | Search by title |
| GET | `/api/Courses/filter?minPrice=&maxPrice=` | Filter by price |
| POST | `/api/Courses/{courseId}/prerequisites` | Add prerequisite |

### Students *(requires JWT)*
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/Students/courses/search` | Global search with pagination |
| POST | `/api/Students/{studentId}/courses` | Enroll student |
| PUT | `/api/Students/{studentId}/courses/{courseId}/status` | Update enrollment status |

**Search query parameters:** `studentName`, `courseName`, `pageNumber`, `pageSize` (max 50)

### Teachers *(requires JWT)*
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/Teachers` | List all teachers |
| POST | `/api/Teachers` | Create teacher |

---

## Business Rules

- A student **cannot enroll twice** in the same course.
- Prerequisites must be **Completed** and **Passed** before enrollment.
- **Circular prerequisites** are rejected.
- Status transitions enforce valid `EnrollmentStatus` / `PassStatus` pairs.
- Invalid enum values are rejected via `Enum.IsDefined`.

---

## Project Structure

```
CourseApi/
├── Controllers/       # API endpoints
├── Data/              # AppDbContext
├── DTOs/              # Response & request DTOs
├── Enums/             # EnrollmentStatus, PassStatus, Role
├── Extensions/        # JWT configuration
├── Middleware/        # Global exception handling
├── Migrations/        # EF Core migrations
├── Models/            # Domain entities
├── Services/          # JwtService
└── ViewModels/        # Input validation models
```

---

## API Samples

Use [`CourseApi.http`](CourseApi.http) for ready-to-run API samples.

---

## Author

**Saly Najjar**
