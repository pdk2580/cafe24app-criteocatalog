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

$("#malls").change(function () {
    updateMallInfo();
});

$("#btnMain").click(function () {
    var partnerId = checkParterId();
    var enableNpay = checkEnableNpay();

    var beforeHtmlString = $("#beforeMain").val();
    var template = getTemplate().replace("$partnerId$", partnerId).replace("$enableNpayTag$", enableNpay);
    var afterHtmlString = addCriteoTag(beforeHtmlString, template);

    $("#afterMain").val(afterHtmlString);
});

$("#btnCommon").click(function () {
    var partnerId = checkParterId();
    var enableNpay = checkEnableNpay();

    var beforeHtmlString = $("#beforeCommon").val();
    var template = getTemplate().replace("$partnerId$", partnerId).replace("$enableNpayTag$", enableNpay);
    var afterHtmlString = addCriteoTag(beforeHtmlString, template);

    $("#afterCommon").val(afterHtmlString);
});

function addCriteoTag(currentHtml, template) {
    var tagVersion = getTagVersion(currentHtml);
    if (tagVersion !== null) {
        alert("Criteo 태그가 이미 적용되어 있습니다. 현재 버전: " + tagVersion);
        return "";
    }

    var indexOfBody = currentHtml.indexOf("</body>");
    if (indexOfBody < 0) {
        alert("</body> 태그를 찾을수 없습니다");
        return "";
    }

    return currentHtml.slice(0, indexOfBody) + template + currentHtml.slice(indexOfBody);
}

function getTagVersion(currentHtml) {
    return currentHtml.match(/(?<=\bdata-version=")[^"]*/g);
}

function getTemplate() {
        return $.ajax({
            type: "GET",
            url: "templates/onetag.html",
            async: false
        }).responseText;
}

function checkEnableNpay() {
    return document.getElementById("cbNpay").checked;
}

function checkParterId() {
    var partnerId = $("#txtPartnerId").val();
    if (partnerId === "") {
        alert("파트너 아이디를 입력해 주시기 바랍니다")
        throw new Error("Partner ID cannot be null");
    } else {
        return partnerId;
    }
}

function updateMallInfo() {
    var yourSelect = document.getElementById("malls");
    var selectedMallId = yourSelect.options[yourSelect.selectedIndex].value;
    var selectedLastUpdate = yourSelect.options[yourSelect.selectedIndex].getAttribute("data-last-updated");

    if (selectedMallId !== "default") {
        $("#txtFeedURL").val(window.location.protocol + "//" + window.location.host + "/catalogs/" + selectedMallId);
        $("#lastUpdated").val(selectedLastUpdate);
    } else {
        $("#txtFeedURL").val("");
        $("#lastUpdated").val("");
    }
}

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