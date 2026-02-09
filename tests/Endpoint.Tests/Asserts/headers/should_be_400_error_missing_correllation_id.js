client.test("Request should return 400 error", function () {
    client.assert(response.status === 400, "Response status is NOT 400");
});

client.test("Should get invalid Correlation Id", function () {
    client.assert(response.status === 400, "Response status is NOT 400");

    var jsonData = response.body;
    client.assert(jsonData.error.code === "missing_header", "Error code is not 'missing_header'");
    client.assert(jsonData.error.message === "Header x-correlation-id is required.", "Error message is incorrect");
    client.assert(jsonData.error.path === "/users", "Path is not '/users'");
    client.assert(jsonData.error.details.header === "x-correlation-id", "Details header is not 'x-correlation-id'");

});
