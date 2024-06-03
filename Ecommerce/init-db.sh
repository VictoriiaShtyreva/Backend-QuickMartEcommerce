#!/bin/bash

# Wait for PostgreSQL to be ready
until pg_isready -h localhost -p 5432 -U test_admin; do
  echo "Waiting for PostgreSQL..."
  sleep 2
done

# Command to create the database if it doesn't exist
echo "Creating database if it does not exist..."
psql -h localhost -U test_admin -tc "SELECT 1 FROM pg_database WHERE datname = 'ecommerce_platform'" | grep -q 1 || psql -h localhost -U test_admin -c "CREATE DATABASE ecommerce_platform"

# Run your database migrations here
echo "Running database migrations..."
dotnet ef database update --no-build -s /publish/Ecommerce.WebAPI.dll
