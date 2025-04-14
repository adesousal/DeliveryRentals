# Delivery Rentals API

**DeliveryRentals** is a REST API built with .NET 9 to manage a motorcycle rental platform for couriers.

This project follows Clean Architecture principles, SOLID design, and modern best practices.

---

## 🚀 Features

- JWT Authentication (admin and couriers)
- Create, read, update, delete (CRUD) for motorcycles
- Courier registration with CNH image upload
- Motorcycle rental with pricing plans and penalty calculations
- Request and error logging with user context
- Messaging (simulated for now)
- Swagger with JWT support
- Test coverage with `xUnit` + `coverlet`

---

## 🧱 Technologies

- .NET 9
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- JWT Bearer Auth
- Swagger (Swashbuckle)
- xUnit, FluentAssertions
- Docker / Docker Compose

---

## 📦 Project Structure

```
DeliveryRentals/
├── API                 # Controllers, middleware, config
├── Application         # Use cases (handlers)
├── Domain              # Business entities and logic
├── Infrastructure      # Repositories, JWT, messaging, storage
├── Persistence         # EF Core and PostgreSQL context
├── Tests               # Unit and integration tests
```

---

## 🔐 Authentication

- **Admin**: manages motorcycles, views logs, etc.
- **Courier**: registers themselves and rents motorcycles

JWT tokens must be provided via **Bearer Token** in Swagger or requests.

---

## 📑 Main Endpoints

| Method | Route                        | Access     | Description                                |
|--------|------------------------------|------------|--------------------------------------------|
| POST   | `/auth/login`                | Public     | Generates token for admin or courier       |
| POST   | `/courier`                   | Public     | Registers a new courier with CNH image     |
| POST   | `/motorcycle`                | Admin      | Registers a new motorcycle                 |
| GET    | `/motorcycle?plate=ABC1234`  | Admin      | Lists motorcycles with license plate filter|
| PUT    | `/motorcycle/{id}/plate`     | Admin      | Updates only the motorcycle license plate  |
| DELETE | `/motorcycle/{id}`           | Admin      | Deletes a motorcycle if not rented         |
| POST   | `/rental`                    | Courier    | Creates a new motorcycle rental            |
| PUT    | `/rental/{id}/return`        | Courier    | Informs the return date of the motorcycle  |
| GET    | `/log`                       | Admin      | Retrieves filtered logs                    |

---

## 🖼️ CNH Image Upload

- Image must be sent as **multipart/form-data**
- Supported formats: `.png`, `.bmp`
- Stored on disk under `CnhImages/`

---

## 🗄️ Database

- Uses **PostgreSQL** with Entity Framework Core
- Migrations run automatically on startup
- Tables created: `User`, `Courier`, `Motorcycle`, `Rental`, `LogEntry`, `Events`

---

## 🐳 Docker

Start a PostgreSQL instance using:

```bash
docker-compose up -d
```

Connection string (preconfigured):

```
Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=senha
```

---

## 🧪 Tests

Run tests with coverage:

```bash
.\Build-tests-coverage.ps1
```

Report will be generated in `Tests/TestResults/` using `coverlet`.

---

## 📘 Documentation

- Swagger is available at:
```
https://localhost:{port}/swagger
```
- Use the **Authorize** button to apply your JWT token.

---

## 🧑‍💻 Default Admin User

Created automatically when the app starts:

```json
{
  "username": "admin",
  "password": "password123",
  "role": "admin"
}
```

> Login using `"admin"` and empty password to get a token.

---

## 📂 Conventions

- Code in English
- Follows SOLID and clean architecture principles
- Request logging includes user ID and role
- Proper separation of concerns across layers
- Unit tests for core business use cases

---

## 🤝 Contributing

Contributions are welcome! Feel free to open issues or PRs to suggest features, fix bugs, or improve documentation and tests.

---

## 📄 License

This project is open for educational use and can serve as a base for your own JWT-based rental or logistics API.