---
title: "Welcome"
description: This is the main page for the documentation
---

# Web API For VK

A project for VKontakte. This service can add new states and groups for users, add new users, block users, retrieve users with pagination, and get a single user by ID.

## Documentation

Workflow:

First, an admin must be manually created in the database. Then, the admin should create the necessary roles and groups. The server will receive JSON data of users with fields like id, login, password, created_date, user_group_id, and user_state_id. It is assumed that the user will initially have a "blocked" state by default. The user will then be added to the database, and after 5 seconds, they will be activated. To retrieve or block a single user, their ID must be provided. To retrieve multiple users, a request with pageNumber and pageSize should be sent. The database used is PostgreSQL.

Database setup:

The ConnectionString is located in `appsettings.Development.json`. You need to enter your database connection string there. The code-first approach is used, but the Database-first approach can also be used.

## Comments

Password: The password will likely be hashed on the client side; theoretically, hashing can also be added on the backend.

User Activation: In a real scenario, I would create a separate status to indicate unverified users. Upon registration on the site, the user would be assigned this status. A separate method would add the user to the database, and the registration method would be activated by another request to the server. I would also write a separate service that would periodically delete users who have not been verified.

Tests: In my opinion, unit testing this service is pointless, as the application's logic is solely based on database operations. In a real scenario, I would resort to other types of tests.

## Authors

<a type="account" href="https://github.com/BR7RKR">@BR7RKR</a>