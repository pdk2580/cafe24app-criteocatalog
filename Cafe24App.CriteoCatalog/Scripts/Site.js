$("#btnInstall").click(function () {
    var mallId = $("#txtMallId").val();
    if (mallId !== "") {
        var clientId = "jfteFzqAlURv2XMrCUuFFL";
        var encodeCsrfToken = generateRandomString(12);
        var scope = "mall.read_product,mall.read_category";
        var encodeRedirectUri = "https://criteo-catalog.azurewebsites.net/oauth";

        if (typeof (Storage) !== "undefined") {
            window.localStorage.setItem("mallId", mallId);
        }

        window.location.href=String.format("https://{0}.cafe24api.com/api/v2/oauth/authorize?response_type=code&client_id={1}&state={2}&redirect_uri={3}&scope={4}", mallId, clientId, encodeCsrfToken, encodeRedirectUri, scope), "_blank";
    } else {
        alert("상점 아이디를 입력해 주시기 바랍니다")
    }
});

$("#btnUrl").click(function () {
    var mallId = $("#txtMallId").val();
    if (mallId !== "") {
        window.location.href=String.format("/catalogs/{0}", mallId);
    } else {
        alert("상점 아이디를 입력해 주시기 바랍니다")
    }
});

function generateRandomString(length) {
    var result = '';
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for (var i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
}

String.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }

    return s;
}