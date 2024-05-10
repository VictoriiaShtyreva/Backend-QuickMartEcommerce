# Fullstack Project - QuickMart

![TypeScript](https://img.shields.io/badge/TypeScript-v.4-green)
![React](https://img.shields.io/badge/React-v.18-blue)
![Redux toolkit](https://img.shields.io/badge/Redux-v.1.9-brown)
![.NET Core](https://img.shields.io/badge/.NET%20Core-v.8-purple)
![EF Core](https://img.shields.io/badge/EF%20Core-v.8-cyan)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-v.16-drakblue)

The repository is an integral part of my capstone final project of the Full Stack Program at [Integrify](https://www.integrify.io/). It is the back-end component of an e-commerce application that showcases the capabilities which online retailers to operate. The data is retrieved, and can be created, deleted etc. through an API, an API which is running on ASP. NET Core. The application is passed with user authentication selected roles for user. Registering as a customer gives you the liberty to place product into cart, create an order, manage your history, and review a product. The administrator is able to do CRUD operations on the products, user- and all the orders from his own dashboard too.

- Frontend: TypeScript, React, Redux Toolkit, React Router, Material UI, Jest
- Backend: ASP.NET Core, Entity Framework Core, PostgreSQL

This repository contains only the backend code of the application. For the frontend implementation, please refer to the [frontend repository](https://github.com/VictoriiaShtyreva/QuickMart). You can explore the live deployment of my frontend e-commerce project by visiting [QuickMart](https://fs17-frontend-project.vercel.app/).

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

| Library                                                                                                                                | Purpose                                                                                               |
| -------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- |
| **[AutoMapper](https://www.nuget.org/packages/automapper/)**                                                                           | Automates the mapping of data entities to DTOs, reducing manual mapping code.                         |
| **[Ardalis.GuardClauses](https://www.nuget.org/packages/Ardalis.GuardClauses)**                                                        | Provides guard clauses to enforce pre-conditions in methods, enhancing robustness and error handling. |
| **[Microsoft.AspNetCore.Identity](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity)**                                      | Manages user authentication, security, password hashing, and role management within the application.  |
| **[JWT Bearer Authentication](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer/9.0.0-preview.3.24172.13)** | Implements token-based authentication for securing API endpoints, requiring valid JWTs for access.    |
| **[xUnit](https://www.nuget.org/packages/xunit)**                                                                                      | Framework for unit testing, ensuring components function correctly in isolation.                      |
| **[Moq](https://www.nuget.org/packages/Moq)**                                                                                          | Mocking library used with xUnit to simulate behavior of dependencies during testing.                  |

## Getting Started

This section provides instructions on how to set up your environment and get the project up and running on your local machine for development and testing purposes.

### Prerequisites

Before you begin, ensure you have the following installed:

- .NET 8.0 SDK or later
- PostgreSQL server
- Git

1. **Clone the repository**:
   Open your terminal and execute the following command to clone the repositories:
   ```bash
   git clone frontend repo
   git clone backend repo
   ```
2. **Configure the application**:
   Navigate to the Web API layer in the back-end directory. Open the appsettings.json file and add the following configurations:
   ```bash
   "ConnectionStrings": {
    "Localhost": "<YOUR_LOCAL_DB_CONNECTION_STRING>"
   },
   "Secrets": {
    "JwtKey": "YourSecretKey",
    "Issuer": "YourIssuer"
   }
   ```
   > Replace <YOUR_LOCAL_DB_CONNECTION_STRING> with your actual PostgreSQL connection strings.
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
   http://localhost:<YOUR_LOCALHOST>/swagger/index.html
   ```
   > This URL directs you to the Swagger UI page that is automatically generated by the Swagger middleware in your ASP.NET Core application.

## Database Schema and ERD

Detailed database schema definitions including data types, constraints, and relationships. All the data is stored and retrieved using [PostgreSQL](https://www.postgresql.org/), a free and open-source relational database management system. ERD diagram was created using [Lucidchart](https://www.lucidchart.com/pages/landing?utm_source=google&utm_medium=cpc&utm_campaign=_chart_en_tier2_mixed_search_brand_exact_&km_CPC_CampaignId=1520850463&km_CPC_AdGroupID=57697288545&km_CPC_Keyword=lucidchart&km_CPC_MatchType=e&km_CPC_ExtensionID=&km_CPC_Network=g&km_CPC_AdPosition=&km_CPC_Creative=442433237648&km_CPC_TargetID=kwd-33511936169&km_CPC_Country=9072483&km_CPC_Device=c&km_CPC_placement=&km_CPC_target=&gad_source=1&gclid=CjwKCAjwrIixBhBbEiwACEqDJdqG-nDfXAHumE5poslMyIZM3meH7qJUs0CXQsOxBaL5fCNZwOUMURoC7MwQAvD_BwE).

![Database schema](/readmeImg/Diagram%20DB.png)

> Below is a table describing the relationships between the various entities in the ERD. Each table's primary key (PK) and foreign key (FK) relationships are denoted in the ERD, which establish the links between different entities.

| Entity 1     | Relationship | Entity 2         | Description                                                                                                                           |
| ------------ | ------------ | ---------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| `users`      | One to Many  | `reviews`        | Each user can write multiple reviews. A review must be associated with a user.                                                        |
| `products`   | One to Many  | `reviews`        | Each product can have multiple reviews. A review must be related to a product.                                                        |
| `products`   | One to Many  | `product_images` | Each product can have multiple product images. A product images must be related to a product.                                         |
| `categories` | One to Many  | `products`       | Each category can encompass multiple products. A product must belong to a category.                                                   |
| `users`      | One to Many  | `orders`         | Each user can place multiple orders. An order is linked to the user who placed it.                                                    |
| `orders`     | One to Many  | `order_items`    | Each order can contain multiple items. Order items are linked back to their respective orders.                                        |
| `products`   | One to Many  | `cart_items`     | Products can be part of multiple order items, showing the quantity and price per order. Each order item is associated with a product. |
| `users`      | One to One   | `carts`          | Each user has one cart associated with them. The cart holds the products that the user is currently interested in purchasing.         |
| `carts`      | One to Many  | `cart_items`     | Each cart can hold multiple items. Cart items link the product with its quantity in the user's cart.                                  |

## Backend Server with ASP.NET Core

### Architecture Diagram

![Architecture Diagram](/readmeImg/Ecommerce.png)

> The diagram represents the architecture of an Ecommerce platform built with ASP.NET Core and complied with the Clean Architecture. It is structured into several layers, each responsible for different aspects of the application:

- **Ecommerce.Core**: This layer includes classes and interfaces that define the basic entities like products, categories, users, and more. It also contains repository interfaces which abstract the data access logic.
- **Ecommerce.Service**: This layer contains services that handle business logic and operations, interacting with the Core layer to manipulate and retrieve data.
- **Ecommerce.Controller**: This layer is responsible for handling incoming HTTP requests and returning responses. It interacts with the Service layer to fetch and send data back to the client.
- **Ecommerce.WebAPI**: This houses the controllers and is the entry point for client interactions through HTTP requests. It includes middleware for error handling.

## API Endpoints

All the endpoints of the API are documented and can be tested directly on the generated Swagger page. From there you can view each endpoint URL, their HTTP methods, request body structures and authorization requirements. Access the Swagger page from this link.

## Key Features

## Unit Testing

Unit tests in this project are crucial for ensuring code quality and functionality. The **Ecommerce.Test** namespace contains tests that cover various components:

- **Testing Strategy**: Utilizes xUnit. Mocking is facilitated by libraries like Moq to simulate repository interactions.

## Repository Structure

My project is organized as follows to maintain a clean and navigable codebase:

```plaintext
|-- /Ecommerce
|   |-- /Ecommerce.Core
|   |   |-- /Common
|   |   |   |-- ProductQueryOptions.cs        // Defines options for querying products, such as pagination and filtering criteria.
|   |   |   |-- QueryOptions.cs               // Base class for query options, providing common properties like Sort.
|   |   |   |-- UserQueryOptions.cs           // Specialized query options for user-related data retrieval.
|   |   |   |-- UserCredential.cs             // Represents credentials for a user, used in authentication processes.
|   |   |-- /Entities
|   |   |   |-- BaseEntity.cs                 // Base class for all entities, containing common properties like Id.
|   |   |   |-- Category.cs                   // Represents a product category with properties like Name and Description.
|   |   |   |-- Product.cs                    // Defines a product with properties such as Name, Price, and CategoryId.
|   |   |   |-- ProductImage.cs               // Associated images for products, storing paths or image data.
|   |   |   |-- Review.cs                     // Customer reviews for products, includes Rating and Comment.
|   |   |   |-- TimeStamp.cs                  // Adds timestamps to entities, handled by TimeStampInterceptor for automatic updates.
|   |   |   |-- User.cs                       // User profile information, including credentials and roles.
|   |   |   |-- CartAggregate.cs              // Represents a shopping cart, including a collection of CartItems.
|   |   |   |-- OrderAggregate.cs             // Details an order, encapsulating order items and transaction data.
|   |-- /Interfaces
|   |   |-- /Interfaces
|   |   |   |-- IBaseRepository.cs            // Generic interface for CRUD operations applicable to all entities.
|   |   |   |-- ICartRepository.cs            // Specific operations for cart management not covered by the generic repository.
|   |   |   |-- ICategoryRepository.cs        // Custom repository actions for categories, like bulk update or specialized queries.
|   |   |   |-- IOrderRepository.cs           // Additional methods for managing orders, including status updates and history.
|   |   |   |-- IProductImageRepository.cs    // Handles operations specific to product image storage and retrieval.
|   |   |   |-- IProductRepository.cs         // Product-specific repository methods, such as searching by category.
|   |   |   |-- IReviewRepository.cs          // Methods for managing product reviews, including approval processes.
|   |   |   |-- IUserRepository.cs            // User-related data access functionalities, including search by role.
|   |   |-- /ValueObjects
|   |   |   |-- ProductSnapshot.cs            // A snapshot of product data at the time of transaction, used in order details.
|   |   |   |-- UserRole.cs                   // Defines roles within the system to manage access control levels.
|   |   |   |-- OrderStatus.cs                // Defines status of orders within the system to manage access control levels.
|   |   |-- /Exceptions
|   |       |-- ErrorDetails.cs               // Format for API error responses, includes status code and message.
|   |       |-- AppException.cs               // Custom exception type for application-specific errors, used for unified handling.
|   |
|   |-- /Ecommerce.Service
|   |   |-- /DTO
|   |   |   |-- CartDto.cs                    // Data transfer object for cart contents, includes list of CartItemDto.
|   |   |   |-- CartItemDto.cs                // Represents a single cart item in a transferable format, includes ProductId and Quantity.
|   |   |   |-- CategoryDto.cs                // Simplified category data for transfer, primarily used in listing endpoints.
|   |   |   |-- OrderDto.cs                   // Summary of an order for client applications, includes OrderItems and total cost.
|   |   |   |-- OrderItemDto.cs               // Details of an individual order item within an order.
|   |   |   |-- ProductDto.cs                 // Product data formatted for client delivery, includes descriptions and pricing.
|   |   |   |-- ProductImageDto.cs            // Data transfer format for images, may include a URL or binary data.
|   |   |   |-- ReviewDto.cs                  // Format for delivering review data to clients, includes user context and content.
|   |   |   |-- UserDto.cs                    // User profile information suitable for client-side use, includes user roles.
|   |-- /Services
|   |   |   |-- /Services
|   |   |   |-- AuthService.cs                // Manages authentication processes, including token generation and validation.
|   |   |   |-- BaseService.cs                // Base service providing common functionalities to all services, such as logging.
|   |   |   |-- CartItemService.cs            // Business logic related to individual cart items, such as additions and removals.
|   |   |   |-- CartService.cs                // Overall management of shopping carts, including session handling and persistence.
|   |   |   |-- CategoryService.cs            // Operations related to category management, from creation to modification.
|   |   |   |-- OrderItemService.cs           // Detailed logic for handling order items during the purchase process.
|   |   |   |-- OrderService.cs               // Coordinates all aspects of order processing, from placement to delivery.
|   |   |   |-- ProductImageService.cs        // Handles the storage and retrieval of product images.
|   |   |   |-- ProductService.cs             // Core service for product management, including inventory and updates.
|   |   |   |-- ReviewService.cs              // Manages the lifecycle of product reviews, including moderation.
|   |   |   |-- UserService.cs                // User profile and credential management, including registration and updates.
|   |   |   |-- TokenService.cs               // Generates and validates security tokens for user authentication.
|   |   |-- /Shared
|   |       |-- AutoMapperProfile.cs          // Configurations for AutoMapper to handle entity to DTO mappings efficiently.
|   |       |-- PasswordService.cs            // Provides password hashing and verification services.
|   |
|   |-- /Ecommerce.Controller
|   |   |-- /Controller
|   |   |   |-- AuthController.cs             // Handles authentication requests, like login and token refresh.
|   |   |   |-- CartController.cs             // API endpoints for cart interactions, such as adding or removing items.
|   |   |   |-- CategoryController.cs         // Provides API access to category data, including CRUD operations.
|   |   |   |-- OrderController.cs            // Manages order-related endpoints, from creation to status updates.
|   |   |   |-- ProductController.cs          // Controls product data exposure through the API, including search and details.
|   |   |   |-- ProductImageController.cs     // Endpoints for uploading and retrieving product images.
|   |   |   |-- ReviewController.cs           // API operations for managing reviews, including posting and editing.
|   |   |   |-- UserController.cs             // Handles user profile requests and user-specific data interactions.
|   |   |
|   |-- /Ecommerce.WebAPI
|   |   |-- /Repo
|   |   |   |-- CartItemRepository.cs         // Repository for handling operations specific to cart items.
|   |   |   |-- CartRepository.cs             // Manages data access related to the shopping cart.
|   |   |   |-- CategoryRepository.cs         // Accesses and manipulates category data in the database.
|   |   |   |-- ProductImageRepo.cs           // Handles the persistence of product images.
|   |   |   |-- ProductRepo.cs                // Facilitates data access for product management.
|   |   |   |-- ReviewRepository.cs           // Manages review data interactions.
|   |   |   |-- UserRepo.cs                   // Repository for user data access and manipulation.
|   |   |-- /Data
|   |   |   |-- AppDbContext.cs               // Entity Framework context, configures model relationships and database mappings.
|   |   |   |-- TimeStampInterceptor.cs       // EF Core interceptor to automatically update timestamp fields on save.
|   |   |-- /Middleware
|   |       |-- ExceptionMiddleware.cs        // Centralizes exception handling for the API, standardizing error responses.
|   |
|   |-- /Ecommerce.Test
|   |   |-- /UnitTests
|   |   |   |-- CartItemTests.cs              // Tests for cart item functionalities.
|   |   |   |-- CartTests.cs                  // Tests for cart functionalities.
|   |   |   |-- CategoryTests.cs              // Tests for category functionalities.
|   |   |   |-- OrderTests.cs                 // Tests for order functionalities.
|   |   |   |-- ProductImageTests.cs          // Tests for product image functionalities.
|   |   |   |-- ProductTests.cs               // Tests for product functionalities.
|   |   |   |-- ReviewTests.cs                // Tests for review functionalities.
|   |   |   |-- UserTests.cs                  // Tests for user functionalities.
|   |
|   |   |-- /Service
|   |   |   |-- CartItemServiceTests.cs       // Tests for cart item service operations.
|   |   |   |-- CartServiceTests.cs           // Tests for cart service operations.
|   |   |   |-- CategoryServiceTests.cs       // Tests for category service operations.
|   |   |   |-- OrderItemServiceTests.cs      // Tests for order item service operations.
|   |   |   |-- OrderServiceTests.cs          // Tests for order service operations.
|   |   |   |-- ProductImageServiceTests.cs   // Tests for product image service operations.
|   |   |   |-- ProductServiceTests.cs        // Tests for product service operations.
|   |   |   |-- ReviewServiceTests.cs         // Tests for review service operations.
|   |   |   |-- UserServiceTests.cs           // Tests for user service operations.

```

## Acknowledgements

- Integrify-Finland for providing the assignment.
- All contributors who help in enhancing and maintaining the project.

[**Return**](#technologies-and-libraries)
