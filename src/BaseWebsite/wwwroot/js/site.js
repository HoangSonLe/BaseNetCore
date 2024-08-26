function filterCustom(item, input) {
    let nonUnicode = removeVietnameseTones(item);
    input = removeVietnameseTones(input);
    let res = nonUnicode.toLowerCase().includes(input.toLowerCase());
    return res;
}
function handleFileUrl(rawUrl) {
    return rawUrl ? `${appSettingJs.hostUsing.mediaFTP}${rawUrl}` : "../content/images/noImage.png";
}
function getBase64(file) {
    var reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
        return reader.result;
    };
    reader.onerror = function (error) {
        console.log('Error: ', error);
    };
}

function closePrint() {
    document.body.removeChild(this.__container__);
}

function setPrint() {
    this.contentWindow.__container__ = this;
    this.contentWindow.onbeforeunload = closePrint;
    this.contentWindow.onafterprint = closePrint;
    this.contentWindow.focus(); // Required for IE
    this.contentWindow.print();
}

function printPage(sURL) {
    var oHiddFrame = document.createElement("iframe");
    oHiddFrame.onload = setPrint;
    oHiddFrame.style.visibility = "hidden";
    oHiddFrame.style.position = "fixed";
    oHiddFrame.style.right = "0";
    oHiddFrame.style.bottom = "0";
    oHiddFrame.src = sURL;
    document.body.appendChild(oHiddFrame);
}

function printHtmlInNewPage(html) {
    var winPrint = window.open('', '_blank');//, 'left=0,top=0,toolbar=0,scrollbars=0,status=0'
    winPrint.document.write(html);
    winPrint.document.close();
    winPrint.focus();
    setTimeout(() => {
        winPrint.print();
    }, 1200);
    //winPrint.close();
    winPrint.onafterprint = function () {
        winPrint.close();
    }
}

async function printWordFilesInFolder() {
    var fileNames = [
        "Phạm Đình Kiên_document.docx",
        "Phạm Đình Thắng_document.docx",
        // Add more file paths here
    ];

    for (const fileName of fileNames) {
        if (fileName.endsWith(".docx")) {
            //const filePath = folderPath + "/" + fileName;
            const fileContent = await readFileContent(fileName);
            const htmlContent = "<div>" + fileContent + "</div>";
            printHtmlInNewPage(htmlContent);
        }
    }
}


async function readFileContent(filePath) {
    const response = await fetch(`/PrintDocument/GetFileContent?filePath=${encodeURIComponent(filePath)}`);
    if (response.ok) {
        const content = await response.text();
        return content;
    } else {
        console.error("Error reading file:", response.status);
        return "";
    }
}

function exportHtmlToWord(element, filename = '') {
    //var preHtml = "<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'><head><meta charset='utf-8'><title>Export HTML To Doc</title></head><body>";
    var preHtml = "<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'><head><meta charset='utf-8'><title>Export HTML to Doc</title></head><body>";

    var postHtml = "</body></html>";
    //var html = preHtml + document.getElementById(element).innerHTML + postHtml;
    var html = preHtml + element + postHtml;

    var blob = new Blob(['\ufeff', html], {
        type: 'application/msword'
    });

    // Specify link url
    var url = 'data:application/vnd.openxmlformats-officedocument.wordprocessingml.document;charset=utf-8,' + encodeURIComponent(html);

    // Specify file name
    filename = filename ? filename + '.docx' : 'document.docx';

    // Create download link element
    var downloadLink = document.createElement("a");

    document.body.appendChild(downloadLink);

    if (navigator.msSaveOrOpenBlob) {
        navigator.msSaveOrOpenBlob(blob, filename);
    } else {
        // Create a link to the file
        downloadLink.href = url;

        // Setting the file name
        downloadLink.download = filename;

        //triggering the function
        downloadLink.click();
    }

    document.body.removeChild(downloadLink);
}

function ExportDocx(blob, fileName) {
    var url = window.URL || window.webkitURL;
    var downloadUrl = url.createObjectURL(blob);

    var a = document.createElement("a");
    a.style.display = "none";

    if (typeof a.download === "undefined") {
        window.location = downloadUrl;
    } else {
        a.href = downloadUrl;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
    }
}

function ajaxCallApi(type, url, databody, Authentication) {
    $.ajax({
        type: type,
        url: url,
        success: function (d) {

        }
    })
}

function downloadFile(data) { //data là blob
    const contentType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
    const blob = new Blob([data], { type: contentType });
    const url = window.URL.createObjectURL(blob);
    window.open(url);
}

