# Programme Management System (PMS)

## 📋 Overview

The Programme Management System (PMS) is a web-based application designed to help programme coordinators centrally manage academic operations. Built with **ASP.NET Core MVC** and **Entity Framework Core**, it provides a clean, intuitive interface for handling all aspects of academic programme administration.

## 🚀 Features

### Functionality

#### 👨‍🎓 Student Management
- Track personal details (first name, last name, email, phone number)
- Manage year of study (Years 1-4)
- View registered modules for each student
- Full CRUD operations with email duplicate validation
- Cascade delete with dependency warnings

#### 👨‍🏫 Lecturer Management
- Manage lecturer information and department affiliations
- Track email addresses with duplicate prevention
- View assigned modules per lecturer
- Full CRUD operations with dependency checking

#### 📚 Module Administration
- Create and manage academic modules with credit values (1-30 credits)
- Unique module code validation
- Track academic year offering
- View assigned lecturer and registered students
- Full CRUD operations

#### 📝 Student Module Registration
- Enroll students in modules with duplicate prevention
- Automatic registration date tracking
- View all registrations with student and module details
- Unregister students with cascade delete support

#### 🔗 Module Assignment (Lecturer Allocation)
- Assign lecturers to modules with duplicate prevention
- View all assignments with lecturer and module details
- Update existing assignments
- Remove assignments safely
