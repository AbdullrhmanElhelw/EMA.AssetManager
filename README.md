# EMA.AssetManager

EMA.AssetManager is an Asset Management System built with **.NET 10** and **Blazor Web App** (Interactive Server), utilizing **MudBlazor** for the UI components.

## Features

- **Dashboard**: Overview of system status.
- **Asset Management**: Create, update, and track assets.
- **Inventory Control**: Manage Categories, Items, and Warehouses.
- **Transactions**: Track asset movements and history.
- **QR Code Integration**: Generate QR codes for assets.

## Tech Stack

- **Framework**: [.NET 10](https://dotnet.microsoft.com/)
- **UI Framework**: [Blazor Web App](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) (Interactive Server)
- **Component Library**: [MudBlazor](https://mudblazor.com/)
- **Database**: SQL Server
- **ORM**: Entity Framework Core

## Project Structure

The solution follows a layered architecture:

- **EMA.AssetManager.Domain**: Contains the domain entities (`Asset`, `Item`, `Warehouse`, etc.), database context, and data migrations.
- **EMA.AssetManager.Services**: Contains the business logic and service implementations (`AssetService`, `TransactionService`, etc.).
- **EMA.AssetManager.UI**: The Blazor Web Application project containing the UI components and pages.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server

### Installation & Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd EMA.AssetManager
   ```

2. **Configure Database Connection**
   Open `src/EMA.AssetManager.UI/appsettings.json` and update the `ConnectionStrings` section with your SQL Server connection string.

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=EMA.AssetManagerDb; User Id=SA; Password=your_password; Encrypt=False"
   }
   ```
   *Note: Ensure the credentials match your SQL Server setup.*

3. **Apply Database Migrations**
   Initialize the database by applying the existing migrations. Run the following command from the solution root:

   ```bash
   dotnet ef database update --project EMA.AssetManager.Domain --startup-project src/EMA.AssetManager.UI
   ```

4. **Run the Application**
   Navigate to the UI project directory and run the application:

   ```bash
   cd src/EMA.AssetManager.UI
   dotnet run
   ```

   The application will start, and you can access it via the URL provided in the console.
