
# 📦 Freelancer – Logistics Management System

## 📖 Overview

**Freelancer** is a logistics management system designed to streamline goods movement, minimize human errors, and provide transparency between users and agents.
It integrates user registration, order management, shipment tracking, vendor coordination, audit logs, and analytics into one centralized platform.

This project was developed for **PRJT-321** as part of the **Bachelor of Agricultural Information Technology** program at **Anand Agricultural University**.

---

## 🎯 Purpose

* Minimize errors related to user and agent transparency.
* Reduce time-consuming manual processes in logistics.
* Provide a platform for efficient record keeping and stakeholder reporting.
* Integrate B2C logistics tasks for optimal use of resources and better delivery services.

---

## 🏆 Objectives

* Manage **user/client workflows** in a logistics management system.
* Provide a **document management** solution.
* Automate manual tasks such as **inventory tracking**, **order management**, and **shipment scheduling**.
* Reduce human errors in day-to-day logistics operations.

---

## 🛠 Modules

### 1. User Registration & Login

* Secure user accounts for personalized experiences.
* Role-based redirection (Admin, Freelancer, Employer).

### 2. Order Management

* Create, update, and delete shipment orders.
* Real-time order status tracking (Pending, In Transit, Delivered, Canceled).

### 3. Vendor & Partner Management

* Manage suppliers, carriers, and service providers.

### 4. Customer Portal

* Shipment tracking.
* Order placement & feedback submission.
* Complaint and issue support.

### 5. Administrative Features

* Role-based permissions.
* Restrict unauthorized activity.

### 6. Audit Logging

* Store login and logout records.

### 7. Analytics & Reporting

* Export reports such as **shipment summaries** and **cost analyses**.

---

## 💻 Technology Stack

| Category     | Tools                                              |
| ------------ | -------------------------------------------------- |
| **Platform** | Visual Studio 2022                                 |
| **Frontend** | HTML, CSS, Bootstrap, JavaScript, ASP.NET MVC (C#) |
| **Backend**  | SQL Server (Local DB)                              |
| **OS**       | Windows 10                                         |
| **Hardware** | 4GB/8GB/16GB RAM, Intel i5/i7, x64 processor       |

---

## 📂 Database Schema

### 1. **User Table**

```sql
User_ID INT PRIMARY KEY AUTO_INCREMENT,
Username VARCHAR(255) UNIQUE NOT NULL,
Email VARCHAR(255) UNIQUE NOT NULL,
Password VARCHAR(255) NOT NULL,
Registration_Date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
FullName VARCHAR(30) NOT NULL,
Bio TEXT(200) NOT NULL,
Contact VARCHAR(20) NOT NULL,
Skills VARCHAR(200) NOT NULL,
Profile_Picture VARCHAR(255),
User_Type ENUM('Admin', 'Freelancer', 'Employer') NOT NULL
```

### 2. **Freelancer Table**

```sql
Freelancer_ID INT PRIMARY KEY,
Skills TEXT(150),
Availability ENUM('Available', 'Unavailable'),
Experience_Years INT,
FOREIGN KEY (Freelancer_ID) REFERENCES User(User_ID)
```

### 3. **Project Table**

```sql
Project_ID INT PRIMARY KEY AUTO_INCREMENT,
Client_ID INT,
Title VARCHAR(255) NOT NULL,
Description TEXT(150) NOT NULL,
Budget DECIMAL(10,2) NOT NULL,
Bid_Date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
Start_Date DATE NOT NULL,
End_Date DATE NOT NULL,
Status ENUM('Open', 'In Progress', 'Completed'),
FOREIGN KEY (Client_ID) REFERENCES User(User_ID)
```

### 4. **Bids Table**

```sql
Bid_ID INT PRIMARY KEY AUTO_INCREMENT,
Project_ID INT,
Freelancer_ID INT,
BidAmount DECIMAL(10,2),
Bid_Date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
Assign VARCHAR(10),
FOREIGN KEY (Project_ID) REFERENCES Project(Project_ID),
FOREIGN KEY (Freelancer_ID) REFERENCES Freelancer(Freelancer_ID)
```

### 5. **Audit Table**

```sql
Audit_ID INT PRIMARY KEY AUTO_INCREMENT,
User_ID INT NOT NULL,
Login_Time TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
Logout_Time TIMESTAMP,
FOREIGN KEY (User_ID) REFERENCES User(User_ID)
```

### 6. **Task Table**

```sql
Task_ID INT PRIMARY KEY AUTO_INCREMENT,
Project_ID INT,
Freelancer_ID INT,
Title VARCHAR(255) NOT NULL,
Status ENUM('To Do', 'In Progress', 'Completed'),
Due_Date DATE NOT NULL,
Completion_Date DATE,
FOREIGN KEY (Project_ID) REFERENCES Project(Project_ID),
FOREIGN KEY (Freelancer_ID) REFERENCES Freelancer(Freelancer_ID)
```

### 7. **Payment Table**

```sql
Payment_ID INT PRIMARY KEY AUTO_INCREMENT,
Project_ID INT,
Payer_ID INT,
Payee_ID INT,
Amount DECIMAL(10,2) NOT NULL,
Payment_Date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
Payment_Method VARCHAR(255) NOT NULL,
Transaction_ID VARCHAR(255) NOT NULL,
Status ENUM('Pending', 'Completed'),
FOREIGN KEY (Project_ID) REFERENCES Project(Project_ID),
FOREIGN KEY (Payer_ID) REFERENCES User(User_ID),
FOREIGN KEY (Payee_ID) REFERENCES User(User_ID)
```

---

## 📊 User Roles & Features

### **Admin**

* Manage all users and projects.
* View audit logs.
* Generate reports.

### **Employer**

* Post projects and manage bids.
* Create tasks.
* Make payments to freelancers.
* Manage profile information.

### **Freelancer**

* View available projects.
* Place bids.
* Complete tasks.
* Receive payments.

---

## 📸 Screenshots

* **Register Page** – New user account creation.
* **Login Page** – Role-based authentication.
* **Admin Dashboard** – View all users, projects, and audit logs.
* **Employer Dashboard** – Add projects, view own projects, approve bids.
* **Freelancer Dashboard** – Available projects, live projects, completed projects.
* **Payment Screens** – Initiate and confirm payments.

---

## 📅 Project Timeline

* **Start Date:** 2024-2025 Academic Year (Semester VI)
* **Submission Date:** 21-03-2025

---

## 👨‍💻 Author

**Chaudhary BharatBhai .B**
Bachelor of Agricultural Information Technology
Anand Agricultural University

---

If you want, I can now **add a visual architecture diagram + DFD + ERD images directly into this README** so it’s 100% complete for GitHub.
That would make it look professional and ready to submit or showcase.
