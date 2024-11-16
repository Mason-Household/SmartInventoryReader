# SmartInventoryReader 📸📦

**A camera-enabled scanner that prioritizes barcodes, QR codes, and general image searches to determine an item's price before giving the user the choice to add it to their manageable inventory.**

---------------------------------------------
---------------------------------------------

## Technologies Used 🛠️

- NET 8.0: Backend API
- React + TypeScript + Vite: Frontend application
- Serilog: Logging
- Entity Framework Core: Database ORM
- PostgreSQL: Database
- Docker: Containerization
- FastAPI: LLM Service
- OpenCV, EasyOCR, Transformers: Image processing and recognition

---------------------------------------------
---------------------------------------------

## Prerequisites 📋

- Docker: Ensure Docker is installed and running.
- NET SDK 8.0: Install from here.
- Node.js: Install from here.

---------------------------------------------
---------------------------------------------

## Running the Project 🚀

1. Clone the Repository 🧑‍💻

    ```powershell
        git clone https://github.com/Mason-Household/SmartInventoryReader.git
        cd SmartInventoryReader
    ```

2. Set Up Environment Variables 🌍

    - Create a .env file in the root directory and add the following:

    ```powershell
        JWT_SECRET_KEY=your_secret_key
        JWT_ISSUER=your_issuer
        JWT_AUDIENCE=your_audience
        JWT_EXPIRATION=your_expiration_time
    ```

3. Build and Run with Docker 🐳

    ```powershell
        docker-compose up --build -d
    ```

4. Access the Services 🌐

- Backend API: <http://localhost:5000>
- Frontend: <http://localhost:3000>
- LLM Service: <http://localhost:8080>

---------------------------------------------
---------------------------------------------

## Authentication 🔒

- The application uses Hugging Face tokens for authentication.
- To obtain a Hugging Face token, visit [Hugging Face Token](https://huggingface.co/settings/tokens) and generate a new token.
- Ensure you have set the environment variables correctly in the .env file with your Hugging Face token.
- The application also supports logging in with Google or Apple, as well as using your email and password.
- For Google or Apple login, follow the respective OAuth2 flow and set the required environment variables for client IDs and secrets.
- The tokens are used to secure the API endpoints and manage user sessions.

---------------------------------------------
---------------------------------------------

## Running Individual Parts 🧩

- Backend API 🖥️
- Navigate to the Backend directory:

    ```powershell
        cd backend
    ```

- Restore dependencies, build, and run:

    ```powershell
        dotnet restore; dotnet build; dotnet run
    ```

### Frontend Application 🌐

- Navigate to the PrettyScanner directory:

    ```powershell
        cd PrettyScanner
    ```

- Instqall dependencies and start the development server:

    ```powershell
        npm install; npm run dev
    ```

### LLM Service 🤖

- Navigate to the LLM directory:

    ```powershell
        cd LLM
    ```

- Install dependencies and run the service:

    ```powershell
        pip install -r requirements.txt; uvicorn main:app --reload
    ```

---------------------------------------------
---------------------------------------------

## Troubleshooting 🛠️

- Common Issues and Solutions:

- Docker Build Failures:
  - Ensure Docker is running.
  - Check for any syntax errors in Dockerfile or docker-compose.yml.

- Database Connection Issues:
  - Verify PostgreSQL is running.
  - Check connection strings in the environment variables.

- Frontend Not Loading:
  - Ensure all dependencies are installed.
  - Check the console for any errors.

- Backend API Errors:
  - Check the logs for detailed error messages.
  - Ensure all migrations are applied.

- Logs and Monitoring 📊
  - Backend Logs: Check the logs in the Backend directory.
  - Frontend Logs: Use the browser console for any frontend errors.
- LLM Service Logs: Check the terminal where the service is running.

---------------------------------------------
---------------------------------------------

## Why These Technologies? 🤔

- .NET 8.0: Provides a robust and scalable backend framework.
- React + TypeScript + Vite: Ensures a fast and modern frontend development experience.
- Serilog: Offers flexible and structured logging.
- Entity Framework Core: Simplifies database interactions.
- PostgreSQL: A powerful, open-source relational database.
- Docker: Ensures consistent environments across different stages of development.
- FastAPI: Provides a high-performance framework for the LLM service.
- OpenCV, EasyOCR, Transformers: Enables advanced image processing and recognition capabilities.

---------------------------------------------
---------------------------------------------

## Contributing 🤝

**I welcome a contribution! If you can do it better, especially the front end, please submit a PR and I'll review it with appreciation!!!**

- License 📜

    ```txt
        This project is licensed under the MIT License.
    ```
