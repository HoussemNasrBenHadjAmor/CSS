#!/bin/bash
set -e

echo "Starting SQL Server..."
/opt/mssql/bin/sqlservr &

echo "Waiting for SQL Server to be ready..."
until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" >/dev/null 2>&1; do
	sleep 2
done

echo "Checking HRDB existence..."
DB_EXISTS=$(/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -h -1 -Q "SET NOCOUNT ON; IF DB_ID('HRDB') IS NULL SELECT 0 ELSE SELECT 1" | tr -d '[:space:]')

if [ "$DB_EXISTS" = "0" ]; then
	echo "HRDB not found. Running init.sql..."
	/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -i /init.sql
	echo "init.sql completed."
else
	echo "HRDB already exists. Skipping init.sql."
fi

wait