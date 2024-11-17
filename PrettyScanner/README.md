# PrettyScanner 📸📦

**A camera-enabled scanner that prioritizes barcodes, QR codes, and general image searches to determine an item's price before giving the user the choice to add it to their manageable inventory.**

---------------------------------------------
---------------------------------------------

## Technologies Used 🛠️

- React + TypeScript + Vite: Frontend application
- Firebase: Authentication and storage
- Lucide Icons: Icons for UI
- Material-UI: UI components
- Docker: Containerization

---------------------------------------------
---------------------------------------------

## Prerequisites 📋

- Docker: Ensure Docker is installed and running.
- Node.js: Install from here.

---------------------------------------------
---------------------------------------------

## Running the Project 🚀

1. Clone the Repository 🧑‍💻

    ```powershell
    git clone https://github.com/Mason-Household/SmartInventoryReader.git
    cd SmartInventoryReader/PrettyScanner
    ```

2. Set Up Environment Variables 🌍

    Create a `.env` file in the `PrettyScanner` directory and add the following:

    ```env
    VITE_FIREBASE_API_KEY=your_api_key
    VITE_FIREBASE_AUTH_DOMAIN=your_auth_domain
    VITE_FIREBASE_PROJECT_ID=your_project_id
    VITE_FIREBASE_STORAGE_BUCKET=your_storage_bucket
    VITE_FIREBASE_MESSAGING_SENDER_ID=your_messaging_sender_id
    VITE_FIREBASE_APP_ID=your_app_id
    VITE_FIREBASE_MEASUREMENT_ID=your_measurement_id
    VITE_API_URL=http://localhost:5000
    ```

3. Build and Run with Docker 🐳

    ```powershell
    docker-compose up --build -d
    ```

4. Access the Frontend 🌐

- Frontend: <http://localhost:3000>

---------------------------------------------
---------------------------------------------

## Running the Frontend Application Locally 🌐

- Create a firebase project and place the firebase.ts file in PrettyScanner/src/config/firebase.ts for authentication

- Navigate to the PrettyScanner directory:

    ```powershell
    cd PrettyScanner
    ```

- Install dependencies and start the development server:

    ```powershell
    npm install; npm run dev
    ```

---------------------------------------------
---------------------------------------------

## Troubleshooting 🛠️

- Common Issues and Solutions:

- Docker Build Failures:
  - Ensure Docker is running.
  - Check for any syntax errors in Dockerfile or docker-compose.yml.

- Frontend Not Loading:
  - Ensure all dependencies are installed.
  - Check the console for any errors.

- Logs and Monitoring 📊
  - Frontend Logs: Use the browser console for any frontend errors.

---------------------------------------------
---------------------------------------------

## Why These Technologies? 🤔

- React + TypeScript + Vite: Ensures a fast and modern frontend development experience.
- Firebase: Provides robust authentication and storage solutions.
- Lucide Icons: Offers a wide range of icons for UI.
- Material-UI: Provides a set of UI components for faster development.
- Docker: Ensures consistent environments across different stages of development.

---------------------------------------------
---------------------------------------------

## Contributing 🤝

**I welcome a contribution! If you can do it better, especially the front end, please submit a PR and I'll review it with appreciation!!!**

- License 📜

    ```txt
    This project is licensed under the MIT License.
    ```