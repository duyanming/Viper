function login() {
    var input = anno.getInput();
    input.account = $("input[name=Account]").val();
    input.pwd = $("input[name=Pwd]").val();
    anno.process(input,"Anno.Plugs.Logic/Platform/Login", function (data, status) {
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