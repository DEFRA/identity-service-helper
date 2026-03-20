# Light weight DB utilities app

This app will run a script againt a database. Sample below is if it is run form the `bin/debug/net10.0` folder 

## Usage
```bash
./DataSeeder run -db postgresql://localhost:5432/identity_service_helper \
  -uid identity_service_helper_ddl \
  -pwd postgres \
  -script ../../../../../../changelog/schema/testcontainer.postgresql.sql
```