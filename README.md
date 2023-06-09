﻿# ASP.NET Library API on Clean Architecture using Mediator and CQRS
# Libro
This repository contains an ASP.NET library API developed using the Clean Architecture design principles. 
The Clean Architecture pattern helps in achieving separation of concerns,
maintainability, and testability in software projects. also
It utilizes the Mediator pattern for handling commands and 
queries and applies the CQRS (Command Query Responsibility Segregation)
principle for separating the read and write operations.

Getting Started
To get started with the ASP.NET Library API on Clean Architecture, follow the instructions below:

# Table of Contents
1.  [Introduction](#introduction)
2.  [Features And Technologies](#features-and-technologies)
3.  [Architecture Overview](#architecture-overview)
4.  [Getting Started](#getting-started)
5.  [Dependencies](#dependencies)
6.  [Usage](#usage)
7.  [Testing](#testing)
8.  [Contributing](#contributing)

## Introduction

The ASP.NET Library API is a library management system built using ASP.NET,
adhering to the principles of Clean Architecture. The API provides a set of endpoints for managing books,
patrons, and borrowing transactions within a library context.

This repository contains an ASP.NET library API developed using the Clean Architecture design principles.
The Clean Architecture pattern helps in achieving separation of concerns, maintainability, 
and testability in software projects.

This API aims to provide a clean and maintainable architecture that is easy to extend, test, and evolve over time.
By following the principles of Clean Architecture, the codebase is structured into distinct layers, with clear separation of concerns,
allowing for flexibility and scalability.

## Features and Technologies

The ASP.NET Library API offers the following key features:

-   **Authentication**: Secure API endpoints using JWT authentication

JSON Web Token (JWT) was used rather than sessions because it's more simple since it's stateless and more suitable for APIs,
Since there's no need to connect to a central Authentication provider to make sure if the user is Authenticated
 
-   **Authorization**: Secure API endpoints by Roles (Patron, Librarian and admin) and policies can be created and assigned to users by admin.

custom filter using Authorization by roles using authorized attribute for the roles then check if the role is given for the user by checking the
database or by checking the claims if we want to make less requests on the server, in this project I check the database since it's a demo project
so no profermoce concern for a demo project 

-   **Validation**: Perform input validation to ensure data integrity.

using fluent validation

-   **Error Handling**: Handle and return meaningful error messages by custom user exceptions for various scenarios.
-   **Book Management**: libraians can Create, retrieve, update, and delete books .
-   **Author Management**: libraians can Manage Authors, including creation, retrieval, updates.
-   **Patron Management**: libraians can Manage library patrons, including creation, retrieval, and updates also viewing their borrowing history.
-   **librarian Management**: Manage library librarians, including creation, retrieval, and updates.
-   **User Management**: users can Manage their own profile and check their own borrowing history.
-   **Borrowing Transactions**: Facilitate borrowing and returning of books, including due date management and transaction history.
-   **Book Reviews**: Patron can review and rate the book they have borrowed and returned.
-   **PlayList Management**: Manage PlayLists, including creation, retrieval, updates, Adding and Removing book from play list.
-   **Notification**: daily Scheduled Notifications sent to the patron about their due date borrowed books,
also Librairans can send notifications to patron about their reserved book and due date borrowed books.

the notification sent in three ways (Email, database and Push Notification)

for **email notifications** FluentEmail package was used rather than MailKit or Amazon Simple Email Service (SES), in fact after trying SES it was easy to use and configure and works just fine 
also it's more suitable for real applications with email verification, on the other hand MailKit has more Functionality and Flexibility, however since it's
a demo project which prioritize simplicity and ease of use, the FluentEmail was the best fit

for **Push notifications** signalR was used, according to the requirements it was best fit for sending push notifications -
Real time communication and live updates, SNS or SQS can be used but they are more suitable for Distributed systems, microservices architectures, 
Pub/sub messaging, broadcasting messages and Reliable message queuing in SQS

for **scheduling** HangFire was used due to the simplicity of the project since it notify patrons once a day about their reserved and due date books
though Quartz provides more extensive features and greater flexibility making it more suitable for complex and customize scheduling 


-   **Recommended Books**: Patrons will be able to retrieve recommeded books based on their borrowing hostory by favourite author and genre .

## Architecture Overview

The ASP.NET Library API follows the Clean Architecture principles, which promote a clear separation of concerns and modular design. The architecture is organized into the following layers:

1.  **Presentation Layer**: This layer contains the API controllers responsible for handling incoming HTTP requests and returning appropriate responses. It is the entry point for external clients.
    
2.  **Application Layer**: This layer contains application-specific business logic and use cases. It coordinates the interactions between the Presentation and Domain layers.
    
3.  **Domain Layer**: This layer represents the core of the application and encapsulates the business rules and domain models. It contains entities, value objects, and domain services.
    
4.  **Infrastructure Layer**: This layer provides implementation details and external dependencies. It includes database access, external services, and other infrastructure-related concerns.
    

The use of interfaces and dependency injection ensures loose coupling between layers, making it easier to replace or modify components without affecting other parts of the application.

though it is a demo project other architectures can be used such as Domain-Driven Design(DDD), Model-View-Controller (MVC), or others.
though clean architecture emphasizes on separation of concerns, maintainability and testability.
 Clean Architecture offers many benefits however might not be necessary or suitable for all projects, it would me more suitable 
 to achieve separation of concerns, maintainability and testability on large and complex projects however in this demo project the 
 clean architecture was chosen to practice new technology and gain knowledge in addition to learn more technologies to cultivate my skills

The combination of the **CQRS (Command Query Responsibility Segregation)
pattern** and the **Mediator pattern** can provide several benefits in software development such as achieving solid principles such as 
single Responsibility, in addition to achieve Separation of Concerns, Scalability, Complex Interactions and Testing though they have benefits in certain scenarios,
they may not be necessary or suitable for every application.

## Getting Started

To get started with the ASP.NET Library API, follow these steps:

1-install docker 

    **windows** : https://docs.docker.com/desktop/install/windows-install/

    **linux** : by apt 

2.  Clone the repository: `git clone <repository-url>`

3.  Navigate to the project directory: `cd .\Libro`

4.  Install the dependencies: `dotnet restore`

5.  Set up the necessary secrets in the `appsettings.json` file.

6. run the database image 

- ##### for development (inside the container) : `docker compose -f .\docker-compose.development.yaml up`

- ##### for production (inside the container) : `docker compose up`

7.  Run database migrations :  `dotnet ef database update`

8.  Build and run the application

- ##### for development  : `dotnet run`

- ##### for production (inside the container) : already did -> `docker compose up`

9.  Access the API

**for development** at `https://localhost:5003` (or the specified port).

**for production** (inside the container) at `http://localhost:5000` (or the specified port).

## Dependencies

The ASP.NET Library API has the following dependencies:

-   ASP.NET Core (version 6.X.X)
-   Entity Framework Core (version 6.X.X)
-   Swagger UI (version 3.0.X)
-  docker
-   Other NuGet packages as specified in the project files.

Ensure that these dependencies are properly installed and up-to-date before running the application.


## Usage

Once the API is up and running, you can access the available endpoints using a tool like cURL, Postman, or any other HTTP client. The API documentation and interactive testing can be accessed via Swagger UI, which is available at `http://localhost:5000/swagger`.

also you can find PDF documentaion for the API [here](https://drive.google.com/file/d/10067-NdgVWR-Zde0LVEn4ps_i7SZlJjR/view?usp=sharing)


Before accessing the protected endpoints, make sure to obtain an authentication token by calling the appropriate authentication endpoint (e.g., `/Authentication/Login`). Include the token in the headers of subsequent requests for authorized operations.

## Testing
- **Unit Testing** : the units functionality in the Commands and Queries were testing with mocked repositories using xUnit and Moq

- **API-Integration Testing** : the controllers - api end points was tested through HTTP Client using In Memory Database under the "Integration testing" Environment with xUnit

The project includes unit tests and integration tests to ensure the correctness of the 
implemented features and behavior. The unit tests is located in the /Libro.Test and the API tests is 
located in /Libro.ApiTest, can be run using your preferred test runner or the `dotnet test` command.

## Contributing

Contributions to the ASP.NET Library API are welcome! If you find any issues or have suggestions for improvements, please submit a pull request or create an issue on the project repository.
