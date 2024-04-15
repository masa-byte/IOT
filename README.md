# Microservice Application for IOT 
This is part one of a project for course **Internet of Things and Services**
<br>
<br>

### Application type
This application is tested through clients like **Postman**, and **Insomnia** and **Swagger**. It is a **microservice** application, meaning it is divided into multiple services, each with its own responsibility.
<br>

### Idea
The idea is to have two microservices which will communicate with each other using gRPC, and one of them will have a REST API for communication with client. 

### Architecture
The first microservice was written in **.NET Core** and the second one in **Flask**. The first microservice is acting as a gRPC client, and a REST server, and the second one is acting as a gRPC server. The first microservice is responsible for handling the data from the second microservice, and the second microservice is responsible for handling the data from the database.

### Data
The database is **PostgreSQL**. The dataset used is https://www.kaggle.com/datasets/ogbuokiriblessing/sensor-based-aquaponics-fish-pond-datasets?select=IoTPond10.csv. The dataset is about the sensor data from an aquaponics fish ponds. The data is about the temperature, pH, oxygen level, etc. in the water. I have cleaned the data and added pond_id to the data, and when iimporting the data, entry_id will be overridden to keep consistency and unique keys. The data is stored in the database, and the second microservice is responsible for handling the data from the database.

### Tech stack
It was written in **.NET Core** and **Flask**, and uses **PostgreSQL** as the database, and **gRPC** and **REST**.
<br>
<br>

### How to run the application
1. Clone the repository
2. Open the repository in Visual Studio Code
3. Open the terminal in Visual Studio Code
4. Run the following command:
    - `docker-compose up`
5. Wait for the services to start
6. Run the import-data.py script to import the data into the database
7.1 You can test gRPC server on its own through Postman or Insomnia with url localhost:5117
7.2 In order to test aggregation functions, use the provided calculate-secoonds.py script to get the seconds for the given time period
8. To test the REST API, go to http://127.0.0.1:5000/apidocs/
9. Database GUI is available on http://localhost:5050/ 