function downloadFile(file_name, content) {
    var csvData = new Blob([content], { type: 'text/csv' });
    if (window.navigator && window.navigator.msSaveOrOpenBlob) { // for IE
        window.navigator.msSaveOrOpenBlob(csvData, file_name);
    } else { // for Non-IE (chrome, firefox etc.)
        var a = document.createElement("a");
        document.body.appendChild(a);
        a.style = "display: none";
        var csvUrl = URL.createObjectURL(csvData);
        a.href = csvUrl;
        a.download = file_name;
        a.click();
        URL.revokeObjectURL(a.href)
        a.remove();
    }
}

function formatCurency(val) {
    val = val.toString();
    let tmp = Number(removeFormatCurency(val));
    return (tmp).toLocaleString('vi-VN');
}

function removeFormatCurency(val) {
    return Number((val).replaceAll(".", "").replaceAll(",", ""));
}
function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

(function ($) {
    $.fn.hasScrollBar = function () {
        return this.get(0).scrollHeight > this.height();
    }
})(jQuery);

function FixHeaderColumn(e) {//grid
    setTimeout(() => {
        let grid = $(e);
        if ($(grid).find(".k-grid-content").height() < $(grid).find(".k-grid-content table").height()) {
            $(grid).find(".k-grid-header").css("padding-right", "17px");
        }
        else {
            $(grid).find(".k-grid-header").css("padding-right", "0px");
        }
    }, 300);
}

function ErorDirection(code) {
    switch (code) {
        case "NOT_FOUND":
            return "Tuyến ít nhất một vị trí không thể định vị.";
        case "ZERO_RESULTS":
            return "Không có tuyến đường nào có thể được tìm thấy.";
        case "MAX_WAYPOINTS_EXCEEDED":
            return "Quá nhiều điểm trong tuyến đường.";
        case "MAX_ROUTE_LENGTH_EXCEEDED":
            return "Tuyến đường được yêu cầu quá dài và không thể xử lý được.";
        case "INVALID_REQUEST":
            return "Yêu cầu chỉ đường được cung cấp không hợp lệ.";
        case "OVER_QUERY_LIMIT":
            return "Trang web đã gửi quá nhiều yêu cầu trong khoảng thời gian cho phép.";
        case "REQUEST_DENIED":
            return "Trang web không được phép sử dụng dịch vụ chỉ đường.";
        case "UNKNOWN_ERROR":
            return "Dịch vụ chỉ đường không thể được xử lý do lỗi máy chủ.";
        default:
            return "Không biết lỗi gì!";
    }
}

function ModayInLastWeekNearlest() {
    let date = new Date(); date.setDate(date.getDate() - (date.getDay() || 7) + 1 - 7)
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    date.setMilliseconds(0);
    return date;
}
function ModayInWeekNearlest() {
    let date = new Date(); date.setDate(date.getDate() - (date.getDay() || 7) + 1)
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    date.setMilliseconds(0);
    return date;
}

function SundayInWeekNearlest() {

    let date = new Date(); date.setDate(date.getDate() + 7 - (date.getDay() || 7))
    date.setHours(23);
    date.setMinutes(59);
    date.setSeconds(59);
    date.setMilliseconds(0);
    return date;
}

function getFirstDateOfMonth(d) {
    let d1 = new Date(d);
    return new Date(d1.setDate(1));
}

function getMonday(d) {
    d = new Date(d);
    let day = d.getDay(),
        diff = d.getDate() - day + (day == 0 ? -6 : 1);
    return new Date(d.setDate(diff));
}

