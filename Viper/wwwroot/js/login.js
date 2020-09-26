function login() {
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "Login";
    input.account = $("input[name=Account]").val();
    input.pwd = $("input[name=Pwd]").val();
    bif.process(input, function (data, status) {
        if (status === "success" && data.msg === null) {
            localStorage.token = data.outputData.profile;
            localStorage.account = data.outputData.account;
            localStorage.profile = JSON.stringify(data.outputData);
            window.location.href = "/";
        } else {
            $("#msg").html(data.msg);
        }
    });
}