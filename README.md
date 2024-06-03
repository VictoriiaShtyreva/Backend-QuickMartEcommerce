# Fullstack Project - QuickMart

![TypeScript](https://img.shields.io/badge/TypeScript-v.4-green)
![React](https://img.shields.io/badge/React-v.18-blue)
![Redux toolkit](https://img.shields.io/badge/Redux-v.1.9-brown)
![EF Core](https://img.shields.io/badge/EF%20Core-v.8-cyan)
![.NET Core](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Cloudinary](https://img.shields.io/badge/Cloudinary-3448C5?style=for-the-badge&logo=Cloudinary&logoColor=white)
![Stripe](https://img.shields.io/badge/Stripe-626CD9?style=for-the-badge&logo=Stripe&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white)
![Nuget](https://img.shields.io/badge/NuGet-004880?style=for-the-badge&logo=nuget&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=Swagger&logoColor=white)

The repository is an integral part of my capstone final project of the Full Stack Program at [Integrify](https://www.integrify.io/). It is the back-end component of an e-commerce application that showcases the capabilities which online retailers to operate. The data is retrieved, and can be created, deleted etc. through an API, an API which is running on ASP. NET Core. The application is passed with user authentication selected roles for user. Registering as a customer gives you the liberty to create an order, manage your history, and review a product. The administrator is able to do CRUD operations on the products, user- and all the orders from his own dashboard too.

- Frontend: TypeScript, React, Redux Toolkit, React Router, Material UI, Jest
- Backend: ASP.NET Core, Entity Framework Core, PostgreSQL

This repository contains only the backend code of the application. For the frontend implementation, please refer to the [frontend repository](https://github.com/VictoriiaShtyreva/Frontend-QuickMartEcommerce). You can explore the live deployment of my frontend e-commerce project by visiting [QuickMart](https://quick-mart-ecommerce.vercel.app/).

## Table of Contents

- [Technologies and Libraries](#technologies-and-libraries)
- [Getting Started](#getting-started)
- [Database Schema and ERD](#database-schema-and-erd)
- [Backend Server with ASP.NET Core](#backend-server-with-aspnet-core)
- [API Endpoints](#api-endpoints)
- [Key Features](#key-features)
- [Unit Testing](#unit-testing)
- [Repository Structure](#repository-structure)
- [Acknowledgements](#acknowledgements)

## Technologies and Libraries

> This section outlines the key technologies and libraries utilized in the backend of my e-commerce application, detailing their roles and significance in the overall architecture.

| Technology                                                                                                                                 | Purpose                                                                                        |
| ------------------------------------------------------------------------------------------------------------------------------------------ | ---------------------------------------------------------------------------------------------- |
| **[ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)**                                                                         | Core framework for building server-side logic, routing, middleware, and dependency management. |
| **[Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli#create-the-database)** | ORM for database operations, abstracts SQL queries, simplifying data manipulation.             |
| **[PostgreSQL](https://www.postgresql.org/)**                                                                                              | Relational database management system for storing all application data.                        |

| Library                                                                                                                                | Purpose                                                                                                                                |
| -------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| **[AutoMapper](https://www.nuget.org/packages/automapper/)**                                                                           | Automates the mapping of data entities to DTOs, reducing manual mapping code.                                                          |
| **[Ardalis.GuardClauses](https://www.nuget.org/packages/Ardalis.GuardClauses)**                                                        | Provides guard clauses to enforce pre-conditions in methods, enhancing robustness and error handling.                                  |
| **[Microsoft.AspNetCore.Identity](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity)**                                      | Manages user authentication, security, password hashing, and role management within the application.                                   |
| **[JWT Bearer Authentication](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer/9.0.0-preview.3.24172.13)** | Implements token-based authentication for securing API endpoints, requiring valid JWTs for access.                                     |
| **[xUnit](https://www.nuget.org/packages/xunit)**                                                                                      | Framework for unit testing, ensuring components function correctly in isolation.                                                       |
| **[Moq](https://www.nuget.org/packages/Moq)**                                                                                          | Mocking library used with xUnit to simulate behavior of dependencies during testing.                                                   |
| **[Microsoft.EntityFrameworkCore.Proxies](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Proxies/)**                     | The Microsoft.EntityFrameworkCore.Proxies package contains implementations of dynamic proxies for lazy-loading and/or change-tracking. |
| **[CloudinaryDotNet](https://www.nuget.org/packages/CloudinaryDotNet)**                                                                | Manages image uploads, storage, and transformations in the cloud with Cloudinary API.                                                  |
| **[Stripe.net](https://www.nuget.org/packages/Stripe.net/)**                                                                           | The official Stripe .NET library, supporting .NET Standard 2.0+, .NET Core 2.0+, and .NET Framework 4.6.1+.                            |

## Getting Started

This section provides instructions on how to set up your environment and get the project up and running on your local machine for development and testing purposes.

### Prerequisites

Before you begin, ensure you have the following installed:

- .NET 8.0 SDK or later
- PostgreSQL server
- Git
- Docker

1. **Clone the repository**:
   Open your terminal and execute the following command to clone the repositories:
   ```bash
   git clone <frontend repo>
   git clone <backend repo>
   ```
2. **Configure the application**:
   Navigate to the Web API layer in the back-end directory. Open the appsettings.json file and add the following configurations:
   ```bash
   "ConnectionStrings": {
    "Localhost": "<YOUR_LOCAL_DB_CONNECTION_STRING>"
    "Remote": <YOUR_REMOTE_DB>
   },
   "Secrets": {
    "JwtKey": "YourSecretKey",
    "Issuer": "YourIssuer"
   },
    "CloudinarySettings": {
    "CloudName": "YourName",
    "ApiKey": "YourSecretKey",
    "ApiSecret": "YourSecretApi"
   },
   "Stripe": {
    "SecretKey": "YourSecretKey",
    "PublishableKey": "YourPublishKey",
    "WhSecret": "YourWebhookSecret"
   },
   ```
   > Replace <YOUR_LOCAL_DB_CONNECTION_STRING> and <YOUR_REMOTE_DB> with your actual PostgreSQL connection strings.
   > Replace YourCloudName, YourApiKey, and YourApiSecret with your Cloudinary account details.
   > Replace YourSecretKey, YourPublishKey, YourWebhookSecret with your Stripe account details.

To get your Cloudinary settings, visit the [Cloudinary website](https://cloudinary.com/) and sign up or log in to your account. Navigate to the Dashboard to find your Cloud Name, API Key, and API Secret.

3. **Create the database**: Run the following commands
   ```bash
    dotnet tool install --global dotnet-ef
    dotnet add package Microsoft.EntityFrameworkCore.Design
    dotnet ef migrations add CreateDb
    dotnet ef database update
   ```
   > If a Migrations folder already exists, delete it.
4. **Start the Backend**: Navigate to the WebAPI layer directory and run the following command to start the backend server on your local machine
   ```bash
   dotnet watch run
   ```
5. **Access Swagger UI**: Open a web browser of your choice and enter the following URL in your browser's address bar:

   ```bash
   http://localhost:<YOUR_LOCALHOST>/index.html
   ```

   > This URL directs you to the Swagger UI page that is automatically generated by the Swagger middleware in your ASP.NET Core application.

6. **Set up the Frontend**: In the front-end directory, create a file called .env in the root of the project. Add this value:

   ```bash
     REACT_APP_API_URL=http://localhost:3000
   ```

   > Note: The port number can be different.

### Running with Docker

You can also run the entire application using Docker and Docker Compose. This is useful for setting up the environment quickly and consistently.

1. **Start the Docker Compose Services**: Navigate to the directory containing your docker-compose.yml file and run the following command to start all the services:

   ```bash
     docker-compose up
   ```

   > Access the Application:
   > \*\*Frontend: Open your web browser and go to `http://localhost:3000`.
   > Backend: The backend will be running at `http://localhost:8080`.

## Database Schema and ERD

Detailed database schema definitions including data types, constraints, and relationships. All the data is stored and retrieved using [PostgreSQL](https://www.postgresql.org/), a free and open-source relational database management system. ERD diagram was created using [Lucidchart](https://www.lucidchart.com/pages/landing?utm_source=google&utm_medium=cpc&utm_campaign=_chart_en_tier2_mixed_search_brand_exact_&km_CPC_CampaignId=1520850463&km_CPC_AdGroupID=57697288545&km_CPC_Keyword=lucidchart&km_CPC_MatchType=e&km_CPC_ExtensionID=&km_CPC_Network=g&km_CPC_AdPosition=&km_CPC_Creative=442433237648&km_CPC_TargetID=kwd-33511936169&km_CPC_Country=9072483&km_CPC_Device=c&km_CPC_placement=&km_CPC_target=&gad_source=1&gclid=CjwKCAjwrIixBhBbEiwACEqDJdqG-nDfXAHumE5poslMyIZM3meH7qJUs0CXQsOxBaL5fCNZwOUMURoC7MwQAvD_BwE).

![Database schema](/readmeImg/Diagram%20DB.png)

> Below is a table describing the relationships between the various entities in the ERD. Each table's primary key (PK) and foreign key (FK) relationships are denoted in the ERD, which establish the links between different entities.

| Entity 1     | Relationship | Entity 2         | Description                                                                                    |
| ------------ | ------------ | ---------------- | ---------------------------------------------------------------------------------------------- |
| `users`      | One to Many  | `reviews`        | Each user can write multiple reviews. A review must be associated with a user.                 |
| `products`   | One to Many  | `reviews`        | Each product can have multiple reviews. A review must be related to a product.                 |
| `products`   | One to Many  | `product_images` | Each product can have multiple product images. A product images must be related to a product.  |
| `categories` | One to Many  | `products`       | Each category can encompass multiple products. A product must belong to a category.            |
| `users`      | One to Many  | `orders`         | Each user can place multiple orders. An order is linked to the user who placed it.             |
| `orders`     | One to Many  | `order_items`    | Each order can contain multiple items. Order items are linked back to their respective orders. |
| `orders`     | One to One   | `address`        | Each order is associated with one address. An address is linked to one order.                  |

## Backend Server with ASP.NET Core

### Architecture Diagram

![Architecture Diagram](/readmeImg/Ecommerce.png)

> The diagram represents the architecture of an Ecommerce platform built with ASP.NET Core and complied with the Clean Architecture. It is structured into several layers, each responsible for different aspects of the application:

- **Ecommerce.Core**: This layer includes classes and interfaces that define the basic entities like products, categories, users, and more. It also contains repository interfaces which abstract the data access logic.
- **Ecommerce.Service**: This layer contains services that handle business logic and operations, interacting with the Core layer to manipulate and retrieve data.
- **Ecommerce.Controller**: This layer is responsible for handling incoming HTTP requests and returning responses. It interacts with the Service layer to fetch and send data back to the client.
- **Ecommerce.WebAPI**: This houses the controllers and is the entry point for client interactions through HTTP requests. It includes middleware for error handling.

### Connections and Data Flow

1. **Controllers** in the WebAPI layer receive HTTP requests.
2. **Controllers** call the **Service** layer to perform the necessary business logic.
3. The **Service** layer uses the **Repositories** from the Data Access layer to fetch or persist data.
4. **Repositories** interact with the database and return the data to the Service layer.
5. The **Service** layer processes the data according to the business rules defined in the Core layer.
6. The processed data is returned to the **Controller**, which sends the appropriate HTTP response back to the client.

### Database Connection

The project follows the Repository pattern for database operations. Each repository implements an interface defined in the Core layer, ensuring that the data access code is abstracted and can be easily swapped or mocked for testing purposes.

- **DbContext**: Configured in the WebAPI layer using dependency injection, providing a session with the database.
- **Repositories**: Use the DbContext to perform CRUD operations on the entities.

### Example

Here is an example of how an Order is processed:

1. A client sends a `POST` request to create a new order.
2. The **OrderController** receives the request and calls the `CreateOrder` use case in the **OrderService**.
3. The **OrderService** calls the `AddOrder` method in the **OrderRepository**.
4. The **OrderRepository** uses the **DbContext** to save the new order to the database.
5. The **OrderService** returns the result to the **OrderController**.
6. The **OrderController** sends a response back to the client indicating the order was successfully created.

### Summary

By following the principles of Clean Architecture, this project ensures that the business logic is kept separate from the infrastructure and UI, promoting better organization, testability, and scalability. Each layer has a specific responsibility and communicates with adjacent layers through well-defined interfaces, ensuring a clean separation of concerns.

## API Endpoints

All the endpoints of the API are documented and can be tested directly on the generated Swagger page. From there you can view each endpoint URL, their HTTP methods, request body structures and authorization requirements. Access the Swagger page from this [link](https://quick-mart-ecommerce.azurewebsites.net/index.html).

![Swagger](/readmeImg/Swagger.png)

## Key Features

| Feature                                   | Description                                                                                            |
| ----------------------------------------- | ------------------------------------------------------------------------------------------------------ |
| **User Authentication and Authorization** | Secure user registration, login, and role-based access control using JWT tokens.                       |
| **Password Hashing**                      | Securely hashes user passwords to protect sensitive information.                                       |
| **Product Management**                    | Comprehensive product management including creation, update, deletion, and viewing of product details. |
| **Order Processing**                      | Full order lifecycle management from order creation, payment processing, to order fulfillment.         |
| **Payment Integration**                   | Integration with payment gateways to handle secure and seamless transactions.                          |
| **Image Upload and Management**           | Utilizes Cloudinary for uploading, storing, and managing product images.                               |
| **Search and Filtering**                  | Advanced search and filtering capabilities to help users find products quickly.                        |
| **Reviews and Ratings**                   | Allows users to leave reviews and ratings for products, enhancing customer feedback and engagement.    |
| **Admin Dashboard**                       | Admin interface for managing users, orders, products, and other administrative tasks.                  |
| **Unit Testing**                          | Comprehensive test coverage using xUnit and Moq to ensure code quality and reliability.                |

## Unit Testing

Unit tests in this project are crucial for ensuring code quality and functionality. The **Ecommerce.Test** namespace contains tests that cover various components:

- **Testing Strategy**: Utilizes xUnit. Mocking is facilitated by libraries like Moq to simulate repository interactions.

![UnitTesting](/readmeImg/UnitTests.png)

### Running Unit Tests

To execute the unit tests, you can use the .NET CLI. Follow these steps to run the tests from the command line:

1. **Navigate to the project directory**: Open your terminal or command prompt and navigate to the root directory of project.

   ```bash
   cd Ecommerce/Ecommerce.Test
   ```

2. **Run the tests**: Execute the following command to run all the tests in the solution:

   ```bash
   dotnet test
   ```

   > This command will run all the tests, providing you with a summary of the test results, including passed, failed, and skipped tests.

## Repository Structure

My project is organized as follows to maintain a clean and navigable codebase:

```plaintext
Ecommerce
   |-- Ecommerce.Controller
   |   |-- Ecommerce.Controller.csproj
   |   |-- src
   |   |   |-- Controllers
   |   |   |   |-- AuthController.cs
   |   |   |   |-- CategoryController.cs
   |   |   |   |-- OrderController.cs
   |   |   |   |-- ProductController.cs
   |   |   |   |-- ProductImagesController.cs
   |   |   |   |-- ReviewController.cs
   |   |   |   |-- UserController.cs
   |-- Ecommerce.Core
   |   |-- Ecommerce.Core.csproj
   |   |-- src
   |   |   |-- Common
   |   |   |   |-- AppException.cs
   |   |   |   |-- CloudinarySettings.cs
   |   |   |   |-- PaginatedResult.cs
   |   |   |   |-- PasswordChange.cs
   |   |   |   |-- QueryOptions.cs
   |   |   |   |-- UserCredential.cs
   |   |   |-- Entities
   |   |   |   |-- BaseEntity.cs
   |   |   |   |-- Category.cs
   |   |   |   |-- OrderAggregate
   |   |   |   |   |-- Address.cs
   |   |   |   |   |-- Order.cs
   |   |   |   |   |-- OrderItem.cs
   |   |   |   |-- Product.cs
   |   |   |   |-- ProductImage.cs
   |   |   |   |-- Review.cs
   |   |   |   |-- TimeStamp.cs
   |   |   |   |-- User.cs
   |   |   |-- Interfaces
   |   |   |   |-- IBaseRepository.cs
   |   |   |   |-- ICategoryRepository.cs
   |   |   |   |-- IOrderRepository.cs
   |   |   |   |-- IProductImageRepository.cs
   |   |   |   |-- IProductRepository.cs
   |   |   |   |-- IReviewRepository.cs
   |   |   |   |-- IUserRepository.cs
   |   |   |-- ValueObjects
   |   |   |   |-- OrderStatus.cs
   |   |   |   |-- ProductSnapshot.cs
   |   |   |   |-- SortOrder.cs
   |   |   |   |-- SortType.cs
   |   |   |   |-- UserRole.cs
   |-- Ecommerce.Service
   |   |-- Ecommerce.Service.csproj
   |   |-- src
   |   |   |-- DTOs
   |   |   |   |-- AddressDto.cs
   |   |   |   |-- CategoryDto.cs
   |   |   |   |-- OrderDto.cs
   |   |   |   |-- OrderItemDto.cs
   |   |   |   |-- ProductDto.cs
   |   |   |   |-- ProductImageDto.cs
   |   |   |   |-- ProductSnapshotDto.cs
   |   |   |   |-- ReviewDto.cs
   |   |   |   |-- UserDto.cs
   |   |   |-- Interfaces
   |   |   |   |-- IAuthService.cs
   |   |   |   |-- IBaseService.cs
   |   |   |   |-- ICategoryService.cs
   |   |   |   |-- ICloudinaryImageService.cs
   |   |   |   |-- IOrderItemService.cs
   |   |   |   |-- IOrderService.cs
   |   |   |   |-- IPasswordService.cs
   |   |   |   |-- IProductImageService.cs
   |   |   |   |-- IProductService.cs
   |   |   |   |-- IReviewService.cs
   |   |   |   |-- ITokenService.cs
   |   |   |   |-- IUserService.cs
   |   |   |-- Services
   |   |   |   |-- AuthService.cs
   |   |   |   |-- BaseService.cs
   |   |   |   |-- CategoryService.cs
   |   |   |   |-- OrderItemService.cs
   |   |   |   |-- OrderService.cs
   |   |   |   |-- ProductImageService.cs
   |   |   |   |-- ProductService.cs
   |   |   |   |-- ReviewService.cs
   |   |   |   |-- UserService.cs
   |   |   |-- Shared
   |   |   |   |-- AutoMapperProfile.cs
   |-- Ecommerce.Tests
   |   |-- Ecommerce.Tests.csproj
   |   |-- src
   |   |   |-- xUnitTests
   |   |   |   |-- Core
   |   |   |   |   |-- OrderTests.cs
   |   |   |   |   |-- ProductTests.cs
   |   |   |   |-- Service
   |   |   |   |   |-- AuthServiceTests.cs
   |   |   |   |   |-- CategoryServiceTests.cs
   |   |   |   |   |-- OrderItemServiceTests.cs
   |   |   |   |   |-- OrderServiceTests.cs
   |   |   |   |   |-- ProductImageServiceTests.cs
   |   |   |   |   |-- ProductServiceTests.cs
   |   |   |   |   |-- ReviewServiceTests.cs
   |   |   |   |   |-- UserServiceTests.cs
   |   |   |   |-- WebAPI
   |   |   |   |   |-- PasswordServiceTests.cs
   |-- Ecommerce.WebAPI
   |   |-- Ecommerce.WebAPI.csproj
   |   |-- Ecommerce.WebAPI.http
   |   |-- Program.cs
   |   |-- Properties
   |   |   |-- launchSettings.json
   |   |-- src
   |   |   |-- AuthorizationPolicy
   |   |   |   |-- AdminOrOwnerAccountRequirement.cs
   |   |   |-- Data
   |   |   |   |-- AppDbContext.cs
   |   |   |   |-- SeedingData.cs
   |   |   |   |-- TimeStampInteceptor.cs
   |   |   |-- ExternalService
   |   |   |   |-- CloudinaryImageService.cs
   |   |   |   |-- PasswordService.cs
   |   |   |   |-- TokenService.cs
   |   |   |-- Middleware
   |   |   |   |-- ExceptionMiddleware.cs
   |   |   |-- Repositories
   |   |   |   |-- AddressRepository.cs
   |   |   |   |-- CategoryRepository.cs
   |   |   |   |-- OrderItemRepository.cs
   |   |   |   |-- OrderRepository.cs
   |   |   |   |-- ProductImageRepository.cs
   |   |   |   |-- ProductRepository.cs
   |   |   |   |-- ReviewRepository.cs
   |   |   |   |-- UserRepository.cs
   |   |   |-- ValueConversion
   |   |   |   |-- JsonValueConverter.cs
   |-- Ecommerce.sln
   |-- Ecommerce.sln.DotSettings.user
README.md
```

## Acknowledgements

- Integrify-Finland for providing the assignment.
- All contributors who help in enhancing and maintaining the project.

[**Return**](#technologies-and-libraries)
