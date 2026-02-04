client.test("Should be 201 Created", () => {
    client.assert(response.status === 201, "HTTP Response status code is not 201");
});