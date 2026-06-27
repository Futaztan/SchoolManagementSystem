# School Management System

This is a comprehensive full-stack School Management System built using the **Blazor WebAssembly (WASM)** and **ASP.NET Core** frameworks. The project follows a modern "Hosted" architecture, allowing for shared data models and logic between the client and server sides.

## 🚀 Features

*   **Student Administration**: Record, edit, and track student data and academic status.
*   **Course & Subject Management**: Create and organize curriculum subjects and class schedules.
*   **Shared Data Models**: Utilizes a `Shared` library to ensure data consistency between the Frontend and Backend.
*   **Database Integration**: Includes a pre-configured `init.sql` script for rapid database schema deployment.
*   **Responsive UI**: A modern interface built with HTML, CSS, and C# that works seamlessly on both desktop and mobile devices.

## 🛠️ Tech Stack

*   **Frontend**: Blazor WebAssembly (.NET)
*   **Backend**: ASP.NET Core Web API
*   **Database**: SQL Server (via Entity Framework Core)
*   **Languages**: C#, JavaScript, HTML, CSS
*   **Scripting**: SQL for database initialization

## 📁 Project Structure

The solution is divided into three main projects:

*   **`Client/`**: The Blazor WASM project containing the UI components, pages, and client-side logic.
*   **`Server/`**: The ASP.NET Core API project handling database migrations, authentication, and business logic.
*   **`Shared/`**: A class library containing DTOs (Data Transfer Objects) and shared models used by both the Client and Server.
*   **`init.sql`**: Database initialization script to set up the necessary tables and seed initial data.
