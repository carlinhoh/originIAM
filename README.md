# Origin Identity Acess Management

## Overview

This repository contains the first version of the OriginIAM implementation. This service is part of a microservices architecture designed to manage both direct user sign-ups and employer-based user eligibility.

![System Architecture](basic_version_OriginIAM.png)

In this architecture, the User and Employer services are created to facilitate testing by creating a mini storage in memory using a dictionary. The architecture has a redis to cache eligible employers and speed up the signup request and a postgresql to store a report of the last eligibilityFile processing run. To make testing easier, the reports can be retrieved via get from the eligibility file controller. The input data has syntactic validation in the api layer and semantic validation in the other layers. 

The return from the eligibility api will provide the link to navigate to the previous result of processing the file. Current processing is approximately ~300 lines per second. The backlog includes proposals to improve the performance and use of the database.
 

## API Documentation

### Signup API

- **Endpoint**: `POST /signup`
- **Description**: Registers a new user.
- **Request Body**:
  ```json
  {
    "email": "user@example.com",
    "password": "MFx6|>V;)eo)/?NwEH<aiV#mrNrr1ANr0[>##h!iTn)baIY[>I",
    "country": "st",
    "acceptTerms": true,
    "fullName": "string",
    "birthDate": "2024-04-12"
  }
  ```

### Eligibility File API

- **Endpoint**: `POST /eligibility`
- **Description**: Processes a CSV file from a given URL to manage user access based on employer records.
- **Request Body**:
  ```json
  {
    "file": "http://url-to-csv.com/file.csv",
    "employer_name": "Torvalds"
  }
  ```
To meet the needs of the different types of companies we can integrate, two endpoints have been designated:

- `POST /eligibilityfile/small-company-eligibility` - will handle requests from companies up to 10,000 employees. 
- `POST /eligibilityfile`  will handle requests from any group of employees.

This differentiation was made simply to create the opportunity to return the processing report in real time to our customers who have small companies. Even so, it is possible for them to retrieve the report later without any problems. 


- `GET /EligibilityFile/reports?employerName=<CompanyName>&pageNumber=1&pageSize=10`

 
## Getting Started

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/origin-user-access.git
   ```
2. Navigate to the project directory:
   ```bash
   cd originIAM
   ```
3. Run the application:
   ```bash
   docker-compose up --build
   ```

## Infra Check

Database: ```bash
			docker exec -it <container_id> psql  -U user -d origin_db
			```

Redis: 	```bash
			docker exec -it <container_id> redis-cli
			```		


### Running Tests
Execute the following command to run the automated tests:
```bash
dotnet test
```

## Usage
Locally (IIs) - Refer to the [Swagger UI](http://localhost:44361/swagger) for testing and interacting with the API after running the application.

## Blobs for test

Liskov Company: [Liskov](https://eligibilityorigin42.blob.core.windows.net/eligibility-files/liskov.csv?sp=r&st=2024-04-11T16:40:16Z&se=2024-04-19T00:40:16Z&spr=https&sv=2022-11-02&sr=b&sig=ZjOHPVrMcYTx%2Fsmq8l4LZR2nvkyT0Kf17CXayGLBDSk%3D) - 3 employees, 0 errors

Dijkstra Company: [Dijkstra](https://eligibilityorigin42.blob.core.windows.net/eligibility-files/eligibility_file_dijkstra_small.csv?sp=r&st=2024-04-12T17:43:02Z&se=2024-04-20T01:43:02Z&spr=https&sv=2022-11-02&sr=b&sig=Dp1oOVNXpke8hGAeZmPheNmihlbX9i9pLJuu%2FKyuFJU%3D) - 13 employees, 0 errors

Example Company: [Example](https://eligibilityorigin42.blob.core.windows.net/eligibility-files/eligibility_file_company2.csv?sp=r&st=2024-04-12T17:43:51Z&se=2024-04-20T01:43:51Z&spr=https&sv=2022-11-02&sr=b&sig=6jWvpMu0%2FWfFlHDpsh7mM0B5pEDMjmRMJK%2FiiqwpVzA%3D) - 67 employees, 8 errors

Dijkstra Company (Larger): [Dijkstra](https://eligibilityorigin42.blob.core.windows.net/eligibility-files/eligibility_file_dijkstra_50mb.csv?sp=r&st=2024-04-09T20:03:14Z&se=2024-04-17T04:03:14Z&spr=https&sv=2022-11-02&sr=b&sig=cnjhFdzjxnxP49GP1n94qAsJ8pvWHJp0u6iTZgMuhSc%3D) - 500000 employees, 0 errors


## Backlog

- Data flow draw;
- Password Cryptography;
- Controller documentation;
- Log;
- Test controllers;
- Consider Standardization of methods =>;
- DI layer;
- Data annotation for Dates;
- 1 GB file test;
- History of eligibility File reports;
- Dynamic csv delimiter;
- Transaction for database methods;
- Rate limiting in the signup controller;
- Lists of Known Passwords - Even if it passes the password check, treat the password as weak;
- Doc with examples of req - res;
- Versioning dtos, models and controllers;
- Monitoring memory usage in the application container;
- Parallel processing of the csv file (with memory management so as not to exceed a reasonable limit);
- Batch trips to the bank to save reports (Reducing the number of insertions in the database can make the process much faster. It's currently going line by line).
- Automated tests with locally mocked up csv files;
- System and Health routes (HealthCheck, Echotest, LogSeverity);
- Folder with architecture decision records;
- Hateoas;
- Test application with Urls that return Html (https://youtube.com for example);
- Documentar expressões regulares;
- Definition of indicators - Response time, availability, SLA for processing large files;
- Load Testing;


## Examples for test

**request:**

  ```json
	{
	  "fileAddress": "https://eligibilityorigin42.blob.core.windows.net/eligibility-files/liskov.csv?sp=r&st=2024-04-11T16:40:16Z&se=2024-04-19T00:40:16Z&spr=https&sv=2022-11-02&sr=b&sig=ZjOHPVrMcYTx%2Fsmq8l4LZR2nvkyT0Kf17CXayGLBDSk%3D",
	  "employerName": "liskov"
	}
  ```


  ```json
	{
	  "fileAddress": "https://eligibilityorigin42.blob.core.windows.net/eligibility-files/eligibility_file_dijkstra_small.csv?sp=r&st=2024-04-11T19:44:58Z&se=2024-04-19T03:44:58Z&spr=https&sv=2022-11-02&sr=b&sig=Q3GERhrpxvNj1KSNKCTfbZVeMuynjc%2FDVPrGFyfVlnc%3D",
	  "employerName": "dijkstra"
	}
  ```
  
  
  ```json
	{
	  "fileAddress": "https://eligibilityorigin42.blob.core.windows.net/eligibility-files/eligibility_file_company2.csv?sp=r&st=2024-04-12T17:43:51Z&se=2024-04-20T01:43:51Z&spr=https&sv=2022-11-02&sr=b&sig=6jWvpMu0%2FWfFlHDpsh7mM0B5pEDMjmRMJK%2FiiqwpVzA%3D",
	  "employerName": "Example"
	}
  ```
