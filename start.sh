#!/bin/bash
# wait for SQL Server to start
echo "Waiting for SQL Server to start..."
sleep 20s  # or use a proper check

# run your init.sql
echo "Running init.sql..."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -i /init.sql # THIS IS NOT WORKING
# /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'StrongPassword123!' -C -i /init.sql

# keep the container running
tail -f /dev/null


# RUN THIS :  docker exec -it benca_sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'StrongPassword123!' -C -i /init.sql