# AutoBalancer

AutoBalancer is a utility designed to automatically reconcile erroneous transactions in the Microsoft Access database utilized by POS terminals. This tool streamlines the correction process, ensuring data integrity and minimizing manual intervention.

## Table of Contents
Overview
Features
Installation
Usage
Configuration


# Overview

POS terminals often encounter transaction discrepancies that require manual correction in the backend database. AutoBalancer automates this process by identifying and rectifying bad transactions, thereby enhancing operational efficiency and reducing human error.

# Features

Automated Transaction Reconciliation: Automatically detects and corrects erroneous transactions in the database. 
User-Friendly Interface: Provides an intuitive graphical user interface for ease of use.
Configurable Settings: Allows customization of reconciliation parameters to suit specific requirements.
Logging and Reporting: Generates logs and reports for audit and troubleshooting purposes.

## Installation

### Prerequisites
Microsoft Access Database (MDB or ACCDB format)
.NET Framework 4.5 or higher

## Steps

Clone the repository:
git clone https://github.com/Yongbi21/AutoBalancer.git
Open the solution file AutoBalancer.sln in Visual Studio.
Build the solution to restore dependencies and compile the application.
Navigate to the output directory and run AutoBalancer.exe.

## Usage
Launch the application.
Connect to the target Microsoft Access database by selecting the appropriate file.
Configure the reconciliation settings as per your requirements.
Click on the "Start" button to begin the automated reconciliation process.
Monitor the progress through the status bar and review the generated reports upon completion.

## Configuration

Configuration settings can be adjusted within the application interface. Key parameters include:
Transaction Thresholds: Define acceptable ranges for transaction amounts.
Reconciliation Rules: Set criteria for identifying and correcting erroneous transactions.
Logging Options: Choose the level of detail for generated logs.
For advanced configurations, refer to the App.config file located in the project directory.
