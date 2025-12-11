# Elevator Control System

A .NET 10 Minimal API implementation of an elevator control system with clean architecture principles, featuring a static HTML test client for easy interaction with the API.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)
- [Exporting to Postman](#exporting-to-postman)
- [Architecture](#architecture)

---

## Prerequisites

### Required Software

#### 1. .NET 10 SDK
The application requires .NET 10 SDK to build and run.

**Download & Install:**
- Visit: https://dotnet.microsoft.com/download/dotnet/10.0
- Download the SDK (not just the runtime)
- Run the installer
- Verify installation:
  ```powershell
  dotnet --version
  ```
  Should output `10.0.x`

#### 2. Visual Studio Code (Recommended)
VS Code is the recommended IDE for development.

**Download & Install:**
- Visit: https://code.visualstudio.com/
- Download for Windows
- Run the installer with default options

**Recommended VS Code Extensions:**
- **C# Dev Kit** (Microsoft) - C# language support
- **C#** (Microsoft) - IntelliSense, debugging, syntax highlighting
- **REST Client** (Huachao Mao) - Test API endpoints directly in VS Code

**To install extensions:**
1. Open VS Code
2. Press `Ctrl+Shift+X` to open Extensions view
3. Search for extension name
4. Click "Install"

#### 3. Git (Optional but Recommended)
For version control.

**Download & Install:**
- Visit: https://git-scm.com/download/win
- Download and run installer
- Use default options

#### 4. PowerShell 5.1 or Later
Included with Windows 10/11. The project includes PowerShell scripts for convenience.

**Verify version:**
```powershell
$PSVersionTable.PSVersion
```

---

## Installation

### 1. Clone or Download the Repository

**Using Git:**
```powershell
git clone <repository-url>
cd ElevatorControl
```

**Or download as ZIP:**
- Download and extract to a folder
- Open PowerShell and navigate to the extracted folder

### 2. Open in Visual Studio Code

```powershell
code .
```

Or:
- Open VS Code
- File → Open Folder
- Select the `ElevatorControl` folder

### 3. Restore Dependencies

```powershell
dotnet restore
```

---

## Running the Application

### Quick Start (Recommended)

The easiest way to run the application with the test client:

```powershell
.\start-dev.ps1
```

This script will:
1. Kill any process using port 8080
2. Clean and build the API
3. Open the static test client (HTML file)
4. Start the API in Development mode on `http://localhost:8080`
5. Press **Ctrl+C** to stop the API

### Manual Start

#### Option 1: Using dotnet CLI
```powershell
# Build the API
dotnet build src\Api\Api.csproj

# Run the API
dotnet run --project src\Api\Api.csproj
```

The API will be available at: `http://localhost:8080`

#### Option 2: Using VS Code Debugger
1. Open `src/Api/Program.cs`
2. Press `F5` to start debugging
3. The API will launch with debugger attached
4. Set breakpoints by clicking in the left margin of code files

#### Option 3: Using Individual Scripts

**Build and Run API:**
```powershell
.\run-api.ps1
```

**Export OpenAPI for Postman:**
```powershell
.\export-openapi.ps1
```

### Accessing the Application

Once running:
- **API Base URL:** `http://localhost:8080`
- **Swagger UI:** `http://localhost:8080/swagger` (Development mode only)
- **Static Test Client:** Open `src\Client\index.html` in a browser or run `.\start-dev.ps1`

---

## API Endpoints

### Elevator Management

#### Create Elevator
```http
POST /api/elevators
```
Creates a new elevator starting at floor 1, direction Up.

#### List All Elevators
```http
GET /api/elevators
```
Returns all elevators with their current state.

#### Request Floor (Internal)
```http
POST /api/elevators/{id}/request
Content-Type: application/json

{
  "floor": 5
}
```
Simulate pressing a floor button inside the elevator.

#### Call Elevator (External)
```http
POST /api/elevators/{id}/call
Content-Type: application/json

{
  "floor": 3,
  "direction": 1
}
```
Simulate calling elevator from a floor (1 = Up, -1 = Down).

#### Move to Floor
```http
POST /api/elevators/{id}/move
Content-Type: application/json

{
  "floor": 7
}
```
Manually move elevator to a specific floor.

#### Get Next Floor
```http
GET /api/elevators/{id}/next
```
Returns the next floor the elevator should visit.

#### Get Floor Destinations
```http
GET /api/elevators/{id}/destinations
```
Returns list of internal floor requests.

### Health Check
```http
GET /health
```
Returns API health status.

---

## Testing

### Run Unit Tests

```powershell
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run specific test project
dotnet test tests\Api.Tests\Api.Tests.csproj
```

The test suite includes 30 unit tests covering:
- Elevator creation and initialization
- Floor destination management
- Floor call management (up/down)
- Next floor calculation logic
- Direction switching
- Movement and call clearing

### Using the Static Test Client
The static test client is provided as an example of how to use the API and is not
meant to be a production client.

1. Run `.\start-dev.ps1` or open `src\Client\index.html` in a browser
2. The client connects to `http://localhost:8080`
3. Features:
   - Create new elevators
   - Request floors (internal)
   - Call elevators (external with direction)
   - Manually move elevators
   - Move to calculated next floor
   - Refresh to see current state
   - Visual display of floor requests and calls

### Using Swagger UI

When running in Development mode:
1. Navigate to `http://localhost:8080/swagger`
2. Explore and test all endpoints interactively
3. View request/response schemas

---

## Exporting to Postman

To create a Postman collection from the OpenAPI specification:

```powershell
.\export-openapi.ps1
```

This will:
1. Start the API in Development mode
2. Download the OpenAPI JSON specification
3. Save it as `ElevatorControl-OpenAPI.json`
4. Stop the API

**Import into Postman:**
1. Open Postman
2. Click "Import" button
3. Select `ElevatorControl-OpenAPI.json`
4. Click "Import"
5. The collection will appear with all endpoints ready to use

---

## Architecture

### Clean Architecture Layers

This project follows Clean Architecture principles with clear separation of concerns:

#### 1. Domain Layer (`Domain/`)
- **Pure business logic** with no external dependencies
- Contains:
  - `Entities/Elevator.cs` - Core elevator entity with business rules
  - `Enums/ElevatorDirection.cs` - Direction enumeration (Up/Down)
- Implements:
  - Floor request management
  - Call handling (up/down)
  - Next floor calculation algorithm
  - Movement and call clearing logic

#### 2. Application Layer (`Application/`)
- **Use case orchestration** and business workflows
- Contains:
  - `Commands/` - Write operation models
  - `Queries/` - Read operation models
  - `Handlers/` - Command and query handlers (CQRS pattern)
  - `DTOs/` - Data transfer objects for API responses
  - `Mappers/` - Entity to DTO mapping logic
- Independent of infrastructure details

#### 3. Infrastructure Layer (`Infrastructure/`)
- **Data access and external concerns**
- Contains:
  - `Repositories/ElevatorRepository.cs` - In-memory data storage
  - Thread-safe with `ConcurrentDictionary`
- Can be replaced with database implementation without affecting other layers

#### 4. Presentation Layer (`Presentation/`)
- **API interface and HTTP concerns**
- Contains:
  - `Endpoints/ElevatorEndpoints.cs` - Minimal API endpoint definitions
  - `Models/` - Request/response models for HTTP
- Maps HTTP requests to application layer

### Key Design Patterns

- **Repository Pattern** - Abstracts data access
- **CQRS (Command Query Responsibility Segregation)** - Separates reads and writes
- **Dependency Injection** - Loose coupling between layers
- **Mapper Pattern** - Separates domain entities from DTOs
- **Singleton Repository** - Single shared instance for in-memory storage

### Elevator Algorithm

The elevator uses a simple SCAN algorithm:
1. Services all requests in current direction
2. When no more requests in current direction, switches direction
3. Clears floor calls based on actual movement direction (not state variable)
4. Handles both internal requests (buttons inside) and external calls (hall buttons)

---

## Troubleshooting

### Port 8080 Already in Use

**Error:** `Failed to bind to address http://127.0.0.1:8080: address already in use`

**Solution:**
```powershell
# Find process using port 8080
netstat -ano | findstr :8080

# Stop the process (replace XXXX with process ID)
Stop-Process -Id XXXX -Force
```

Or run `.\start-dev.ps1` which automatically handles this.

### Build Errors

**Clear build artifacts:**
```powershell
dotnet clean
dotnet build
```

**Restore packages:**
```powershell
dotnet restore
```

### API Not Starting in Development Mode

Ensure the environment variable is set:
```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project src\Api\Api.csproj
```

### VS Code Debugger Issues

1. Install **C# Dev Kit** extension
2. Restart VS Code
3. Open any `.cs` file
4. Press `F5` to start debugging

---

## Development

### Clean Solution
```powershell
dotnet clean
```

### Format Code
```powershell
dotnet format
```

### Run in Development Mode
```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project src\Api\Api.csproj
```

---

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests: `dotnet test`
5. Submit a pull request

---


## Support

For issues or questions:
- Review this README
- Check the test suite in `tests/Api.Tests/`
- Examine the static client in `src/Client/index.html`
- Review API documentation at `/swagger` when running in Development mode

---

## Additional Resources

- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Minimal APIs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)

