# Course API

A professional RESTful Web API built with **ASP.NET Core** and **Entity Framework Core** for managing courses, students, teachers, and course enrollments.

This project was developed to apply backend development concepts including API design, database modeling, Entity Framework Core relationships, authentication, authorization, and clean code practices.

---

## Project Overview

Course API is a backend application that provides a complete API for managing an educational course system.

The system supports managing:

* Courses
* Students
* Teachers
* Student enrollments
* Course prerequisites

The API uses **SQL Server** as the database and **Entity Framework Core** for data access following the Code First approach.

---

## Technologies

* ASP.NET Core Web API (.NET 10)
* C#
* Entity Framework Core
* SQL Server
* LINQ
* Swagger / OpenAPI
* JWT Authentication
* Dependency Injection
* Middleware
* DTOs and ViewModels

---

## Features

### Course Management

* Get all courses
* Get course by ID
* Create a new course
* Update course information
* Delete courses
* Search courses by title
* Filter courses by price

### Student Management

* Create and manage students
* Enroll students in courses
* View student enrolled courses
* Track enrollment details and status

### Teacher Management

* Create and manage teachers
* Assign teachers to courses

### Course Relationships

* Manage course prerequisites
* Handle many-to-many relationships between students and courses
* Configure entity relationships using Entity Framework Core

### Security and Validation

* Data validation
* JWT-based authentication
* Role-based authorization support
* JSON serialization configuration
* Logging support

---

## Database Design

The project uses Entity Framework Core with the Code First approach.

Main entities:

* Student
* Teacher
* Course
* StudentCourse
* CoursePrerequisite

Relationships:

* One-to-Many relationship between Teacher and Courses
* Many-to-Many relationship between Students and Courses
* Course prerequisite relationships

---

## Project Structure

```
CourseApi
│
├── Controllers
├── Data
├── Models
├── DTOs
├── ViewModels
├── Services
├── Extensions
├── Enums
└── Migrations
```

---


## Author

Saly Najjar

