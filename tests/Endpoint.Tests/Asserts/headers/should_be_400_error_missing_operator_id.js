client.test("Request should return 400 error", function () {
    client.assert(response.status === 400, "Response status is Not 400");
});

client.test("Should get invalid Operator Id", function () {
    client.assert(response.status === 400, "Response status is 400");

    var jsonData = response.body;
    client.assert(jsonData.error.code === "missing_header", "Error code is not 'missing_header'");
    client.assert(jsonData.error.message === "Header x-operator-id is required.", "Error message is incorrect");
    client.assert(jsonData.error.details.header === "x-operator-id", "Details header is not 'x-operator-id'");

});