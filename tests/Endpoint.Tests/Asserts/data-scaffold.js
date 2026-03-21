console.log("Resetting test data...");
const dotnetRoot = request.environment.get("dotnet-root");
const dbUrl = request.environment.get("db-url");
const dbUser = request.environment.get("db-user");
const dbPassword = request.environment.get("db-passowrd");
const repoRoot = request.environment.get("repo-root");

const result = execSync(
    `${dotnetRoot}/dotnet run --project src/Database/DataSeeder/DataSeeder.csproj -- run ` +
    `-db ${dbUrl} ` +
    `-uid ${dbUser} ` +
    `-pwd ${dbPassword} ` +
    "-script changelog/schema/testcontainer.postgresql.sql",
    { cwd: repoRoot, timeout: 120000 }
);

console.log(result.toString());
console.log("DataSeeder completed successfully");
