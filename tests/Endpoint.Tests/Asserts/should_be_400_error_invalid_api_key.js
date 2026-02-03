client.test("Request should return 400 error", function () {
    client.assert(response.status === 400, "Response status is not 400");
});

client.test("Should get invalid API Key payload", function () {
    client.assert(response.status === 400, "Response status is not 400");

    var jsonData = response.body;
    client.assert(jsonData.error.code === "Invalid header", "Error code is not 'Invalid header'");
    client.assert(jsonData.error.message === "Header x-api-key is required.", "Error message is incorrect");
    client.assert(jsonData.error.path === "/users", "Path is not '/users'");
    client.assert(jsonData.error.details.header === "x-api-key", "Details header is not 'x-api-key'");

});
