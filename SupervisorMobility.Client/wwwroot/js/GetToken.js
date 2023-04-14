function getToken(username, password) {
    var promise = new Promise(function (resolve, reject) {
        var scopes = ["user.read"];

        var app = new msal.PublicClientApplication({
            auth: {
                clientId: "7a184926-2f58-4f9c-872c-97d54d825912",
                authority: "https://login.microsoftonline.com/84539953-c856-42b8-a26c-a60e5362d3e4"
            }
        });

        app.acquireTokenByUsernamePassword(scopes, username, password)
            .then(function (response) {
                resolve(response.accessToken);
            })
            .catch(function (error) {
                reject(error);
            });
    });

    return promise;
};