function setCookie(cname, cvalue, exdays) {
    const d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function addScript(src) {
    var s = document.createElement('script');
    s.setAttribute('src', src);
    document.body.appendChild(s);
}

function printHtmlInNewPage(html) {
    //var winPrint = window.open('', 'Print-Window', 'left=0,top=0,toolbar=0,scrollbars=0,status=0');
    let winPrint = window.open('', 'Print-Window');
    winPrint.document.open();
    winPrint.document.write(html);
    winPrint.document.close();
    winPrint.focus();
    setTimeout(() => {
        winPrint.print();
    }, 1200);

    //winPrint.onafterprint = function () {
    //    winPrint.close();
    //}
}

function ExportDoc(html, filename, isPageDoc) {
    let div = document.createElement("div");
    div.id = "export-content";
    div.style.display = "none";
    div.innerHTML = html;
    document.body.appendChild(div);
    $("#export-content").wordExport(filename);
    document.body.removeChild(div);
}


function exportExcel(data, filename) {
    let bytes = Base64ToBytes(data);
    //Convert Byte Array to BLOB.
    let blob = new Blob([bytes], { type: "application/octetstream" });
    //Check the Browser type and download the File.
    let isIE = false || !!document.documentMode;
    if (isIE) {
        window.navigator.msSaveBlob(blob, filename);
    } else {
        let url = window.URL || window.webkitURL;
        link = url.createObjectURL(blob);
        let a = $("<a />");
        a.attr("download", filename);
        a.attr("href", link);
        $("body").append(a);
        a[0].click();
        a.remove();
    }
}
function Base64ToBytes(base64) {
    var binary_string = window.atob(base64);
    var len = binary_string.length;
    var bytes = new Uint8Array(len);
    for (var i = 0; i < len; i++) {
        bytes[i] = binary_string.charCodeAt(i);
    }
    return bytes.buffer;
}

Date.prototype.addMonth = function (months) {
    let date = new Date(this.valueOf());
    date.setMonth(date.getMonth() + months);
    return date;
}

Date.prototype.addDays = function (days) {
    let date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
}

Date.prototype.addTime = function (times) {
    let date = new Date(this.valueOf());
    date.setHours(date.getHours() + times);
    return date;
}
Date.prototype.addMinutes = function (minutes) {
    let date = new Date(this.valueOf());
    date.setMinutes(date.getMinutes() + minutes);
    return date;
}

Date.prototype.toJSON = function () {
    const hoursDiff = this.getHours() - this.getTimezoneOffset() / 60;
    let date = new Date(this.getTime());
    date.setHours(hoursDiff);
    return date.toISOString();
};
function isEmptyObject(value) {
    return Object.keys(value).length === 0 && value.constructor === Object;
}
function getDataFromObject(sourceHasData, destinationObject) {
    let tmp = destinationObject;
    let listProperty = Object.getOwnPropertyNames(sourceHasData)
    listProperty.forEach(e => {
        if (destinationObject.hasOwnProperty(e)) tmp[e] = sourceHasData[e];
    })
    return tmp;
}


function checcktime(elfrom, elto, message) {

    let fromdate = $("#" + elfrom).data("kendoDatePicker").value();
    let todate = $("#" + elto).data("kendoDatePicker").value();
    if (fromdate != null && todate != null && fromdate > todate) {
        notification.show({
            title: "Thông báo",
            message: message
        }, "error");
        return false;
    }
    return true;
}

function validate(evt) { //onkeypress="validate(event)"
    var theEvent = evt || window.event;

    // Handle paste
    if (theEvent.type === 'paste') {
        key = event.clipboardData.getData('text/plain');
    } else {
        // Handle key press
        var key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
    }
    var regex = /[0-9]|\./;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}

function validate_has_regex(evt, regex = /[0-9]|\./) { //onkeypress="validate_has_regex(event,regex)"
    var theEvent = evt || window.event;

    // Handle paste
    if (theEvent.type === 'paste') {
        key = event.clipboardData.getData('text/plain');
    } else {
        // Handle key press
        var key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
    }
    //var regex = /[0-9]|\./;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}

function validateOnlyNumber(evt) {
    var theEvent = evt || window.event;

    // Handle paste
    if (theEvent.type === 'paste') {
        key = event.clipboardData.getData('text/plain');
    } else {
        // Handle key press
        var key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
    }
    //var regex = /[0-9]|\./;
    var regex = /[0-9]/;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}
function removeVietnameseTones(str) {
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    // Some system encode vietnamese combining accent as individual utf-8 characters
    // Một vài bộ encode coi các dấu mũ, dấu chữ như một kí tự riêng biệt nên thêm hai dòng này
    str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // ̀ ́ ̃ ̉ ̣  huyền, sắc, ngã, hỏi, nặng
    str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // ˆ ̆ ̛  Â, Ê, Ă, Ơ, Ư
    // Remove extra spaces
    // Bỏ các khoảng trắng liền nhau
    str = str.replace(/ + /g, " ");
    str = str.trim();
    // Remove punctuations
    // Bỏ dấu câu, kí tự đặc biệt
    str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
    return str;
}

String.prototype.Format = function () {
    var regex = /{-?[0-9a-zA-Z_]+}/g;
    var args = arguments.length == 1 ? arguments[0] : arguments.length > 1 ? arguments : null;
    if (args == null) return "";
    var seq = 0;
    return this.replace(regex, function (item) {
        var replace = "";
        var key = item.substring(1, item.length - 1);

        if (isNaN(key)) {
            // format type is {a-z}
            if (args.length)
                replace = args[seq++];
            else
                replace = args[key.toString()];
            replace = replace == null || replace == 'undefined' ? '' : replace;
        }
        else {
            // format type is {0-9}
            key = parseInt(key);
            if (args.length) {
                if (key >= 0) {
                    replace = args[key];
                } else if (key === -1) {
                    replace = "{";
                } else if (key === -2) {
                    replace = "}";
                }
            }
            else {
                var i = 0;
                for (var a in args) {
                    if (args.hasOwnProperty(a) && typeof (a) !== 'function') {
                        if (i == key) {
                            replace = args[a];
                            break;
                        }
                        i++;
                    }
                }
            }
        }
        return replace;
    });
};

// Write your JavaScript code.
var debounceTimeout;
var debounceDelay = 3000; // Adjust the delay as needed
function showMessages(type = "success",messageList = []) {
    if (messageList?.length > 0) {
        for (let i = 0; i < messageList.length; i++) {
            notification.show({
                title: "Thông báo",
                message: messageList[i]
            }, type);
        }
    }
    else {
        notification.show({
            title: "Thông báo",
            message: type == "success" ? "Thành công" : "Lỗi"
        }, type);
    }
}
function showErrorMessages(errorMessageList = []) {
    showMessages("error", errorMessageList);
}
function showSuccessMessages(successMessageList) {
    showMessages("success", successMessageList);

}
function callbackSuccessFunc(response, callbackSuccess, isShowSuccessAlert) {
    isShowSuccessAlert ? showSuccessMessages(response.successMessageList) : null;
    typeof callbackSuccess == "function" && callbackSuccess(response);
}
function callbackErrorFunc(response, callbackError) {
    showErrorMessages(response.errorMessageList);
    typeof callbackError == "function" && callbackError(response);
}
function ajax(method = "GET", url = "", data, callbackSuccess, callbackError, isShowSuccessAlert = true) {
    let loadingTimeout;
    return $.ajax({
        url: url,
        type: method,
        datatype: "json",
        contentType: "application/json",
        data: method == "GET" ? data : JSON.stringify(data),
        beforeSend: function () {
            loadingTimeout = setTimeout(showLoading, 200); 
        },
        success: function (response) {
            if (response.isSuccess) {
                callbackSuccessFunc(response, callbackSuccess, isShowSuccessAlert);
            }
            else {
                callbackErrorFunc(response, callbackError);
            }
        },
        error: function (response) {
            if (response.status == 403) {
                window.location.href = "/home/error"
            }
            else {
                notification.show({
                    title: "Thông báo",
                    message: "Lỗi"
                }, "error");
            }
           
            console.log("response", response);
        },
        complete: function () {
            clearTimeout(loadingTimeout); // Clear the timeout to prevent the loading indicator from showing
            hideLoading();
        }
    });
}
function showLoading() {
    kendo.ui.progress($("#loadingIndicator"), true);
    $("#loadingIndicator").show();
}

function hideLoading() {
    kendo.ui.progress($("#loadingIndicator"), false);
    $("#loadingIndicator").hide();
}
function OpenImage(image, titles = "Sơ đồ lưu trữ", styles) {
    let body = document.querySelector("body");
    let PopupId = "#divPopupImage";
    let divPopup = null;
    if ($(divPopup).length == 0) {
        divPopup = document.createElement("div");
        divPopup.id = "divPopupImage";
        body.append(divPopup);
    }
    else {
        divPopup = document.querySelector(PopupId);
    }

    let html = `<div class="d-flex align-items-center justify-content-center"><img src="${image}" style="${styles}" class="" /></div>`;

    divPopup.innerHTML = (html);

    let myWindow = $(divPopup);
    myWindow.kendoWindow({
        width: "80%",
        height: "80%",
        maxHeight: "100%",
        title: "",
        visible: false,
        actions: ["Maximize", "Close"],
        resizable: false,
        draggable: false,
        modal: true,
        close: function (e) {
            //$("#window").empty();
            // remove();
        },
    }).data("kendoWindow").title(titles).center();
    myWindow.data("kendoWindow").open();
}
function debounce(fn, delay) {
    return function () {
        var context = this, args = arguments;
        clearTimeout(debounceTimeout);
        debounceTimeout = setTimeout(function () {
            fn.apply(context, args);
        }, delay);
    };
}
AuthenticationFunc = () => {
    ajax("GET", "/User/Authentication", null, (response) => {
        $("#pnAccount").text("Xin chào: " + response.data.name);
        Userdata = response.data;
    }, () => {
        location.href = 'Account/Logout';
    }, false);
} 
String.prototype.removeSpecialCharater = function () {
    return this.replace(/[^\x00-\x7F]/g, "").trim();
}